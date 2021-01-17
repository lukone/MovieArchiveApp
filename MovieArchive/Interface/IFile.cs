using System.IO;

namespace MovieArchive
{
    public interface IFile
    {
        MemoryStream GetFileStream(string Path);

        void SaveFileStream(string Path,Stream inputStream);
    }
}
