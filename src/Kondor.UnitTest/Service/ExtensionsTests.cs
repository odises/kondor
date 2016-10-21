using System;
using Kondor.Service.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kondor.UnitTest.Service
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void Does_GetRounded_Work_Fine()
        {
            var x1 = new DateTime(2016, 10, 21, 23, 20, 13).GetRounded();
            var x1Expected = new DateTime(2016, 10, 21, 23, 0, 0);

            Assert.AreEqual(x1, x1Expected);

            var x2 = new DateTime(2016, 10, 21, 23, 31, 18).GetRounded();
            var x2Expected = new DateTime(2016, 10, 22, 0, 0, 0);

            Assert.AreEqual(x2, x2Expected);
        }
    }
}
