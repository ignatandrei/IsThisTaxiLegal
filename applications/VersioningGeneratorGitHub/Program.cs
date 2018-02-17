using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VersioningSummary;

namespace VersioningGeneratorGitHub
{
    class Program
    {
        static void Main(string[] args)
        {
            MainTask(args).GetAwaiter();
        }
        static async Task MainTask(string[] args)
        {
            //var Url = @"https://github.com/ignatandrei/IsThisTaxiLegal/";
            //var client = new GitHubClient(new ProductHeaderValue("Versioning"));
            //var id = client.Repository.Get("ignatandrei", "IsThisTaxiLegal").GetAwaiter().GetResult().Id;
            var jsonFile = new string[]
            {
                "applications/TaxiWebAndAPI/versionTaxiWebAndAPI.json",
                "applications/TaxiLoadingData/versionTaxiLoadingData.json",
                "applications/TaxiObjects/versionTaxiObjects.json",
                "applications/VersioningGeneratorGitHub/versionVersioningGeneratorGitHub.json",
                "applications/VersioningSummary/versionVersioningSummary.json"

            };
            var length = jsonFile.Length;
            var tasks = new Task[length];
            var arr = new GitVersionSourceControlFileFolder[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = new GitVersionSourceControlFileFolder("ignatandrei", "IsThisTaxiLegal", jsonFile[i]);
                tasks[i]=arr[i].Init();
            }
            await Task.WhenAll(tasks);
            for (int i = 0; i < length; i++)
            {
                if (!arr[i].HasModifications())
                    continue;


            
                
                string text = "";
                //there is a difference
                foreach (var item in arr[i].FolderCommits.OrderBy(it => it.CommitedDate))
                {

                    text += item.Message + " from @" + item.Committer;

                }

                //obtain file
                var existingFile = arr[i].File;
                var tmp = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".txt");
                File.WriteAllText(tmp, existingFile);

                var vc = new VersionComponents();
                var versions = vc.LoadFromFile(tmp).ToList();
                File.Delete(tmp);
                var latestVersion = versions.OrderByDescending(it => it.Version).First();
                var newVersion = new VersionDll();
                newVersion.Name = latestVersion.Name;
                newVersion.DateRelease = DateTime.Now;
                newVersion.ReleaseNotes = text;
                newVersion.Version = new Version(latestVersion.Version.Major, latestVersion.Version.Minor, latestVersion.Version.Build, latestVersion.Version.Revision + 1);
                versions.Insert(0, newVersion);

                tmp = Path.Combine(Path.GetTempPath(), jsonFile[i]);
                var vals = JsonConvert.SerializeObject(versions.ToArray());

                File.WriteAllText(tmp, vals);
                Console.WriteLine(tmp);
                Thread.Sleep(1000);
                var startInfo = new ProcessStartInfo("notepad.exe");
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.Arguments = tmp;
                Process.Start(startInfo);
                
            }
        }
    }
}
