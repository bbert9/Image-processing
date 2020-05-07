using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjetTraitementImage_BLANQUE_BERTRAND;

namespace Test_unitaires
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            //Directory.SetCurrentDirectory(@"\\Mac\Home\Desktop\COURS\informatique\ProjetTraitementImage_BLANQUE_BERTRAND\ProjetTraitementImage_BLANQUE_BERTRAND\bin\Debug");
            MyImage test = new MyImage("TEST001.bmp");
            string actual = test.int_to_8bit(255);
            //j'ai utilisé un convertisseur sur le web pour trouver ce binaire.
            string expected = "1111";
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TesTMethod2()
        {
            //Directory.SetCurrentDirectory(@"\\Mac\Home\Desktop\COURS\informatique\ProjetTraitementImage_BLANQUE_BERTRAND\ProjetTraitementImage_BLANQUE_BERTRAND\bin\Debug");
            MyImage test = new MyImage("TEST001.bmp");
            string actual = test.int_to_8bit_tot(255);
            string expected = "11111111";
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TesTMethod3()
        {
            //Directory.SetCurrentDirectory(@"\\Mac\Home\Desktop\COURS\informatique\ProjetTraitementImage_BLANQUE_BERTRAND\ProjetTraitementImage_BLANQUE_BERTRAND\bin\Debug");
            MyImage test = new MyImage("TEST001.bmp");
            int actual = test.bit_to_int("11111111");
            int expected = 255;
            Assert.AreEqual(expected, actual);
        }
    }
}
