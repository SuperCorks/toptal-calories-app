using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Calories.App.Services
{
    /// <summary>
    /// This interface is used to abstract the http client component.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Performs an HTTP GET request.
        /// </summary>
        /// 
        /// <param name="uri">The request's URI (scheme + host + url).</param>
        /// <param name="headers">Http headers to overwrite. Keys are header components and values are header values.</param>
        /// <param name="ct">The request's cancellation token.</param>
        /// 
        /// <returns>The pending request's response in a Task.</returns>
        Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> headers = null, CancellationToken? ct = null);

        /// <summary>
        /// TODO
        /// </summary>
        /// 
        /// <param name="uri">The request's URI (scheme + host + url).</param>
        /// <param name="headers">Http headers to overwrite. Keys are header components and values are header values.</param>
        /// <param name="ct">The request's cancellation token.</param>
        /// 
        /// <returns>The pending request's response in a Task.</returns>
        Task<HttpResponseMessage> GetFile(string uri, Dictionary<string, string> headers, CancellationToken? ct);

        /// <summary>
        /// Performs an HTTP POST request.
        /// </summary>
        /// 
        /// <param name="uri">The request's URI (scheme + host + url).</param>
        /// <param name="body">The request's body as a data transfer object.</param>
        /// <param name="headers">Http headers to overwrite. Keys are header components and values are header values.</param>
        /// <param name="ct">The request's cancellation token.</param>
        /// 
        /// <returns>The pending request's response in a Task.</returns>
        Task<HttpResponseMessage> Post(string uri, object body, Dictionary<string, string> headers = null, CancellationToken? ct = null);

        /// <summary>
        /// TODO
        /// </summary>
        Task<HttpResponseMessage> PostFile(string uri, Stream fileStream, Dictionary<string, string> headers, CancellationToken? ct);

        /// <summary>
        /// Performs an HTTP GET request.
        /// </summary>
        /// 
        /// <param name="uri">The request's URI (scheme + host + url).</param>
        /// <param name="headers">Http headers to overwrite. Keys are header components and values are header values.</param>
        /// <param name="ct">The request's cancellation token.</param>
        /// 
        /// <returns>The pending request's response in a Task.</returns>
        Task<HttpResponseMessage> Delete(string uri, Dictionary<string, string> headers = null, CancellationToken? ct = null);
    }
}
