using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using System;
using System.Collections.Generic;
using System.Text;
using TaxiObjects;

namespace TaxiTestNetCore
{
    [TestClass]
    public class CityFromName
    {
        [TestMethod]
        public void UniqueCity()
        {
            string Bucarest = "Bucarest";
            var city1 = City.FromName(Bucarest);
            var city2 = City.FromName(Bucarest);
            city1.ShouldNotBeNull();
            city2.ShouldNotBeNull();
            city1.Name.ShouldBeSameAs(Bucarest);
            city2.Name.ShouldBeSameAs(Bucarest);
            city1.ShouldBeSameAs(city2);
            Assert.AreEqual(city1, city2);
        }
    }
}
