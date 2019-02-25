using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiaMapTester
{
    class TibiaMapFile
    {
        public const int SIZEX = 256;
        public const int SIZEY = 256;
        public const int SIZEZ = 1;

        public int BaseX
        {
            get;
            set;
        }

        public int BaseY
        {
            get;
            set;
        }

        public int BaseZ
        {
            get;
            set;
        }

        public bool[] UnwalkableTiles
        {
            get;
            set;
        }

        public int[] TileSpeeds
        {
            get;
            set;
        }

        public bool[] TileExists
        {
            get;
            set;
        }

        public int[] TileMapColors
        {
            get;
            set;
        }

        public List<TibiaMapMarker> Markers
        {
            get;
            set;
        }

        public static string GetTibiaMapFileName(int x, int y, int z)
        {
            return x.ToString().PadLeft(3, '0') + y.ToString().PadLeft(3, '0') + z.ToString().PadLeft(2, '0');
        }

        public string GetTibiaMapFileName()
        {
            return GetTibiaMapFileName(BaseX, BaseY, BaseZ);
        }

        public static int GetTileOffset(int x, int y)
        {
            return (x << 8) + y;
        }

        public void SetSpeed(int x, int y, int speed)
        {
            int offset = GetTileOffset(x, y);
            TileSpeeds[offset] = speed;
        }

        public int GetSpeed(int x, int y)
        {
            int offset = GetTileOffset(x, y);
            return TileSpeeds[offset];
        }

        public void SetMapColor(int x, int y, int colorID)
        {
            int offset = GetTileOffset(x, y);
            TileMapColors[offset] = colorID;
        }

        public int GetMapColor(int x, int y)
        {
            int offset = GetTileOffset(x, y);
            return TileMapColors[offset];
        }

        public void SetHasTile(int x, int y, bool val)
        {
            int offset = GetTileOffset(x, y);
            TileExists[offset] = val;
        }

        public bool GetHasTile(int x, int y)
        {
            int offset = GetTileOffset(x, y);
            return TileExists[offset];
        }

        public void SetIsUnwalkable(int x, int y, bool val)
        {
            int offset = GetTileOffset(x, y);
            UnwalkableTiles[offset] = val;
        }

        public bool GetIsUnwalkable(int x, int y)
        {
            int offset = GetTileOffset(x, y);
            return UnwalkableTiles[offset];
        }

        public TibiaMapMarker AddMapMarker(int x, int y, int type, string text)
        {
            TibiaMapMarker marker = new TibiaMapMarker((BaseX << 8) + x, (BaseY << 8) + y, type, text);
            Markers.Add(marker);
            return marker;
        }

        public void Save(string directory)
        {
            int x, y, offset;
            List<byte> outBuffer = new List<byte>();
            byte[] colorBuffer = new byte[256 * 256];
            byte[] speedBuffer = new byte[256 * 256];
            //byte[] outBuffer = new byte[256 * 256 * 2 + 4];

            for (x = 0; x < 256; x++)
            {
                for (y = 0; y < 256; y++)
                {
                    offset = GetTileOffset(x, y);

                    // Color
                    colorBuffer[offset] = (byte)GetMapColor(x, y);

                    // Path
                    byte pathByte;
                    if (GetHasTile(x, y))
                    {
                        pathByte = (byte)(0xFF - (GetIsUnwalkable(x, y) ? 0x00 : GetSpeed(x, y)));
                    }
                    else
                    {
                        pathByte = (byte)0xFA;
                    }
                    speedBuffer[offset] = pathByte;
                }
            }

            outBuffer.AddRange(colorBuffer);
            outBuffer.AddRange(speedBuffer);

            int markerCount = Math.Min(Markers.Count, 0x2000);
            if (markerCount > 0)
            {
                outBuffer.Add((byte)((markerCount >> 0) & 0xFF));
                outBuffer.Add((byte)((markerCount >> 8) & 0xFF));
                outBuffer.Add((byte)((markerCount >> 16) & 0xFF));
                outBuffer.Add((byte)((markerCount >> 24) & 0xFF));
                for (int i = 0; i < markerCount; i++)
                {
                    outBuffer.AddRange(Markers[i].GetByteArray());
                }
            }

            // Null-termination bytes.
            for (int i = 0; i < 4; i++)
            {
                outBuffer.Add((byte)0x00);
            }

            byte[] byteBuffer = outBuffer.ToArray<byte>();

            using (FileStream fs = new FileStream(directory + "\\" + GetTibiaMapFileName() + ".map", FileMode.Create, FileAccess.Write))
            {
                fs.Write(byteBuffer, 0, byteBuffer.Length);
            }
        }

        protected void Initialize()
        {
            this.TileExists = new bool[256 * 256];
            this.TileMapColors = new int[256 * 256];
            this.TileSpeeds = new int[256 * 256];
            this.UnwalkableTiles = new bool[256 * 256];
            this.Markers = new List<TibiaMapMarker>();
        }

        public TibiaMapFile(int x, int y, int z)
        {
            BaseX = x;
            BaseY = y;
            BaseZ = z;

            Initialize();
        }

        public TibiaMapFile()
        {
            Initialize();
        }
    }
}
