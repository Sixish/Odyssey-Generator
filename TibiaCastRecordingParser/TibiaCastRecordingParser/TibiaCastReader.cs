using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCastRecordingParser
{
    class TibiaCastReader
    {
        public static Dictionary<long, float> TibiacastDatVersions = new Dictionary<long, float>
        {
            //{???L, 9.50F},
            //{???L, 9.51F},
            //{???L, 9.52F},
            //{???L, 9.53F},
            //{???L, 9.54F},

            
            {0x404L, 9.60F},

            {0x504L, 9.70F}, // OK
            {0x804L, 9.80F}, // OK

            {0x904L, 9.86F}, // OK?
            {0xC04L, 9.86F}, // ?
            {0xE04L, 10.01F}, // OK?

            {0x0F04L, 10.10F}, // OK?

            {0x1004, 10.10F}, // OK? (13/09/2013)

            {0x1104L, 10.20F}, // ? TODO TEST
            {0x1204L, 10.30F}, // ? TODO TEST
            {0x1404L, 10.39F}, // OK? (26/02/2014)
            {0x1604L, 10.39F}, // ? TODO TEST
            {0x1704L, 10.39F}, // OK? (05/05/2014)
            {0x1804L, 10.51F}, // OK? (Resolves issue with non-animated water edging).
            {0x1E04L, 10.90F},
        };
        public static Dictionary<float, string> TibiaDatDirectoryNames = new Dictionary<float, string>
        {
            {9.60F, "tibia960"},
            {9.70F, "tibia970"},
            {9.80F, "tibia980"},
            {9.86F, "tibia986"},
            {10.01F, "tibia1001"},
            {10.10F, "tibia1010"},
            {10.20F, "tibia1020"},
            {10.30F, "tibia1030"},
            //{10.50F, "tibia1050"},
            {10.51F, "tibia1051"},
            {10.80F, "tibia1080"},
            {10.90F, "tibia1090"},
        };
    }
}
