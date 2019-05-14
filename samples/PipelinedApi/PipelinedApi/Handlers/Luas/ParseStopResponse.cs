﻿using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PipelinedApi.Models;
using Tumble.Core;

namespace PipelinedApi.Handlers.Luas
{
    public class ParseStopResponse : IPipelineHandler        
    {
        public async Task InvokeAsync(IPipelineContext context, PipelineDelegate next)
        {
            if (context.Get(out HttpResponseMessage responseMessage))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    //var response = await responseMessage.Content.ReadAsStringAsync();
                    XmlSerializer serializer = new XmlSerializer(typeof(LuasStop));
                    var response = serializer.Deserialize(await responseMessage.Content.ReadAsStreamAsync());
                    context.Set((LuasStop)response);
                }
            }
            //else
            //    throw new PipelineDependencyException<HttpResponseMessage>(this);
        }
    }
}
