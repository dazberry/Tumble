using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tumble.Client
{
    public class HttpResponseBuilder
    {

        private HttpResponseMessage _httpResponseMessage;

        private HttpResponseBuilder(HttpResponseMessage httpResponseMessage)
        {
            _httpResponseMessage = new HttpResponseMessage();
        }

        public static HttpResponseBuilder HttpResponseMessage() =>
            new HttpResponseBuilder(new HttpResponseMessage());

        public static HttpResponseBuilder HttpResponseMessage(HttpResponseMessage httpResponseMessage) =>
            new HttpResponseBuilder(httpResponseMessage);

        public HttpResponseBuilder WithContent(HttpContent httpContent)
        {
            _httpResponseMessage.Content = httpContent;
            return this;
        }

        public HttpResponseBuilder WithNoContent()
        {
            _httpResponseMessage.Content = null;
            return this;
        }

        public HttpResponseBuilder WithStringContent(string value)
        {
            _httpResponseMessage.Content = new StringContent(value);
            return this;
        }

        public HttpResponseBuilder WithStatusCode(int statusCode)
        {
            _httpResponseMessage.StatusCode = (HttpStatusCode)statusCode;
            return this;
        }

        public HttpResponseBuilder WithHeaders(Action<HttpResponseHeaders> headerAction)
        {
            headerAction?.Invoke(_httpResponseMessage.Headers);
            return this;
        }

        public HttpResponseBuilder WithReasonPhrase(string reasonPhrase)
        {
            _httpResponseMessage.ReasonPhrase = reasonPhrase;
            return this;
        }

        public HttpResponseBuilder WithVersion(Version version)
        {
            _httpResponseMessage.Version = version;
            return this;
        }

        public HttpResponseMessage Build() =>
            _httpResponseMessage;
        
    }
}
