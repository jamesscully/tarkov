using Microsoft.VisualStudio.TestTools.UnitTesting;
using TarkovAssistantWPF.data;

namespace TarkovAssistantTests
{
    [TestClass]
    public class HideoutDataTest
    {

        [TestMethod]
        public void HideoutPrintAll()
        {
            HideoutData data = HideoutData.GetInstance();
            
            data.PrintAll();
        }
    }
}