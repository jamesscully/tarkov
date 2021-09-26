using Microsoft.VisualStudio.TestTools.UnitTesting;
using TarkovAssistantWPF.data;

namespace TarkovAssistantTests
{
    [TestClass]
    public class QuestDataTest
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
        public void TestAllQuests()
        {
            QuestData data = QuestData.GetInstance();
            
            data.PrintAll();
        }
    }
}