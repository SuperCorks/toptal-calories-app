using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Calories.App.Entities.Authentication;

namespace Calories.App.Adapters
{
    /// <summary>Adapter class for the Oauth2 protocol.</summary>
    public abstract class IOAuthAdapter
    {

        private const string LocalAccessTokenStorageKey = "OAuthSessionAccessToken";

        private StringValueStore StringValueStore => Injector.Get<StringValueStore>();


        /// <summary>
        /// Main Constructor for the OAuth Adapter
        /// </summary>
        /// 
        /// <param name="options">Options for the 0Auth client</param>
        public IOAuthAdapter(OAuthOptions options)
        {
            this.InitClient(options ?? throw new ArgumentNullException(nameof(options)));
        }


        /// <summary>Initialize the auth0 client.</summary>
        protected abstract void InitClient(OAuthOptions options);


        /// <summary>Open the OAuth2 login page.</summary>
        /// 
        /// <param name="role">The role under which to log in the user.</param>
        /// 
        /// <returns>A task that completes when the access token retrieved from the OAuth2 login process.</returns>
        public async Task<SessionAccessToken> ExecuteLogin()
        {

            var accessToken = new SessionAccessToken(await this.ExecuteOAuthLogin());

            this.SetLocalSessionAccessToken(accessToken);

            return accessToken;
        }



        /// <summary>Gets an access token from a new user by showing an OAuth2 sign up form.</summary>
        /// 
        /// <returns>A task that completes when the access token retrieved from the OAuth2 signup process.</returns>
        public async Task<SessionAccessToken> ExecuteSignup()
        {

            var accessToken = new SessionAccessToken(await this.ExecuteOAuthLogin());

            this.SetLocalSessionAccessToken(accessToken);

            return accessToken;
        }



        /// <summary>Logs out OAuth's user.</summary>
        public abstract Task ExecuteLogout();


        /// <summary>
        /// Implementation will the login/signup form
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        protected abstract Task<string> ExecuteOAuthLogin();


        #region LocalStorage ------------------------------------------------------

        /// <summary>
        /// Retrieve from local storage the Session Access Token
        /// </summary>
        /// 
        /// <returns>Return the SessionAccessToken from the local storage</returns>
        public async Task<SessionAccessToken> GetLocalSessionAccessToken()
        {
            try
            {
                return new SessionAccessToken((await StringValueStore[LocalAccessTokenStorageKey]));
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }


        /// <summary>
        /// Set an Local Session Token to the local session.
        /// </summary>
        /// 
        /// <param name="SessionAccessToken">The Local Session Access Token to set to the local storage</param>
        public async void SetLocalSessionAccessToken(SessionAccessToken SessionAccessToken)
        {
            await StringValueStore.Set(LocalAccessTokenStorageKey, SessionAccessToken.RawJwt);
        }


        /// <summary>
        /// Delete the local stored Session Access Token
        /// </summary>
        public async void DeleteLocalSessionAccessToken()
        {
            await StringValueStore.Remove(LocalAccessTokenStorageKey);
        }

        #endregion
    }
}
