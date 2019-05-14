using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tumble.Core;

namespace Tumble.Tests
{
    [TestClass]
    public class PipelineContextTests
    {
        //rivate PipelineContext _ctx;

        [TestInitialize]
        public void Init()
        {
            //_ctx = new PipelineContext();
            //_ctx.Set(1);
            //_ctx.Set(2);
            //_ctx.Set(3);
            //_ctx.Set("4");
        }

        [TestMethod]
        public void Context_Add()
        {
            //given
            //_ctx.Set(5);

            //then
            //Assert.AreEqual(5, _ctx.Count);
            //Assert.AreEqual(4, _ctx.Get<int>().Count());
            //Assert.AreEqual(1, _ctx.Get<string>().Count());

            //CollectionAssert.AreEquivalent(
            //    new[] { 1, 2, 3, 5 },
            //    _ctx.Get<int>().ToArray());

            //CollectionAssert.AreEquivalent(
            //    new[] { "4" },
            //    _ctx.Get<string>().ToArray());
        }

        [TestMethod]
        public void Context_Remove_ByType()
        {
            //given
            //when
            //_ctx.Remove<int>();

            ////then
            //Assert.AreEqual(1, _ctx.Count);
            //Assert.AreEqual(0, _ctx.Get<int>().Count());
        }

        [TestMethod]
        public void Context_Remove_Item()
        {
            //given
            //when
            //_ctx.Remove<int>(x => x == 1);

            ////then
            //CollectionAssert.AreEquivalent(
            //    new[] { 2, 3 },
            //    _ctx.Get<int>().ToArray());
        }

        [TestMethod]
        public void Context_Remote_Item2()
        {
            //given
            //when
            //_ctx.Remove(1);

            ////then
            //CollectionAssert.AreEquivalent(
            //    new[] { 2, 3 },
            //    _ctx.Get<int>().ToArray());
        }
    }
}
