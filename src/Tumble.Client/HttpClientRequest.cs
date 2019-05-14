using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Tumble.Client
{
    public class HttpClientRequest
    {
        private static HttpClient _httpClient = new HttpClient();
        private readonly HttpRequestMessage _httpRequestMessage;
               
        private HttpClientRequest() : this(new HttpRequestMessage())
        {            
        }

        public HttpClientRequest(HttpRequestMessage httpRequestMessage) =>
            _httpRequestMessage = httpRequestMessage;

        private HttpClientRequest WithRequestMessage(Action<HttpRequestMessage> action)
        {
            action?.Invoke(_httpRequestMessage);
            return this;
        }

        public static HttpClientRequest WithMethod(HttpMethod httpMethod) =>
            new HttpClientRequest().WithRequestMessage(msg => msg.Method = httpMethod);

        public static HttpClientRequest Get() =>
            WithMethod(HttpMethod.Get);

        public static HttpClientRequest Post() =>
            WithMethod(HttpMethod.Post);

        public static HttpClientRequest Put() =>
            WithMethod(HttpMethod.Put);

        public static HttpClientRequest Delete() =>
            WithMethod(HttpMethod.Delete);

        public HttpClientRequest Uri(Uri uri) =>
            WithRequestMessage(msg => msg.RequestUri = uri);

        public HttpClientRequest Headers(Action<HttpRequestHeaders> headerAction) =>
            WithRequestMessage(msg => headerAction.Invoke(msg.Headers));

        public HttpClientRequest Content(HttpContent httpContent) =>
            WithRequestMessage(msg => msg.Content = httpContent);

        public HttpClientRequest Content(Func<HttpContent> contentAction) =>
            WithRequestMessage(msg => msg.Content = contentAction.Invoke());

        public HttpClientRequest Request(Action<HttpRequestMessage> requestAction) =>
            WithRequestMessage(msg => requestAction.Invoke(msg));
        
        public async Task<HttpResponseMessage> InvokeAsync(CancellationToken cancellationToken = default(CancellationToken)) =>                   
            await _httpClient.SendAsync(_httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
    }
}
