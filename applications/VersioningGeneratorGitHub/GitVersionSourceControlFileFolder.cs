using Octokit;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VersioningSourceControl;

namespace VersioningGeneratorGitHub
{
    class GitVersionSourceControlFileFolder : VersionSourceControlFileFolder
    {
        static ConcurrentDictionary<string, long> repoId=new ConcurrentDictionary<string, long>();
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public GitVersionSourceControlFileFolder(string owner, string repositoryName,string pathFile):base(pathFile)
        {
            Owner = owner;
            RepositoryName = repositoryName;
        }

        public string Owner { get; }
        public string RepositoryName { get; }

        GitHubClient client;
        long idRepository;
        Func<GitHubCommit, CommitSourceControl> translate =
            (g) =>
            new CommitSourceControl()
            {
                CommitedDate = g.Commit.Committer.Date,
                Committer = g.Commit.Committer.Name,
                Message = g.Commit.Message,
                Id=g.Sha
            };
        public override async Task FindFromSourceControl()
        {
            var folder = PathFile.Substring(0, PathFile.LastIndexOf("/"));
            string fileJSON = PathFile.Substring(PathFile.LastIndexOf("/") + 1);
            
            var requestJSON = new CommitRequest { Path = PathFile };
            FileCommits = 
                (await client.Repository.Commit.GetAll(
                    idRepository, requestJSON))
                     .Select(translate)
                    .ToArray();
            var dateJSON = FileCommits.Max(it => it.CommitedDate);

            var requestFolder = new CommitRequest { Since= dateJSON.AddTicks(1), Path = folder };
            FolderCommits =
                (await client.Repository.Commit.GetAll(
                    idRepository, requestFolder))
                .Where(it=>it.Commit.Committer.Date>dateJSON)
                .Select(translate)
                .ToArray();

            File =(await client.Repository.Content.GetAllContents(idRepository, PathFile)).First().Content;



        }

        public override async Task Init()
        {
            client = new GitHubClient(new ProductHeaderValue("Versioning"));
            string repoFullName = Owner + "/" + RepositoryName;
            if (!repoId.ContainsKey(repoFullName))
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    if (!repoId.TryGetValue(repoFullName, out idRepository))
                    {
                        Console.WriteLine("get value");
                        idRepository = (await client.Repository.Get(Owner, RepositoryName)).Id;
                        //Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "--" + idRepository);

                        repoId.AddOrUpdate(
                            repoFullName,
                            idRepository,
                            (key, oldValue) => idRepository);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            if(!repoId.TryGetValue(repoFullName,out idRepository))
            {
                throw new ArgumentException("cannot find" + repoFullName);
            }

        }
    }
}
