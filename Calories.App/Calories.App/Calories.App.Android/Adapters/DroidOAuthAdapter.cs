using System;
using System.Threading.Tasks;

using Auth0.OidcClient;

using Calories.App.Adapters;
using Calories.App.Entities.Authentication;

namespace Calories.App.Droid.Adapters
{
    internal class DroidOAuthAdapter : IOAuthAdapter
    {

        #region Private Property ---------------------------------

        private Auth0Client _auth0Client;

        #endregion

        public DroidOAuthAdapter(OAuthOptions options) : base(options) { }

        protected override void InitClient(OAuthOptions options)
        {
            _auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = options.Domain,
                ClientId = options.ClientId,
                Scope = options.Scope,
            });
        }

        protected override async Task<string> ExecuteOAuthLogin()
        {
            var loginResult = await _auth0Client.LoginAsync();

            if (loginResult.IsError) throw new Exception(loginResult.Error);

            return loginResult.AccessToken;
        }


        /// <summary>
        /// Logout from the application and delete the LocalSessionAccessToken
        /// </summary>
        public override async Task ExecuteLogout()
        {
            await _auth0Client.LogoutAsync();
            this.DeleteLocalSessionAccessToken();
        }
    }
}