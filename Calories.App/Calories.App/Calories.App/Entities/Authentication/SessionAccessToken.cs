using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace Calories.App.Entities.Authentication
{
    public class SessionAccessToken
    {

        /// <summary> The encoded Jwt of the access token. </summary>
        public string RawJwt;

        /// <summary> The Jwt of the Login. </summary>
        public Jwt Jwt;

        /// <summary>Whether this access token is expired.</summary>
        public bool IsExpired
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.RawJwt))
                {
                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    return unixTimestamp >= this.Jwt.Exp;
                }

                return true;
            }
        }

        /// <summary>Main constructor that will also decode JWT token.</summary>
        /// <param name="rawJwt"></param>
        public SessionAccessToken(string rawJwt)
        {
            if (!string.IsNullOrWhiteSpace(rawJwt))
            {
                this.RawJwt = rawJwt;

                var parsedJwt = new JwtSecurityTokenHandler().ReadToken(rawJwt) as JwtSecurityToken;

                this.Jwt = new Jwt
                {
                    Exp = parsedJwt.Payload.Exp ?? 0,
                    Iat = parsedJwt.Payload.Iat ?? 0,
                    Scope = parsedJwt.Payload.Sub,
                    Aud = parsedJwt.Payload.Aud?.ToArray(),
                };
            }
        }
    };

    /// <summary>Represent the JWT object.</summary>
    public class Jwt
    {
        /// <summary>The audience for this JWT.</summary>
        public string[] Aud { get; set; }

        /// <summary>The value `Issued at`.</summary>
        public int Iat { get; set; }

        /// <summary>The value `Expiration time`.</summary>
        public int Exp { get; set; }

        /// <summary>Get the scope (what the authorization asked for information e.g: email).</summary>
        public string Scope { get; set; }
    }
}
