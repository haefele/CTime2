using System.IO;
using System.IO.Compression;

namespace CTime2.Scripts
{
    public static class FileHelper
    {
        public static void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory) == false)
                return;

            Directory.Delete(directory, recursive: true);
        }

        public static void MoveDirectory(string fromDirectory, string toDirectory, string filePattern = "*.*")
        {
            Directory.CreateDirectory(toDirectory);

            foreach (string file in Directory.GetFiles(fromDirectory, filePattern, SearchOption.AllDirectories))
            {
                var dirPath = Path.GetDirectoryName(file);
                var targetDirPath = dirPath.Replace(fromDirectory, toDirectory);

                Directory.CreateDirectory(targetDirPath);

                File.Move(file, file.Replace(fromDirectory, toDirectory));
            }
        }

        public static void DeleteFiles(string directory, string filePattern)
        {
            foreach (var file in Directory.GetFiles(directory, filePattern))
            {
                File.Delete(file);
            }
        }

        public static void RenameFile(string directory, string fileName, string newFileName)
        {
            File.Move(Path.Combine(directory, fileName), Path.Combine(directory, newFileName));
        }

        public static void ZipDirectory(string directory, string outputFilePath)
        {
            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            var outputDirectory = Path.GetDirectoryName(outputFilePath);
            Directory.CreateDirectory(outputDirectory);

            ZipFile.CreateFromDirectory(directory, outputFilePath);
        }
    }
}
