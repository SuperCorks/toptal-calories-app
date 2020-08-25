using System;
using Calories.App.Models;
using System.Threading.Tasks;
using Calories.App.Serialization;
using Calories.App.Entities.Authentication;

namespace Calories.App.Services
{
    [Injector.Singleton]
    public class AuthenticationService
    {
        private readonly IHttpClient http;

        public AuthenticationService(IHttpClient http)
        {
            this.http = http;
        }

        public async Task<SessionInfo> Login(SessionAccessToken token)
        {
            var response = await http.Post($"{Constants.ServerUrl}/authentication/session", token);

            if (response.IsSuccessStatusCode) 
                return Serializer.FromJson<SessionInfo>(await response.Content.ReadAsStringAsync());

            throw new Exception($"{response.StatusCode}!");
        }
    }
}
