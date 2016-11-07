using System;
using System.IO;

namespace Plotter4
{
    abstract class FileLoadModule
    {                
        static public byte[] Load(string path)
        {
            return File.ReadAllBytes(path);
        }

        static public void Load(string path, int chunkSize, Action<byte[], double> callback)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            long len = fs.Length;
            long pos = 0;
            byte[] buf;
            double perc = 0;

            while (pos < len)
            {
                int bytes_to_read = Math.Min(chunkSize, (int)(len - pos));
                buf = new byte[bytes_to_read];

                int bytes_read = br.Read(buf, 0, bytes_to_read);
                pos += bytes_read;
                perc = (double)pos / len;

                callback(buf, perc);
            }
        }
    }

    public enum FileLoadType
    {
        text, bytes, lines
    }
}
