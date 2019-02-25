using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiacastReader
{
    class Packet
    {
        public const int ID_MAP_WHOLE = 0x64;
        public const int ID_MAP_NORTH = 0x65;
        public const int ID_MAP_EAST = 0x66;
        public const int ID_MAP_SOUTH = 0x67;
        public const int ID_MAP_WEST = 0x68;

        public const int ID_MAP_UP = 0xBE;
        public const int ID_MAP_DOWN = 0xBF;

        public MemoryStream Stream = null;

        public byte[] data;

        public int packetSize = 0;
        public int packetType = 0;

        public static List<int> VerifiedVersions = new List<int>()
        {
            0x0404,// 9.60F

            0x0504,// 9.70F
            0x0804,// 9.80F

            0x0904,// 9.86F
            0x0C04,// 9.86F
            0x0E04,// 10.01F

            0x0F04,// 10.10F

            0x1104,// 10.20F

            0x1204,// 10.30F
            // 0x1604, // 10.30F??
            0x1704,// 10.30F

            0x1804,// 10.50F
            0x1E04,// 10.80F
        };

        public int ReadPacketSize(MemoryStream ms)
        {
            int size = 0;
            for (int i = 0; i < 2; i++)
            {
                size += ms.ReadByte() << (i * 8);
            }
            return size;
        }
        public int ReadPacketType(MemoryStream ms)
        {
            int type = 0;
            for (int i = 0; i < 1; i++)
            {
                type += ms.ReadByte() << (i * 8);
            }
            return type;
        }

        public static int ReadIntFromBytes(Stream fs, int bytes)
        {
            int ret = 0;
            for (int i = 0; i < bytes; i++)
            {
                ret += (int)(fs.ReadByte() << (i * 8));
            }

            return ret;
        }

        public static string ReadStringFromBytes(Stream fs, int chars)
        {
            string outString = "";
            try
            {

                for (int i = 0; i < chars; i++)
                {
                    outString += char.ConvertFromUtf32(fs.ReadByte());
                }
                return outString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
        public static TCCreature ParseCreature(Stream fs, int version)
        {
            long charid = 0;
            int u4 = 0;
            int charnamelength = 0;
            int u5 = 0;
            string charname = "";
            int hppc = 0;
            int direction = 0;
            int outfit = 0;
            int colorHead = 0;
            int colorBody = 0;
            int colorLegs = 0;
            int colorFeet = 0;
            int addons = 0;
            int mount = 0;
            int lightRadius = 0;
            int lightColor = 0;
            int speed = 0;
            int skull = 0;
            int partyshield = 0;
            int guildshield = 0;
            int u6 = 0;
            charid = ReadIntFromBytes(fs, 4);
            u4 = ReadIntFromBytes(fs, 1);
            charnamelength = ReadIntFromBytes(fs, 1);
            u5 = ReadIntFromBytes(fs, 1);
            charname = ReadStringFromBytes(fs, charnamelength);
            hppc = ReadIntFromBytes(fs, 1);
            direction = ReadIntFromBytes(fs, 1);
            outfit = ReadIntFromBytes(fs, 2);
            colorHead = ReadIntFromBytes(fs, 1);
            colorBody = ReadIntFromBytes(fs, 1);
            colorLegs = ReadIntFromBytes(fs, 1);
            colorFeet = ReadIntFromBytes(fs, 1);
            if (outfit != 0)
            {
                addons = ReadIntFromBytes(fs, 1);
                mount = ReadIntFromBytes(fs, 2);
            }
            lightRadius = ReadIntFromBytes(fs, 1);
            lightColor = ReadIntFromBytes(fs, 1);
            speed = ReadIntFromBytes(fs, 2);
            skull = ReadIntFromBytes(fs, 1);
            partyshield = ReadIntFromBytes(fs, 1); // Possibly removed in 9.5.
            // Old versions lack a GuildShield(?).

            if (version > 0x1104)
            //if (version != 0x1104)
            {
                guildshield = ReadIntFromBytes(fs, 1);
            }

            // ?

            if (version == 0x0F04)
            {
                // Works for 10.10
                u6 = ReadIntFromBytes(fs, 2);
            }
            else
            {
                // Works for others...
                u6 = ReadIntFromBytes(fs, 6);
            }
            // 9.5. Where do these come from?
            fs.Position -= 4;
            fs.Position -= 1; // REC # 197453, April 3rd, 2012, 14:40 UTC
            fs.Position += 1; // REC # 308461, ?

            // 0x0F04.
            if (0x0F04 <= version && version <= 0x0F04)
            {
                fs.Position += 4;
            }

            // 0x1104-
            if (0x1104 <= version && version <= 0x104)
            {
                fs.Position += 4;
            }

            // 0x1204 - 0x1204 (OK)
            if (0x1204 <= version && version <= 0x1404)
            //if (0x1204 <= version && version <= 0x1604)
            {
                fs.Position += 3;
            }

            // 0x1704 (OK)
            //if (0x1704 <= version)
            if (0x1604 <= version)
            {
                fs.Position += 4;
            }

            return new TCCreature(charid, u4, charnamelength,
                u5, charname, hppc, direction, outfit, colorHead,
                colorBody, colorLegs, colorFeet, addons, mount,
                lightRadius, lightColor, speed, skull, partyshield,
                guildshield, u6);
        }
        public static List<TCCreature> ParseCreatureList(Stream fs, int version)
        {
            List<TCCreature> list = new List<TCCreature>();
            int creatures = 0;

            // Not sure what the first byte is used for...
            // Not present in 9.5.??
            //int pre = fs.ReadByte();
            if (0xC04 <= version)
            {
                ReadIntFromBytes(fs, 1);
            }

            for (int i = 0; i < 2; i++)
            {
                creatures += fs.ReadByte() << (8 * i);
            }

            for (int i = 0; i < creatures; i++)
            {
                // Parse CreatureData
                TCCreature c = ParseCreature(fs, version);

                list.Add(c);
            }
            if (!VerifiedVersions.Contains(version))
            {
                Console.WriteLine("DEBUG: " + version + ": CREATURES");
            }
            return list;
        }
        public static Packet ReadPacket(MemoryStream ms)
        {
            Packet packet = new Packet();
            packet.packetSize = packet.ReadPacketSize(ms);
            packet.packetType = packet.ReadPacketType(ms);
            // Todo remove.
            if (packet.packetSize < 0)
            {
                Console.WriteLine("Negative packet size ...");
                ms.Position -= 1;
                return packet;// new MemoryStream();
            }

            byte[] packetData = new byte[packet.packetSize];
            for (int i = 0; i < packetData.Length; i++)
            {
                packetData[i] = (byte)ms.ReadByte();
            }
            packet.data = packetData;
            ms.Position -= 1;
            // needs value
            //return new MemoryStream(packetData);

            return packet;
        }
        public static int GetPacketType(MemoryStream ms)
        {
            ms.Position = 0;
            return ms.ReadByte();
        }
        public static Position GetMapPosition(Packet packet)
        {
            Position pos = new Position(0, 0, 0);
            int bPosition = 0;

            for (int i = 0; i < 2; i++, bPosition++)
            {
                pos.x += packet.data[bPosition] << (i * 8);
            }
            for (int i = 0; i < 2; i++, bPosition++)
            {
                pos.y += packet.data[bPosition] << (i * 8);
            }
            for (int i = 0; i < 1; i++, bPosition++)
            {
                pos.z += packet.data[bPosition] << (i * 8);
            }
            return pos;
        }


        public Packet()
        {
        }
    }
}
