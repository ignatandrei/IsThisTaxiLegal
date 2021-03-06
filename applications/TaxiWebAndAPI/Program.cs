﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaxiLoadingData;
using TaxiObjects;

namespace TaxiWebAndAPI
{
    public class Program
    {
        //internal static TaxiAutorizations BucarestTaxis; 
        public static void Main(string[] args)
        {

            if (!File.Exists("taxis.sqlite3"))
            {
                var buc = new LoadBucarestTaxis();
                buc.DownloadDatabaseSqlLite().GetAwaiter().GetResult();
            }
            //var buc = new LoadBucarestTaxis();
            //BucarestTaxis = buc.TaxisFromCSV().Item1;





            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                
                .UseStartup<Startup>()
                .Build();
    }
}
