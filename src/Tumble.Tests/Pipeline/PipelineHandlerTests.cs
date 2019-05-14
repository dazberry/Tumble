using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Tests.Pipeline
{
    [TestClass]
    public class PipelineHandlerTests
    {
        public interface IResultValue
        {
            int Result { get; set; }
        }

        public interface IIntValue
        {
            int Value { get; set; }
        }

        public interface IIntValue2
        {
            int Value2 { get; set; }
        }

        public interface IIntValue3
        {
            int Value3 { get; set; }
        }

        public class TestValues : IResultValue, IIntValue, IIntValue2, IIntValue3
        {
            public int Result { get; set; }

            public int Value { get; set; }
            public int Value2 { get; set; }
            public int Value3 { get; set; }
        }

        private class SingleParamInvokeHandler : IPipelineHandler<IResultValue>
        {            
            public int ValueToSet { get; set; }

            public async Task InvokeAsync(PipelineDelegate next, IResultValue context)
            {
                context.Result = ValueToSet;
                await next.Invoke();
            }
        }

        private class TwoParameterInvokeHandler : IPipelineHandler<IIntValue, IResultValue>
        {
            public async Task InvokeAsync(PipelineDelegate next, IIntValue context, IResultValue context1)
            {
                context1.Result = context.Value;
                await next.Invoke();
            }
        }

        private class ThreeParameterInvokeHandler : IPipelineHandler<IIntValue, IIntValue2, IResultValue>
        {
            public async Task InvokeAsync(PipelineDelegate next, IIntValue context, IIntValue2 context1, IResultValue context2)
            {
                context2.Result = context.Value + context1.Value2;
                await next.Invoke();
            }
        }


        private class FourParameterInvokeHandler : IPipelineHandler<IIntValue, IIntValue2, IIntValue3, IResultValue>
        {
            public async Task InvokeAsync(PipelineDelegate next, IIntValue context, IIntValue2 context1, IIntValue3 context2, IResultValue context3)
            {
                context3.Result = context.Value + context1.Value2 + context2.Value3;
                await next.Invoke();
            }
        }


        private Random _rnd = new Random();
        
        [TestMethod]
        public async Task SingleHandlerInvoke()
        {
            var initialValue = _rnd.Next();
            var testValues = new TestValues();

            PipelineRequest request = new PipelineRequest();
            var result = await request.AddHandler<SingleParamInvokeHandler>(
                handler => handler.ValueToSet = initialValue)
                .InvokeAsync(testValues);

            Assert.AreEqual(result, testValues);
            Assert.AreEqual(testValues.Result, initialValue);            
        }


        [TestMethod]
        public async Task TwoParamHandlerInvoke()
        {            
            var testValues = new TestValues()
            {
                Value = _rnd.Next()
            };

            PipelineRequest request = new PipelineRequest();
            var result = await request.AddHandler<TwoParameterInvokeHandler>()
                .InvokeAsync(testValues);

            Assert.AreEqual(result, testValues);
            Assert.AreEqual(testValues.Value, testValues.Result);
        }


        [TestMethod]
        public async Task ThreeParamHandlerInvoke()
        {
            var testValues = new TestValues()
            {
                Value = _rnd.Next(),
                Value2 = _rnd.Next()
            };

            PipelineRequest request = new PipelineRequest();
            var result = await request.AddHandler<ThreeParameterInvokeHandler>()
                .InvokeAsync(testValues);

            Assert.AreEqual(result, testValues);
            Assert.AreEqual(testValues.Value + testValues.Value2, testValues.Result);
        }

        [TestMethod]
        public async Task FourParamHandlerInvoke()
        {
            var testValues = new TestValues()
            {
                Value = _rnd.Next(),
                Value2 = _rnd.Next(),
                Value3 = _rnd.Next()
            };

            PipelineRequest request = new PipelineRequest();
            var result = await request.AddHandler<FourParameterInvokeHandler>()
                .InvokeAsync(testValues);

            Assert.AreEqual(result, testValues);
            Assert.AreEqual(testValues.Value + testValues.Value2 + testValues.Value3, testValues.Result);
        }


    }
}
