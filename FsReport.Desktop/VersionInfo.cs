using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FsReport.Desktop
{
    internal class VersionInfo
    {
        private static readonly string _applicationPath = typeof(App).Assembly.Location;

        public static string? Name => FileVersionInfo.GetVersionInfo(_applicationPath).ProductName;

        public static string? Version => FileVersionInfo.GetVersionInfo(_applicationPath).ProductVersion;

        public static string? Developer => FileVersionInfo.GetVersionInfo(_applicationPath).CompanyName;

        public static string? Copyright => FileVersionInfo.GetVersionInfo(_applicationPath).LegalCopyright;

        public static string System => Environment.OSVersion.ToString();

        public static string Runtime => RuntimeInformation.FrameworkDescription;
    }
}
