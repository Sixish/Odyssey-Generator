using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiacastReader
{
    class TCBlockMetaData
    {
        public int timestamp = 0;
        public int size = 0;
        public int unknown1 = 0;
        public int unknown2 = 0;
        public int type = 0;

        public TCBlockMetaData(int timestamp, int size, int unknown1, int unknown2, int type)
        {
            this.timestamp = timestamp;
            this.size = size;
            this.unknown1 = unknown1;
            this.unknown2 = unknown2;
            this.type = type;
        }
    }
}
