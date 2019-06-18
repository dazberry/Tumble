using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Handlers;
using PipelinedApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client.Contexts;
using Tumble.Client.Parameters;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Contexts
{
    public class DublinBikesContext<TApiResponse> : 
        IPipelineHandlerContextResolver,
        IQueryParameters,
        IHttpResponseMessageContext,
        IObjectResultContext
    {
        public Uri Uri { get; set; }

        private IQueryParameters queryParameters = new QueryParameters();

        public HttpResponseMessage HttpResponseMessage { get; set; }

        public TApiResponse ApiResponse { get; set; }

        public ObjectResult ObjectResult { get; set; }

        private bool Is<T>(Type type) =>
            typeof(T) == type;

        public IQueryParameters Add<T>(string name, T value, bool optional = false) =>
            queryParameters.Add(name, value, optional);

        public IEnumerable<QueryParameter> Get() =>
            queryParameters.Get();


        public object Resolve<THandler, TContext, TResolvedContext>(THandler handler, TContext context, int index)
        {
            if (Is<Uri>(typeof(TResolvedContext)))
                return Uri;

            if (Is<IContextResolver<Uri>>(typeof(TResolvedContext)))
                return new ContextResolver<Uri>(
                    () => Uri, x => Uri = x);

            if (Is<HttpResponseMessage>(typeof(TResolvedContext)))
                return HttpResponseMessage;

            if (Is<TApiResponse>(typeof(TResolvedContext)))
                return ApiResponse;

            if (Is<IContextResolver<TApiResponse>>(typeof(TResolvedContext)))
                return new ContextResolver<TApiResponse>(
                    () => ApiResponse, x => ApiResponse = x);

            return this;
        }
    }
}
