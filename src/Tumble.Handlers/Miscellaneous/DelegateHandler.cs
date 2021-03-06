﻿using System;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Miscellaneous
{
    public class DelegateHandler : IPipelineHandler
    {
        public Action<PipelineContext> BeforeNextInvokeAction { get; set; }
        public Action<PipelineContext> AfterNextInvokeAction { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            try
            {
                BeforeNextInvokeAction?.Invoke(context);
                await next.Invoke();
            }
            finally
            {
                AfterNextInvokeAction?.Invoke(context);
            }
        }
    }

    public static class PipelineRequestExtension
    {
        public static PipelineRequest AddDelegateHandler(
            this PipelineRequest pipelineRequest,
            Action<PipelineContext> beforeNextInvokeAction,
            Action<PipelineContext> afterNextInvokeAction = null)
        {
            pipelineRequest.AddHandler(
                new DelegateHandler()
                {
                    BeforeNextInvokeAction = beforeNextInvokeAction,
                    AfterNextInvokeAction = afterNextInvokeAction
                });
            return pipelineRequest;
        }
            
    }
}
