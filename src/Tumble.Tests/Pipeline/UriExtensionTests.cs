using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tumble.Client.Extensions;

namespace Tumble.Tests.Pipeline
{
    [TestClass]
    public class UriExtensionTests
    {
        [TestMethod]
        public void UriAppend_Success()
        {
            // given
            var Uri = new Uri("http://www.host.com/segment1/segment2?param1=value1");
            var addition = "/segment3/segment4";

            //when
            var appendedUri = Uri.Append(addition);

            //then
            Assert.AreEqual("http://www.host.com/segment1/segment2/segment3/segment4?param1=value1", appendedUri.ToString());
        }
    }
}
