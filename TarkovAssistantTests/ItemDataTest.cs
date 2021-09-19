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
        }
    }
}