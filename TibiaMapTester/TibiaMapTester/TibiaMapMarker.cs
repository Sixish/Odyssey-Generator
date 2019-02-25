using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaMapTester
{
    public class TibiaMapMarker
    {
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

        public int OffsetX
        {
            get;
            set;
        }

        public int OffsetY
        {
            get;
            set;
        }

        public int Type
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public byte[] GetByteArray()
        {
            List<byte> byteList = new List<byte>();
            // Position X.
            byteList.Add((byte)OffsetX);
            byteList.Add((byte)BaseX);
            byteList.Add((byte)0x00);
            byteList.Add((byte)0x00);
            // Position Y.
            byteList.Add((byte)OffsetY);
            byteList.Add((byte)BaseY);
            byteList.Add((byte)0x00);
            byteList.Add((byte)0x00);
            // Type
            byteList.Add((byte)Type);
            // Unknown x3.
            byteList.Add((byte)0x00);
            byteList.Add((byte)0x00);
            byteList.Add((byte)0x00);
            // Text length.
            int textLength = Text.Length;
            byteList.Add((byte)(textLength & 0xFF));
            byteList.Add((byte)((textLength >> 8) & 0xFF));
            // Text.
            for (int i = 0; i < textLength; i++)
            {
                byteList.Add((byte)(Text[i]));
            }

            return byteList.ToArray<byte>();
        }

        public void SetPosition(int x, int y)
        {
            BaseX = (x >> 8) & 0xFF;
            BaseY = (y >> 8) & 0xFF;
            OffsetX = x & 0xFF;
            OffsetY = y & 0xFF;
        }

        public TibiaMapMarker(int posx, int posy, int type, string text)
        {
            SetPosition(posx, posy);
            Type = type;
            Text = text;
        }
    }
}
