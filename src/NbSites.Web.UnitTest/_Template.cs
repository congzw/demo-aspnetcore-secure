using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    //[TestClass]
    public class Template
    {
        [TestMethod]
        public void StringCompare_Should_Ok()
        {
            "A".ShouldEqual("A");
        }
    }
}
