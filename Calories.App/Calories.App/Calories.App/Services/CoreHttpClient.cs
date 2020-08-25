using Calories.App.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calories.App.Services
{
    public class CoreHttpClient : IHttpClient
    {
        /// <summary>HttpClient used for short api request (get, post, delete).</summary>
        private static readonly HttpClient _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };

        /// <summary>HttpClient used for long api request (file downloads and uploads).</summary>
        private static readonly HttpClient _filesHttpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(5) };

        /// <summary>Object used to flag the mutex on <see cref="HttpClient.DefaultRequestHeaders"/>.</summary>
        private static readonly object httpDefaultHeaderLock = new Object();

        /// <summary>
        /// Performs an HTTP GET request on the network.
        /// </summary>
        /// 
        /// <param name="uri">See <see cref="IHttpClient.Get"/> uri argument.</param>
        /// <param name="headers">See <see cref="IHttpClient.Get"/> headers argument.</param>
        /// <param name="ct">See <see cref="IHttpClient.Get"/> ct argument.</param>
        /// 
        /// <returns>See <see cref="IHttpClient.Get"/></returns>
        public Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> headers = null, CancellationToken? ct = null)
        {
            if (ct == null) ct = CancellationToken.None;

            lock (httpDefaultHeaderLock) // Avoid overwritting another request's headers
            {
                CoreHttpClient.RequestHeaders = headers; // Clear and set HttpClient.DefaultRequestHeaders 

                return _httpClient.GetAsync(uri, (CancellationToken)ct);
            }
        }

        /// <summary>
        /// Performs an HTTP GET request on the network.
        /// </summary>
        /// 
        /// <param name="uri">See <see cref="IHttpClient.Get"/> uri argument.</param>
        /// <param name="headers">See <see cref="IHttpClient.Get"/> headers argument.</param>
        /// <param name="ct">See <see cref="IHttpClient.Get"/> ct argument.</param>
        /// 
        /// <returns>See <see cref="IHttpClient.Get"/></returns>
        public Task<HttpResponseMessage> GetFile(string uri, Dictionary<string, string> headers = null, CancellationToken? ct = null)
        {
            if (ct == null) ct = CancellationToken.None;

            lock (httpDefaultHeaderLock) // Avoid overwritting another request's headers
            {
                CoreHttpClient.RequestHeaders = headers; // Clear and set HttpClient.DefaultRequestHeaders 

                return _filesHttpClient.GetAsync(uri, (CancellationToken)ct);
            }
        }

        /// <summary>
        /// Performs an HTTP POST request on the network.
        /// </summary>
        /// 
        /// <param name="uri">See <see cref="IHttpClient.Post"/> uri argument.</param>
        /// <param name="body">See <see cref="IHttpClient.Post"/> body argument.</param>
        /// <param name="headers">See <see cref="IHttpClient.Post"/> headers argument.</param>
        /// <param name="ct">See <see cref="IHttpClient.Post"/> ct argument.</param>
        /// 
        /// <returns>See <see cref="IHttpClient.Post"/></returns>
        public Task<HttpResponseMessage> Post(string uri, object body, Dictionary<string, string> headers = null, CancellationToken? ct = null)
        {
            if (ct == null) ct = CancellationToken.None;

            // Serialize to JSON
            StringContent postBody = new StringContent(Serializer.ToJson(body), Encoding.UTF8);

            postBody.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            lock (httpDefaultHeaderLock) // Avoid overwritting another request's headers
            {
                CoreHttpClient.RequestHeaders = headers; // Clear and set HttpClient.DefaultRequestHeaders 

                return _httpClient.PostAsync(uri, postBody, (CancellationToken)ct);
            }
        }

        public Task<HttpResponseMessage> PostFile(string uri, Stream fileStream, Dictionary<string, string> headers, CancellationToken? ct)
        {
            if (ct == null) ct = CancellationToken.None;

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", "file2");

            var contentType = content.Headers.ContentType;

            lock (httpDefaultHeaderLock) // Avoid overwritting another request's headers
            {
                CoreHttpClient.RequestHeaders = headers; // Clear and set HttpClient.DefaultRequestHeaders 

                return _filesHttpClient.PostAsync(uri, content, (CancellationToken)ct);
            }
        }

        /// <summary>
        /// Performs an HTTP DELETE request on the network.
        /// </summary>
        /// 
        /// <param name="uri">See <see cref="IHttpClient.Delete"/> uri argument.</param>
        /// <param name="headers">See <see cref="IHttpClient.Delete"/> headers argument.</param>
        /// <param name="ct">See <see cref="IHttpClient.Delete"/> ct argument.</param>
        /// 
        /// <returns>See <see cref="IHttpClient.Delete"/></returns>
        public Task<HttpResponseMessage> Delete(string uri, Dictionary<string, string> headers = null, CancellationToken? ct = null)
        {
            if (ct == null) ct = CancellationToken.None;

            lock (httpDefaultHeaderLock) // Avoid overwritting another request's headers
            {
                CoreHttpClient.RequestHeaders = headers; // Clear and set HttpClient.DefaultRequestHeaders 

                return _httpClient.DeleteAsync(uri, (CancellationToken)ct);
            }
        }

        /// <summary>
        /// Clears and sets the <see cref="HttpClient.DefaultRequestHeaders"/>.
        /// </summary>
        private static Dictionary<string, string> RequestHeaders
        {
            set
            {
                _httpClient.DefaultRequestHeaders.Clear();

                if (value != null)
                {
                    foreach (var header in value)
                    {
                        if (header.Value != null && !header.Value.Equals(String.Empty))
                        {
                            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }
            }
        }
    }
}
