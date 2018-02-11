using System;

namespace VersioningSummary
{
    public class VersionDll
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime DateRelease { get; set; }
    }
}
