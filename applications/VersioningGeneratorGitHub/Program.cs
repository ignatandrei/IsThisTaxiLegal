using Newtonsoft.Json;
using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using VersioningSummary;

namespace VersioningGeneratorGitHub
{
    class Program
    {
        static void Main(string[] args)
        {
            var Url = @"https://github.com/ignatandrei/IsThisTaxiLegal/";
            var client = new GitHubClient(new ProductHeaderValue("Versioning"));
            var id = client.Repository.Get("ignatandrei", "IsThisTaxiLegal").GetAwaiter().GetResult().Id;
            var jsonFile = new string[]
            {
                "applications/TaxiWebAndAPI/versionTaxiWebAndAPI.json",
                "applications/TaxiLoadingData/versionTaxiLoadingData.json",
                "applications/TaxiObjects/versionTaxiObjects.json",
                "applications/VersioningGeneratorGitHub/versionVersioningGeneratorGitHub.json",
                "applications/VersioningSummary/versionVersioningSummary.json"

            };
            foreach (var pathJSON in jsonFile) {
                var folder = pathJSON.Substring(0, pathJSON.LastIndexOf("/"));
                string fileJSON = pathJSON.Substring(pathJSON.LastIndexOf("/") + 1);
                var requestFolder = new CommitRequest { Path = folder };
                var listFolder = client.Repository.Commit.GetAll(id, requestFolder).GetAwaiter().GetResult().ToList();

                var requestJSON = new CommitRequest { Path = pathJSON };
                var listJSON = client.Repository.Commit.GetAll(id, requestJSON).GetAwaiter().GetResult().ToList();
                var dateJSON = listJSON.Select(it => it.Commit.Committer.Date).Max();



                listFolder.RemoveAll(it => it.Commit.Committer.Date <= dateJSON);

                Console.Write(listFolder.Count);
                string text = "";
                if (listFolder.Count == 0)
                    continue;
                //there is a difference
                var items = listFolder.Select(it => it.Commit).ToArray();
                foreach (var item in items.OrderBy(it => it.Committer.Date))
                {

                    text += item.Message + " from @" + item.Committer.Name;

                }

                //obtain file
                var existingFile = client.Repository.Content.GetAllContents(id, pathJSON).GetAwaiter().GetResult().First().Content;
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

                tmp = Path.Combine(Path.GetTempPath(), fileJSON);
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
