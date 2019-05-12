using System.IO;
using System.Linq;

namespace CTime2.Scripts
{
    public static class CTimePaths
    {
        public static string SlnDirectory
        {
            get
            {
                var currentFolder = new DirectoryInfo(Path.GetDirectoryName(typeof(Program).Assembly.Location));
                return currentFolder.Parent.Parent.Parent.Parent.Parent.FullName;
            }
        }
        public static string ArtifactsDirectory => Path.Combine(SlnDirectory, "artifacts");

        public static string DotNetToolsDirectory => Path.Combine(SlnDirectory, "dotnettools");
        public static string Nbgv => Path.Combine(DotNetToolsDirectory, "nbgv");
        
        public static class CTime2Core
        {
            public static string CsProj => Path.Combine(SlnDirectory, "src", "CTime2.Core", "CTime2.VoiceCommandService.csproj");
            public static string BinDirectory => Path.Combine(SlnDirectory, "src", "CTime2.Core", "bin");
            public static string ObjDirectory => Path.Combine(SlnDirectory, "src", "CTime2.Core", "obj");
        }

        public static class CTime2EmployeeNotification
        {
            public static string CsProj => Path.Combine(SlnDirectory, "src", "CTime2.EmployeeNotificationService", "CTime2.EmployeeNotificationService.csproj");
            public static string BinDirectory => Path.Combine(SlnDirectory, "src", "CTime2.EmployeeNotificationService", "bin");
            public static string ObjDirectory => Path.Combine(SlnDirectory, "src", "CTime2.EmployeeNotificationService", "obj");
        }

        public static class CTime2LiveTile
        {
            public static string CsProj => Path.Combine(SlnDirectory, "src", "CTime2.LiveTileService", "CTime2.LiveTileService.csproj");
            public static string BinDirectory => Path.Combine(SlnDirectory, "src", "CTime2.LiveTileService", "bin");
            public static string ObjDirectory => Path.Combine(SlnDirectory, "src", "CTime2.LiveTileService", "obj");
        }

        public static class CTime2VoiceCommand
        {
            public static string CsProj => Path.Combine(SlnDirectory, "src", "CTime2.VoiceCommandService", "CTime2.VoiceCommandService.csproj");
            public static string BinDirectory => Path.Combine(SlnDirectory, "src", "CTime2.VoiceCommandService", "bin");
            public static string ObjDirectory => Path.Combine(SlnDirectory, "src", "CTime2.VoiceCommandService", "obj");
        }

        public static class CTime2App
        {
            public static string CsProj => Path.Combine(SlnDirectory, "src", "CTime2", "CTime2.csproj");
            public static string AppxManifest => Path.Combine(SlnDirectory, "src", "CTime2", "Package.appxmanifest");
            public static string BinDirectory => Path.Combine(SlnDirectory, "src", "CTime2", "bin");
            public static string ObjDirectory => Path.Combine(SlnDirectory, "src", "CTime2", "obj");

            public static string BundleArtifactsDirectory => Path.Combine(SlnDirectory, "src", "CTime2", "BundleArtifacts");
            public static string AppPackagesDirectory => Path.Combine(SlnDirectory, "src", "CTime2", "AppPackages");
            public static string TestPackageDirectory => Directory.GetDirectories(AppPackagesDirectory).FirstOrDefault(f => f.EndsWith("Test"));

            public static string TestPackageArtifactPath => Path.Combine(ArtifactsDirectory, "TestPackage.zip");
            public static string StorePackageArtifactPath => Path.Combine(ArtifactsDirectory, "StorePackage.zip");
        }
    }
}
