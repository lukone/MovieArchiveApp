using MovieArchive.iOS;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileIOS))]
namespace MovieArchive.iOS
{
    public class FileIOS : IFile
    {
        public byte[] GetFileStream(string Path)
        {
            //return new MemoryStream(File.ReadAllBytes(Path));
            int BufferSize = 0;
            byte[] result = null;
            using (Stream s = File.Open(Path, FileMode.Open))
            {
                result = new byte[s.Length];
                var offset = 0;
                while (s.ReadAsync(result, offset, BufferSize).Result > 0)
                {
                    offset += BufferSize;
                }
            }
            return result;
        }
        public void SaveFileStream(string Path, Stream inputStream)
        {
            FileStream fileStream = File.Open(Path, FileMode.Create);
            inputStream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
        }

        MemoryStream IFile.GetFileStream(string Path)
        {
            throw new NotImplementedException();
        }
    }
}