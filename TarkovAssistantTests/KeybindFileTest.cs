using System.Diagnostics;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TarkovAssistantWPF.enums;
using TarkovAssistantWPF.keybinding;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TarkovAssistantTests
{
    [TestClass]
    public class KeybindFileTest
    {
        private KeybindManager binds;


        [TestInitialize]
        public void Setup()
        {
            Debug.WriteLine("---- START OF TEST ----");
            binds = KeybindManager.GetInstance(true);
        }


        [TestCleanup]
        public void Cleanup()
        {
            binds.Debug_WriteBindsDictionary();
            Debug.WriteLine("----  END OF TEST  ----");
            binds.ResetToDefault();
        }


        // Test the default config 
        [TestMethod]
        public void TestDefaultConfig()
        {
            var returnedKey = binds.GetKeybindForKey(Key.R);

            Assert.AreEqual(returnedKey, Keybind.Reset);

            returnedKey = binds.GetKeybindForKey(Key.NumPad9);

            Assert.AreEqual(returnedKey, Keybind.CycleSubMap);
        }


        [TestMethod]
        public void TestAddKeybind()
        {
            binds.SetKeybind(Keybind.CycleSubMap, Key.A);
            binds.SaveToFile();
            binds.Reload();


            var cycleKey = binds.GetKeybindForKey(Key.A);

            binds.Debug_WriteBindsDictionary();

            Debug.WriteLine($"Testing {cycleKey} is == ");
            Assert.IsTrue(cycleKey == Keybind.CycleSubMap);
        }

        [TestMethod]
        public void TestClearingByKey()
        {
            // Test clearing by Key
            binds.SetKeybind(Keybind.NextMap, Key.A);
            binds.ClearKey(Key.A);

            var hotkey = binds.GetKeybindForKey(Key.A);

            Assert.IsNull(hotkey);
        }

        [TestMethod]
        public void TestClearingByHotkey()
        {
            // Test clearing by Keybind
            binds.Debug_WriteBindsDictionary();

            Debug.WriteLine("Test: Setting bind for NEXTMAP to A");
            binds.SetKeybind(Keybind.NextMap, Key.A);

            Debug.WriteLine("Test: Saving bind for NEXTMAP to A");
            binds.SaveToFile();

            binds.Debug_WriteBindsDictionary();

            Debug.WriteLine("Test: Clearing bind for NEXTMAP to A");
            binds.ClearKeybind(Keybind.NextMap);

            binds.Debug_WriteBindsDictionary();


            var hotkey = binds.GetKeybindForKey(Key.A);

            Debug.WriteLine("Found key: " + hotkey);
            Assert.IsNull(hotkey);
        }
    }
}
