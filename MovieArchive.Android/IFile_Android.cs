using MovieArchive.Droid;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileAndroid))]
namespace MovieArchive.Droid
{

    public class FileAndroid : IFile
    {
        public MemoryStream GetFileStream(string path)
        {
            return new MemoryStream(File.ReadAllBytes(path));
        }

        public void SaveFileStream(string Path, Stream inputStream)
        {
            FileStream fileStream = File.Open(Path, FileMode.Create);
            inputStream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
        }

    }

}