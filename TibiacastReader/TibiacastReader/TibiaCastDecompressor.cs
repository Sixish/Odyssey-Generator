using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace TibiacastReader
{
    class TibiaCastDecompressor
    {
        public static int ReadVersionNumber(Stream fs)
        {
            int versionNumber = 0;

            for (int i = 0; i < 2; i++)
            {
                versionNumber += fs.ReadByte() << (i * 8);
            }
            return versionNumber;
        }

        public static int ReadUnknown(Stream fs)
        {
            int u1 = 0;

            for (int i = 0; i < 5; i++)
            {
                u1 += fs.ReadByte() << (i * 8);
            }

            return u1;
        }

        public static Object ReadRecordingHeader(Stream fs)
        {
            int versionNumber = 0;
            int u1 = 0;
            for (int i = 0; i < 2; i++)
            {
                versionNumber += fs.ReadByte() << (i * 8);
            }
            for (int i = 0; i < 5; i++)
            {
                u1 += fs.ReadByte() << (i * 8);
            }
            return new { version = versionNumber, unknown = u1 };
        }

        public static MemoryStream DecompressRecording(string source)
        {
            MemoryStream ms = new MemoryStream();
            using (FileStream fs = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                int version = ReadVersionNumber(fs);
                int u1 = ReadUnknown(fs);

                using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
                {
                    ds.CopyTo(ms);
                }
            }
            return ms;
        }
    }
}
