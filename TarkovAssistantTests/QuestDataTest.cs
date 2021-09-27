using System;
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
            
            Console.WriteLine("No offence REQUIREMENTS: " + data.GetById(199) );

            // Assert.IsTrue(data.GetById(1).require[0].level == 1);
        }
    }
}