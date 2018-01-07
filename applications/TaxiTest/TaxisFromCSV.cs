using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using System;
using TaxiLoadingData;

namespace TaxiTest
{
    [TestClass]
    public class TaxisFromCSV
    {
        [TestMethod]
        public void Bucarest()
        {
            var buc = new LoadBucarestTaxis();
            var taxis=buc.TaxisFromCSV();
            taxis.ShouldNotBeNull();
            taxis.Item1.Count.ShouldBeGreaterThan(0);
            var errorLines = taxis.Item2;
            //only error should be header
            errorLines.Length.ShouldEqual(1,string.Join(Environment.NewLine,errorLines));

        }
    }

}
