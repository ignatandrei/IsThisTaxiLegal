using System.Collections.Generic;
using System.Threading.Tasks;

namespace VersioningSourceControl
{
    public abstract class VersionSourceControlFileFolder : IVersionSourceControlFileFolder
    {
        public VersionSourceControlFileFolder(string pathFile)
        {
            PathFile = pathFile;
        }
        public string PathFile { get ; set ; }

        public ICommitSourceControl[] FileCommits { get; protected set; }

        public ICommitSourceControl[] FolderCommits { get; protected set; }

        public string File { get; protected set; }
        
        public abstract Task Init();

        public abstract Task FindFromSourceControl();
        
        public bool HasModifications()
        {
            return FolderCommits?.Length > 0;
        }
    }
}
