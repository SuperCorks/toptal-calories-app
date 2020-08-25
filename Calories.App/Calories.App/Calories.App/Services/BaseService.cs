using System;
using Calories.App.Models;
using System.Threading.Tasks;
using Calories.App.Serialization;
using Calories.App.Managers;
using System.Collections.Generic;

namespace Calories.App.Services
{
    public class BaseService
    {
        private IHttpClient Http => Injector.Get<IHttpClient>();

        public readonly static Dictionary<string, string> Headers = new Dictionary<string, string>();
    
        protected async Task<T> HttpGet<T>(string url)
        {
            var response = await Http.Get($"{Constants.ServerUrl}{url}", Headers);

            if (response.IsSuccessStatusCode)
                return Serializer.FromJson<T>(await response.Content.ReadAsStringAsync());

            throw new Exception($"HTTP {response.StatusCode}!");
        }

        protected async Task<T> HttpPost<T>(string url, object body)
        {
            var response = await Http.Post($"{Constants.ServerUrl}{url}", body, Headers);

            if (response.IsSuccessStatusCode)
                return Serializer.FromJson<T>(await response.Content.ReadAsStringAsync());

            throw new Exception($"HTTP {response.StatusCode}!");
        }

        protected async Task HttpDelete(string url)
        {
            var response = await Http.Delete($"{Constants.ServerUrl}{url}", Headers);

            if (!response.IsSuccessStatusCode) 
                throw new Exception($"HTTP {response.StatusCode}!");
        }
    }
}
