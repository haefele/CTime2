using System;
using static CTime2.Scripts.RunHelper;
using static CTime2.Scripts.FileHelper;
using static Bullseye.Targets;
using static CTime2.Scripts.CTimePaths;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CTime2.Scripts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Target("clean", () =>
            {
                GitResetFile(CTime2App.AppxManifest);
                foreach (var language in UpdateNotes.Languages)
                {
                    GitResetFile(UpdateNotes.GetTargetFilePath(language));
                }

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

            Target("sync-update-notes", DependsOn("clean"), () =>
            {
                foreach (var language in UpdateNotes.Languages)
                {
                    var sourceFile = UpdateNotes.GetSourceFilePath(language);
                    var targetFile = UpdateNotes.GetTargetFilePath(language);
                                       
                    CopyFile(sourceFile, targetFile);

                    var content = FileReadLines(targetFile);
                    var updatedContent = new List<string>();

                    bool firstLine = true;
                    foreach (var line in content.Skip(2)) //Throw away the first 2 lines
                    {
                        //Add lines before and after the version numbers (they always start with "## ")
                        if (line.StartsWith("## ") && firstLine == false)
                        {
                            updatedContent.Add("-----");
                        }

                        updatedContent.Add(line);

                        if (line.StartsWith("## "))
                        {
                            updatedContent.Add("-----");
                        }

                        firstLine = false;
                    }
                    FileWriteLines(targetFile, updatedContent);
                }
            });

            Target("build", DependsOn("update-appxmanifest-version", "sync-update-notes"), () =>
            {
                RunMsBuild($"\"{CTime2App.CsProj}\" /t:Restore");
                RunMsBuild($"\"{CTime2App.CsProj}\" /p:Configuration=Release /p:AppxPackageDir=AppPackages /p:AppxBundlePlatforms=\"x86|x64|ARM\" /p:UapAppxPackageBuildMode=StoreUpload /p:AppxBundle=Always");

                ZipDirectory(CTime2App.TestPackageDirectory, CTime2App.TestPackageArtifactPath);
                DeleteDirectory(CTime2App.TestPackageDirectory);

                ZipDirectory(CTime2App.AppPackagesDirectory, CTime2App.StorePackageArtifactPath);
            });

            Target("default", DependsOn("build"));

            await RunTargetsAndExitAsync(args);
        }
    }
}
