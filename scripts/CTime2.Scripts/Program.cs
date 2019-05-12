using System;
using System.IO;
using static CTime2.Scripts.RunHelper;
using static CTime2.Scripts.FileHelper;
using static Bullseye.Targets;
using static CTime2.Scripts.CTimePaths;
using System.Threading.Tasks;

namespace CTime2.Scripts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Target("clean", () =>
            {
                RunGit($"reset HEAD \"{CTime2App.AppxManifest}\"");
                RunGit($"checkout -- \"{CTime2App.AppxManifest}\"");

                DeleteDirectory(ArtifactsDirectory);
                DeleteDirectory(DotNetToolsDirectory);

                DeleteDirectory(CTime2Core.BinDirectory);
                DeleteDirectory(CTime2Core.ObjDirectory);

                DeleteDirectory(CTime2EmployeeNotification.BinDirectory);
                DeleteDirectory(CTime2EmployeeNotification.ObjDirectory);

                DeleteDirectory(CTime2LiveTile.BinDirectory);
                DeleteDirectory(CTime2LiveTile.ObjDirectory);

                DeleteDirectory(CTime2VoiceCommand.BinDirectory);
                DeleteDirectory(CTime2VoiceCommand.ObjDirectory);

                DeleteDirectory(CTime2App.BinDirectory);
                DeleteDirectory(CTime2App.ObjDirectory);
                DeleteDirectory(CTime2App.AppPackagesDirectory);
                DeleteDirectory(CTime2App.BundleArtifactsDirectory);
            });

            Target("setup-versioning", DependsOn("clean"), () =>
            {
                RunDotNetTool($"install nbgv --tool-path \"{DotNetToolsDirectory}\"");
                RunNbgv("cloud");
            });

            Target("update-appxmanifest-version", DependsOn("setup-versioning"), () =>
            {
                var version = Version.Parse(ReadNbgv("get-version -v Version"));
                UpdateAppxmanifestVersion(CTime2App.AppxManifest, version);
            });

            Target("build", DependsOn("update-appxmanifest-version"), () =>
            {
                RunMsBuild($"\"{CTime2App.CsProj}\" /t:Restore");
                RunMsBuild($"\"{CTime2App.CsProj}\" /p:Configuration=Release /p:AppxPackageDir=AppPackages /p:AppxBundlePlatforms=\"x86|x64|ARM\" /p:UapAppxPackageBuildMode=StoreUpload /p:AppxBundle=Always");
            });

            Target("default", DependsOn("build"));

            await RunTargetsAndExitAsync(args);
        }
    }

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
            public static string AppPackagesDirectory => Path.Combine(SlnDirectory, "src", "CTime2", "AppPackages");
            public static string BundleArtifactsDirectory => Path.Combine(SlnDirectory, "src", "CTime2", "BundleArtifacts");
        }
    }
}
