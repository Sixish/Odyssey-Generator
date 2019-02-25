using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCastRecordingParser
{
    class PacketStructureData
    {
        public int Type
        {
            get;
            set;
        }

        public string Filename
        {
            get;
            set;
        }

        public int Time
        {
            get;
            set;
        }

        public string Position
        {
            get;
            set;
        }

        public PacketStructureData(int type, string filename, int time, string position)
        {
            Type = type;
            Filename = filename;
            Time = time;
            Position = position;
        }
    }
}
