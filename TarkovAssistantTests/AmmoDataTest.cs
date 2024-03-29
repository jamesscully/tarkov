﻿using System;
using System.Collections.Generic;
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
                Console.WriteLine(s);
            }
        }

        [TestMethod]
        public void TestFindByCaliber()
        {
            AmmoData data = AmmoData.GetInstance();

            var list = data.GetAmmoByCaliber("Caliber57x28");
            List<string> strList = new List<string>(); 

            foreach (var bullet in list)
            {
                Console.WriteLine("Finding bullets");
                Console.WriteLine(bullet.name);
                
                strList.Add(bullet.name);
            }
            
            Assert.IsTrue(strList.Contains("5.7x28mm L191"));
            Assert.IsTrue(strList.Contains("5.7x28mm SS198LF"));
            Assert.IsTrue(strList.Contains("5.7x28mm SS197SR"));
            Assert.IsTrue(strList.Contains("5.7x28mm SS190"));
            
        }
    }
}