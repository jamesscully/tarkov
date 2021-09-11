using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TarkovAssistantWPF.data;

namespace TarkovAssistantTests
{
    [TestClass]
    public class AmmoDataTest
    {
        
        
        [TestInitialize]
        public void Setup()
        {
            
        }


        [TestCleanup]
        public void Cleanup()
        {

        }


        // Test the default config 
        [TestMethod]
        public void TestGetAllCalibers()
        {
            AmmoData data = AmmoData.GetInstance();

            foreach (string s in data.GetAllCalibers())
            {
                Debug.WriteLine(s);
            }
        }

        [TestMethod]
        public void TestFindByCaliber()
        {
            AmmoData data = AmmoData.GetInstance();

            var list = data.GetAmmoByCaliber("Caliber57x28");

            foreach (var bullet in list)
            {
                Debug.WriteLine("Finding bullets");
                Debug.WriteLine(bullet.name);
            }
            
        }
    }
}