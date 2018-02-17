using System.Collections.Generic;
using System.Threading.Tasks;

namespace VersioningSourceControl
{
    public interface IVersionSourceControlFileFolder
    {
        Task Init();
        string PathFile { get; set; }
        Task FindFromSourceControl();
        ICommitSourceControl[] FileCommits { get; }
        ICommitSourceControl[] FolderCommits { get; }
        string File { get; }
    }

}
