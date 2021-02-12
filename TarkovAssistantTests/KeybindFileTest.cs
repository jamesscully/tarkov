using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using TarkovAssistantWPF.enums;
using TarkovAssistantWPF.keybinding;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TarkovAssistantTests
{
    [TestClass]
    public class KeybindFileTest
    {
        private JsonKeybinds binds;


        [TestInitialize]
        public void Setup()
        {
            Debug.WriteLine("---- START OF TEST ----");
            binds = JsonKeybinds.GetInstance(true);
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
            var returnedKey = binds.GetHotkeyForBind(Key.R);

            Assert.AreEqual(returnedKey, Keybind.Reset);

            returnedKey = binds.GetHotkeyForBind(Key.NumPad9);

            Assert.AreEqual(returnedKey, Keybind.CycleSubMap);
        }


        [TestMethod]
        public void TestAddKeybind()
        {
            binds.SetBind(Keybind.CycleSubMap, Key.A);
            binds.SaveBinds();
            binds.Reload();


            var cycleKey = binds.GetHotkeyForBind(Key.A);

            binds.Debug_WriteBindsDictionary();

            Debug.WriteLine($"Testing {cycleKey} is == ");
            Assert.IsTrue(cycleKey == Keybind.CycleSubMap);
        }

        [TestMethod]
        public void TestClearingByKey()
        {
            // Test clearing by Key
            binds.SetBind(Keybind.NextMap, Key.A);
            binds.ClearBind(Key.A);

            var hotkey = binds.GetHotkeyForBind(Key.A);

            Assert.IsNull(hotkey);
        }

        [TestMethod]
        public void TestClearingByHotkey()
        {
            // Test clearing by Keybind
            binds.Debug_WriteBindsDictionary();

            Debug.WriteLine("Test: Setting bind for NEXTMAP to A");
            binds.SetBind(Keybind.NextMap, Key.A);

            Debug.WriteLine("Test: Saving bind for NEXTMAP to A");
            binds.SaveBinds();

            binds.Debug_WriteBindsDictionary();

            Debug.WriteLine("Test: Clearing bind for NEXTMAP to A");
            binds.ClearBind(Keybind.NextMap);

            binds.Debug_WriteBindsDictionary();


            var hotkey = binds.GetHotkeyForBind(Key.A);

            Debug.WriteLine("Found key: " + hotkey);
            Assert.IsNull(hotkey);
        }
    }
}
