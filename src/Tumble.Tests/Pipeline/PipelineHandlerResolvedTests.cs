using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Tests.Pipeline
{
    [TestClass]
    public class PipelineHandlerResolverTests
    {  
        public interface IIntValue
        {
            int Value { get; set; }
        }

        private class FlatContext : IPipelineHandlerContextResolver, IIntValue
        {
            public int Value1 { get; set; }
            public int Value2 { get; set; }
            public int Value { get; set; }

            public object Resolve<THandler, TContext, TResolvedContext>(THandler handler, TContext context, int index)
            {
                switch (index)
                {
                    case 0: return Value1;
                    case 1: return Value2;
                    default:
                        return context;
                }
            }                            
        }

        private class NestedContext : IPipelineHandlerContextResolver, IIntValue
        {
            private class IntValue : IIntValue
            {
                public int Value { get; set; }
            }

            public IIntValue IntValue1 { get; set; } = new IntValue();
            public IIntValue IntValue2 { get; set; } = new IntValue();            
            public int Value { get; set; }

            public object Resolve<THandler, TContext, TResolvedContext>(THandler handler, TContext context, int index)
            {                
                switch (index)
                {
                    case 0: return IntValue1;
                    case 1: return IntValue2;
                    default:
                        return context;
                }             
            }
        }

        private class AdditionHandler : IPipelineHandler<int, int, IIntValue>
        {
            public async Task InvokeAsync(PipelineDelegate next, int value1, int value2, IIntValue context)
            {
                context.Value = value1 + value2;
                await next.Invoke();
            }
        }

        private class AdditionHandler2 : IPipelineHandler<IIntValue, IIntValue, IIntValue>
        {
            public async Task InvokeAsync(PipelineDelegate next, IIntValue context, IIntValue context1, IIntValue context2)
            {
                context2.Value = context.Value + context1.Value;
                await next.Invoke();
            }
        }

        private Random _rnd = new Random();
        
        [TestMethod]
        public async Task FlatContextResolverInvoke()
        {
            var context = new FlatContext()
            {
                Value1 = _rnd.Next(),
                Value2 = _rnd.Next()
            };

            PipelineRequest request = new PipelineRequest();
            var result = await request.AddHandler<AdditionHandler>()                
                .InvokeAsync(context);            

            Assert.AreEqual(result, context);
            Assert.AreEqual(context.Value, context.Value1 + context.Value2);            
        }


        [TestMethod]
        public async Task NextedContextResolverInvoke()
        {            
            PipelineRequest request = new PipelineRequest();
            var context = await request.AddHandler<AdditionHandler2>()
                .InvokeAsync(new NestedContext(),
                    ctx => 
                    {
                        ctx.IntValue1.Value = _rnd.Next();
                        ctx.IntValue2.Value = _rnd.Next();
                    });            
            
            Assert.AreEqual(context.Value, context.IntValue1.Value + context.IntValue2.Value);
        }

    }
}
