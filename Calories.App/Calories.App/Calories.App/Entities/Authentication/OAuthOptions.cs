using System;
using System.Collections.Generic;
using System.Text;

namespace Calories.App.Entities.Authentication
{
    public class OAuthOptions
    {
        /// <summary>Auth0 tenant domain.</summary>
        public readonly string Domain;

        /// <summary>Auth0 Client ID.</summary>
        public readonly string ClientId;

        /// <summary>The scopes you want to request. e.g "openid email".</summary>
        public readonly string Scope;


        /// <param name="domain">Your Auth0 tenant domain.</param>
        /// <param name="clientId">Your Auth0 Client ID.</param>
        /// <param name="scope">The scopes you want to request. e.g "openid email".</param>
        public OAuthOptions(string domain, string clientId, string scope)
        {
            this.Domain = domain;
            this.ClientId = clientId;
            this.Scope = scope;
        }

    }
}
