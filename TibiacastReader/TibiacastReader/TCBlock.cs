using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiacastReader
{
    class TCBlock
    {
        public const int BLOCK_TYPE_UNKNOWN = 0x06;
        public const int BLOCK_TYPE_INIT = 0x07;
        public const int BLOCK_TYPE_PACKETS = 0x08;
        public const int BLOCK_TYPE_TIBIACASTMESSAGE = 0x09;

        public static List<int> ExpectedBlockTypes = new List<int>()
        {
            BLOCK_TYPE_UNKNOWN,
            BLOCK_TYPE_INIT,
            BLOCK_TYPE_PACKETS,
            BLOCK_TYPE_TIBIACASTMESSAGE
        };

        public MemoryStream Stream = null;

        public int blockSize = 0;
        public int blockTime = 0;
        public int blockType = 0;

        public static TCBlock ReadBlock(MemoryStream byteCheckerFile)
        {
            TCBlock block = new TCBlock(byteCheckerFile);
            block.blockTime = block.ReadBlockTime(byteCheckerFile);
            block.blockSize = block.ReadBlockSize(byteCheckerFile);
            block.blockType = block.ReadBlockType(byteCheckerFile);

            return block;
        }

        public int ReadBlockSize(MemoryStream ms)
        {
            int size = 0;
            for (int i = 0; i < 4; i++)
            // Works for 9.5
            //for (int i = 0; i < 2; i++)
            {
                size += (ms.ReadByte() << (8 * i));
            }
            return size;
        }
        public int ReadBlockType(MemoryStream ms)
        {
            int type = 0;
            for (int i = 0; i < 1; i++)
            {
                type += ms.ReadByte() << (i * 8);
            }
            return type;
        }
        public int ReadBlockTime(MemoryStream ms)
        {
            int timestamp = 0;
            for (int i = 0; i < 4; i++)
            {
                timestamp += ms.ReadByte() << (i * 8);
            }
            return timestamp;
        }

        public TCBlock(MemoryStream ms)
        {
            this.Stream = ms;
        }
    }
}
