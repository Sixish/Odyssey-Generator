using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace TibiacastReader
{
    class TCRecording
    {
        public int Version = 0;
        public int Unknown1 = 0;
        public int ID = 0;
        public int VersionPostFix = 0;

        public static int ReadVersionNumber(Stream fs)
        {
            int versionNumber = 0;

            for (int i = 0; i < 2; i++)
            {
                versionNumber += fs.ReadByte() << (i * 8);
            }
            return versionNumber;
        }

        public static int ReadUnknown(Stream fs, int version)
        {
            int u1 = 0;
            int size = 4;

            // 0x804, 0x904, ..., 0x1104, 0x1204, 0x1704, 0x1804, 0x1E04
            if (0x804 <= version && version <= 0x1E04)
            {
                size = 5;
            }

            if (version == 0x404)
            {
                size = 0;
            }

            for (int i = 0; i < size; i++)
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

        public MemoryStream Decompress(string source)
        {
            MemoryStream ms = new MemoryStream();
            FileStream fs = new FileStream(source, FileMode.Open, FileAccess.Read);
            this.Version = ReadVersionNumber(fs);
            //this.VersionPostFix = ReadFixedPostVersion(fs);
            this.Unknown1 = ReadUnknown(fs, this.Version);
            long pos = fs.Position;

            using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
            {
                ds.CopyTo(ms);
            }
            return ms;
        }

        public TCRecording(int id)
        {
            this.ID = id;
        }
    }
}
