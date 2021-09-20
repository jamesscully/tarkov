using Microsoft.VisualStudio.TestTools.UnitTesting;
using TarkovAssistantWPF.data;

namespace TarkovAssistantTests
{
    [TestClass]
    public class ItemDataTest
    {
        [TestInitialize]
        public void setup()
        {
            
        }

        [TestCleanup]
        public void cleanup()
        {
            
        }

        [TestMethod]
        public void TestAllItems()
        {
            ItemData data = ItemData.GetInstance();
            
            data.PrintAll();

            Assert.IsTrue(data.GetById("5d6e6a42a4b9364f07165f52").shortName == "Poleva-6u");
            Assert.IsTrue(data.GetById("5447ac644bdc2d6c208b4567").shortName == "M855");
            Assert.IsTrue(data.GetById("545cdae64bdc2d39198b4568").shortName == "Tri-Zip");
            Assert.IsTrue(data.GetById("560835c74bdc2dc8488b456f").name == "MP-133 12ga 510mm barrel with rib");
        }
    }
}