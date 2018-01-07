using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using System;
using System.Linq;
using TaxiLoadingData;
using TaxiObjects;

namespace TaxiTestNetCore
{
    [TestClass]
    public class TaxisFromCSV
    {
        [TestMethod]
        public void Bucarest()
        {
            var buc = new LoadBucarestTaxis();
            var taxis = buc.TaxisFromCSV();
            taxis.ShouldNotBeNull();
            var errorLines = taxis.Item2;
            //only error should be header
            errorLines.Length.ShouldEqual(1, string.Join(Environment.NewLine, errorLines));


            var taxisOK = taxis.Item1;
            taxisOK.Count.ShouldBeGreaterThan(0);
            taxisOK.Where(it => it.State == LicenceState.Valid).Count().ShouldBeGreaterThan(0);
            taxisOK.Where(it => it.State == LicenceState.NotValid).Count().ShouldBeGreaterThan(0);
            taxisOK.Where(it => it.State == LicenceState.ToBeAnalyzed).Count().ShouldBeGreaterThan(0);
            taxisOK.Where(it => it.State == LicenceState.Unknown).Count().ShouldBeLessThan(100);



        }
    }
}
