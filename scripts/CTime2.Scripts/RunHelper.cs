using System;
using System.Linq;
using static SimpleExec.Command;
using System.IO;

namespace CTime2.Scripts
{
    public static class RunHelper
    {
        private static readonly string MsBuildPath = GetMsBuildPath();
        private static string GetMsBuildPath()
        {
            var vsWhere = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "Installer", "vswhere.exe");
            var path = Read(vsWhere, "-version 16.0 -products * -requires Microsoft.Component.MSBuild -property installationPath").Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();

            return Path.Combine(path, "MSBuild", "Current", "Bin", "MSBuild.exe");
        }

        public static void RunDotNet(string arguments)
        {
            Run("dotnet", arguments, noEcho: true);
        }
        public static void RunDotNetTool(string arguments)
        {
            Run("dotnet", "tool " + arguments, noEcho: true);
        }
        public static void RunMsBuild(string arguments)
        {
            Run(MsBuildPath, arguments, noEcho: true);
        }
        public static void RunGit(string arguments)
        {
            try
            {
                Run("git", arguments, workingDirectory: CTimePaths.SlnDirectory, noEcho: true);
            }
            catch (SimpleExec.NonZeroExitCodeException e)
            {

            }
        }
        public static void RunNbgv(string arguments)
        {
            try
            {
                Run(CTimePaths.Nbgv, arguments, workingDirectory: CTimePaths.SlnDirectory, noEcho: true);
            }
            catch (SimpleExec.NonZeroExitCodeException e) when (e.ExitCode == 5 && arguments == "cloud")
            {
                // No cloud environment detected
            }
            catch
            {
                throw;
            }
        }
        public static string ReadNbgv(string arguments)
        {
            return Read(CTimePaths.Nbgv, arguments, workingDirectory: CTimePaths.SlnDirectory, noEcho: true);
        }
    }
}
