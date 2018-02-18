using System;

namespace VersioningSourceControl
{
    public interface ICommitSourceControl
    {
        string Id { get; set; }
        string Committer { get; set; }
        string Message { get; set; }
        DateTimeOffset CommitedDate { get; set; }
    }

}
