using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxiLoadingData;
using TaxiObjects;

namespace TaxiTestNetCore
{
    [TestClass]
    public class TaxisFromSqlite
    {
        [TestMethod]
        public async Task BucarestSampleData()
        {
            var buc = new LoadBucarestTaxis();
            var aut = await buc.TaxiFromPlateSqlite("B30LOB");
            aut.State.ShouldEqual(LicenceState.Valid);
            aut = await buc.TaxiFromPlateSqlite("B30FUV");
            aut.State.ShouldEqual(LicenceState.NotValid);
            aut = await buc.TaxiFromPlateSqlite("B09PVA");
            aut.State.ShouldEqual(LicenceState.ToBeAnalyzed);


        }

        [TestMethod]
        public async Task BucarestSampleDataNoCase()
        {
            var buc = new LoadBucarestTaxis();
            var aut = await buc.TaxiFromPlateSqlite("B30LOB");
            aut?.State.ShouldEqual(LicenceState.Valid);
            var aut1 = await buc.TaxiFromPlateSqlite("b30lob");            
            aut?.State.ShouldEqual(LicenceState.Valid);
            

        }
        [TestMethod]
        public async Task BucarestVerifyImportSqlLite()
        {
            var buc = new LoadBucarestTaxis();
            var taxis = buc.TaxisFromCSV();
            var taxisOK = taxis.Item1;

            var results =await buc.TaxiFromPlateSqliteAll();
            results.RemoveAll(it => string.IsNullOrWhiteSpace(it.NumberAutorization));
            taxisOK.RemoveAll(it => string.IsNullOrWhiteSpace(it.NumberAutorization));
            results.ShouldNotBeEmpty();
            taxisOK.ShouldNotBeEmpty();
            foreach (var res in results)
            {                
                res.ShouldNotBeNull();                
                var dataCSV = taxisOK.FindNumber(res.NumberAutorization);
                dataCSV.ShouldNotBeNull("cannot find autorization :" + res.NumberAutorization);                
                dataCSV.NumberAutorization.ShouldEqual(res.NumberAutorization);
                
                    taxisOK.Remove(dataCSV);
            };
            taxisOK.Count.ShouldEqual(0,"not loaded sqlite:"+string.Join(",",taxisOK.Select(it=>it.NumberAutorization)));
            results.Count.ShouldNotEqual(0);
        }

    }
}