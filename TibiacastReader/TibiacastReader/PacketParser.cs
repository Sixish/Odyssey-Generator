using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiacastReader
{
    class PacketParser
    {
        public FileStream fs;

        // For MapPacket, maybe others...
        public int bposx = 0;
        public int bposy = 0;
        public int bposz = 0;

        public int oposx = 0;
        public int oposy = 0;
        public int oposz = 0;

        public const int MAX_OFFSETX = 18;
        public const int MAX_OFFSETY = 14;
        public const int MAX_OFFSETZ = 16;

        public static bool hasStarted = false;

        public int ConsumeUntil(int b)
        {
            int byteVal;
            while ((byteVal = fs.ReadByte()) != -1 && byteVal != b)
            {
                // Om nom
            }
            return byteVal;
        }
        public int Consume(int n = 1)
        {
            int last = -1;
            for (int i = 0; i < n; i++)
            {
                last = this.fs.ReadByte();
            }
            return last;
        }

        public bool AdvancePosition(int dy = 1)
        {
            if (!hasStarted)
            {
                hasStarted = true;
                return false;
            }
            this.oposy += dy;
            int dx, dz;
            while (this.oposy >= MAX_OFFSETY)
            {
                dx = this.oposy / MAX_OFFSETY;
                this.oposy = this.oposy % MAX_OFFSETY;
                this.oposx += dx;
            }
            while (this.oposx >= MAX_OFFSETX)
            {
                dz = this.oposx / MAX_OFFSETX;
                this.oposx = this.oposx % MAX_OFFSETX;
                this.oposz += dz;
            }
            return true;
        }
        public PacketParser(FileStream fs)
        {
            this.fs = fs;
        }
        public int GetItemID(int b1, int b2)
        {
            return (b2 << 8) + b1;
        }
        public int GetPositionX()
        {
            return this.oposx + this.bposx;
        }
        public int GetPositionY()
        {
            return this.oposy + this.bposy;
        }
        public int GetPositionZ()
        {
            return this.oposz + this.bposz;
        }
        public Position GetPosition()
        {
            return new Position(
                this.oposx + this.bposx,
                this.oposy + this.bposy,
                this.oposz + this.bposz
            );
        }
        public List<SkillObject> ParseSkillPacket(int size, int id)
        {
            List<SkillObject> data = new List<SkillObject>();
            return data;
        }
    }
}
