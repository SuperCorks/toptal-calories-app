using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Calories.App.Adapters;
using Calories.App.Services;
using Calories.App.Serialization;
using Calories.App.Entities.Authentication;

namespace Calories.App.Managers
{
    public class AuthenticationManager
    {

        #region Private Property -------------------------------------
        /// <summary>Key to store/get the local session from the local storage</summary>
        private const string LocalSessionKey = "LocalSession";

        private StringValueStore StringValueStore => Injector.Get<StringValueStore>();

        private IOAuthAdapter OAuthAdapter => Injector.Get<IOAuthAdapter>();

        private AuthenticationService AuthenticationService => Injector.Get<AuthenticationService>();

        #endregion

        #region Public Property -------------------------------------
        /// <summary>Current Session (can be expired)</summary>
        public SessionInfo CurrentSession { get; private set; }
        #endregion

        /// <summary>Main Constructor.</summary>
        public AuthenticationManager()
        {
            this.LoadCurrentSessionFromLocalStorage();
        }


        /// <summary>Load the local session to the current session.</summary>
        public async void LoadCurrentSessionFromLocalStorage()
        {   // Inside a separate class since constructor cannot be async.
            this.CurrentSession = await this.GetSessionFromLocalStorage();
        }


        /// <summary>Throws an error if the current session is inactive.</summary>
        /// <exception cref="NotAuthenticatedError" />
        public void ThrowIfNotAuthenticated()
        {
            if (this.CurrentSession == null || this.CurrentSession.IsExpired) throw new Exception("not_authenticated");
        }


        /// <summary>
        /// Authenticates a user. If the current session is still active, this does nothing.
        /// </summary>
        /// 
        /// <returns>
        /// A task that completes when the user is authenticated and the current
        /// <see cref="SessionInfo"/> is activated on the server.
        /// </returns>
        public async Task<SessionInfo> Login()
        {
            try
            {
                if (this.CurrentSession == null || this.CurrentSession.IsExpired)
                {
                    // The OAuth2 access token that gives the user the permission to create a new session
                    var accessToken = await OAuthAdapter.GetLocalSessionAccessToken();

                    if (accessToken == null || accessToken.IsExpired)
                    {
                        accessToken = await OAuthAdapter.ExecuteLogin();   // will create or renew the session access token
                                                                           // and stores it locally
                    }

                    this.CurrentSession = await this.CreateNewSession(accessToken);
                }

            }
            catch (Exception error)
            {
                await this.SetSessionToLocalStorage(null);
                this.CurrentSession = null;
            }
                
            return this.CurrentSession;
        }


        ///// <summary>
        ///// Create a new User using auth0
        ///// </summary>
        ///// 
        ///// <param name="andLogin"> If we should create a new session and log in the new user
        ///// after the sign up </param>
        ///// 
        ///// <returns> Return a Session if a new Session have been created return null otherwise </returns>
        //public async Task<SessionInfo> SignUp(bool andLogin = true)
        //{
        //    if (this.CurrentSession == null || !this.CurrentSession.IsActive)
        //    {
        //        // The OAuth2 access token that gives the user the permission to create a new session
        //        var accessToken = await OAuthAdapter.GetLocalSessionAccessToken();

        //        if (accessToken == null || accessToken.isExpired)
        //        {
        //            accessToken = await OAuthAdapter.ExecuteSignup();   // will create the session access token
        //                                                                // and stores it locally
        //        }
        //        var SignupParameters = new SignUpAuthenticationParams();
        //        SignupParameters.shouldLogin = andLogin;

        //        var signupPackage = new SignUpPackage();
        //        signupPackage.accessToken = accessToken;

        //        signupPackage = await AuthenticationService.SignUp(signupPackage, SignupParameters);

        //        if (andLogin && signupPackage.sessionInfo != null)
        //        {

        //            this.CurrentSession = signupPackage.sessionInfo;

        //            await this.SetSessionToLocalStorage(signupPackage.sessionInfo);

        //            return this.CurrentSession;
        //        }
        //    }
        //    return null;
        //}


        /// <summary>
        /// Delete the local session and logout from the OAuth adapter.
        /// </summary>
        /// 
        /// <returns>Return a void task.</returns>
        public async Task LogOut()
        {
            this.CurrentSession = null;
            await StringValueStore.Remove(LocalSessionKey);
            await OAuthAdapter.ExecuteLogout();
        }


        /// <summary>
        /// Creates and stores a new authentication session on the server and caches it locally.
        /// </summary>
        /// 
        /// <param name="sessionAccessToken">Access Token from OAuth2 to authenticate the user.</param>
        /// 
        /// <returns>
        /// A task that completes when the session is created on the server and returns
        /// the session info that corresponds to the created session.
        /// </returns>
        private async Task<SessionInfo> CreateNewSession(SessionAccessToken sessionAccessToken)
        {
            var sessionInfo = await AuthenticationService.Login(sessionAccessToken);

            await this.SetSessionToLocalStorage(sessionInfo);

            return sessionInfo;
        }


        /// <summary>Set a session to be stored inside the local storage.</summary>
        /// 
        /// <param name="SessionInfo">The <see cref="SessionInfo"/> to store.</param>
        /// 
        /// <returns>A task that completes when the SessionInfo is stored in the local storage.</returns>
        private Task SetSessionToLocalStorage(SessionInfo SessionInfo)
        {
            return StringValueStore.Set(LocalSessionKey, Serializer.ToJson(SessionInfo));
        }


        /// <summary>Get a stored session from the local storage.</summary>
        /// 
        /// <returns>
        /// A task that completes when the SessionInfo is recovered from local storage. The task's result 
        /// is <see langword="null"/> if nothing is stored.
        /// </returns>
        private async Task<SessionInfo> GetSessionFromLocalStorage()
        {
            try
            {
                return (Serializer.FromJson<SessionInfo>(await StringValueStore.Get(LocalSessionKey)));
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

    }
}
