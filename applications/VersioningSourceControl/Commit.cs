using System;

namespace VersioningSourceControl
{
    public class CommitSourceControl : ICommitSourceControl
    {
        public string Id { get; set; }
        public string Committer { get ; set ; }
        public string Message { get ; set ; }
        public DateTimeOffset CommitedDate { get ; set ; }
    }
}
