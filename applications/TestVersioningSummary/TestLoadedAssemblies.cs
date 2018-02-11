using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VersioningSummary;

namespace TestVersioningSummary
{
    [TestClass]
    public class TestLoadedAssemblies
    {
        [TestMethod]
        public void TestThisAssembly()
        {
            var vc = new VersionComponents();
            var data=vc.LoadCurrentDir();
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
            Assert.AreEqual(2,data.Count(it => it.Name.Contains("VersioningSummary")) );
        }
        //[TestMethod]
        //public void TestSerialize()
        //{
        //    var list = new List<VersionDll>();
        //    list.Add(new VersionDll() { DateRelease = DateTime.Now, Name = "andrei.dll", ReleaseNotes = "SDAD0", Version = new Version(1, 1, 0, 1) });
        //    var x = JsonConvert.SerializeObject(list.ToArray());
        //    var v = JsonConvert.DeserializeObject<VersionDll[]>(x);
        //    Console.WriteLine(x);


        //}
    }
}
