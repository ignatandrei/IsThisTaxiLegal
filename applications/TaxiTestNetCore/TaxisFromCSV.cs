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
        public void BucarestAll()
        {
            var buc = new LoadBucarestTaxis();
            var taxis = buc.TaxisFromCSV();
            taxis.ShouldNotBeNull();
            var errorLines = taxis.Item2;
            //only error should be header
            errorLines.Length.ShouldEqual(1, string.Join(Environment.NewLine, errorLines));


            var taxisOK = taxis.Item1;
            taxisOK.Count.ShouldBeGreaterThan(0);
            var valid = taxisOK.Where(it => it.State == LicenceState.Valid);
            valid.Any().ShouldBeTrue();
            valid.Select(it => it.CarLicensed.PlateNumber).ShouldContain("B30LOB");

            var notvalid = taxisOK.Where(it => it.State == LicenceState.NotValid);
            notvalid.Any().ShouldBeTrue();
            notvalid.Select(it => it.CarLicensed.PlateNumber).ShouldContain("B30FUV");

            var analyze = taxisOK.Where(it => it.State == LicenceState.ToBeAnalyzed);
            analyze.Any().ShouldBeTrue();
            analyze.Select(it => it.CarLicensed.PlateNumber).ShouldContain("B09PVA");

            taxisOK.Where(it => it.State == LicenceState.Unknown).Count().ShouldBeLessThan(100);



        }
    }
}
