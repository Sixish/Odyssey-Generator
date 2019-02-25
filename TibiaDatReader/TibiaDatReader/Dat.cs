using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text; // StringBuilder
//using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace TibiaDatReader
{
    public class Dat
    {
        private static Dictionary<float, long> VersionHeaders = new Dictionary<float, long> {
            {9.50F, 1333114795L},
            {9.51F, 1333114795L},
            {9.52F, 1334232365L},
            {9.53F, 1335956050L},
            {9.54F, 1339397483L},
            {9.60F, 1341813964L},
            {9.61F, 1344434077L},
            {9.62F, 1346156851L},
            {9.63F, 1346156851L},
            {9.70F, 1349690512L},
            {9.71F, 1349690512L},
            {9.80F, 1355220596L},
            {9.81F, 1355924918L},
            {9.82F, 1361882377L},
            {9.83F, 1363180391L},
            {9.84F, 1363180391L},
            {9.85F, 1365514779L},
            {9.86F, 1366354180L},
            {10.00F, 1370266196L},
            {10.01F, 1370943401L},
            {10.02F, 1370943401L},
            {10.10F, 1373894851L},
            {10.11F, 1373894851L},
            {10.12F, 1373894851L},
            {10.13F, 1378130969L},
            {10.20F, 1379332393L},
            {10.21F, 1382699112L},
            {10.22F, 1382699112L},
            {10.30F, 1386582070L},
            {10.31F, 1387189633L},
            {10.32F, 1389940905L},
            {10.33F, 1389940905L},
            {10.34F, 1390889653L},
            {10.35F, 1392376876L},
            {10.36F, 1393925246L},
            {10.37F, 1394518062L},
            {10.38F, 1395900825L},
            {10.39F, 1398427821L},
            {10.40F, 1400477773L},
            {10.41F, 1401114702L},
            {10.50F, 1404454414L},
            {10.51F, 1405668375L},
            {10.52F, 1407752381L},
            {10.53F, 1408948078L},
            {10.54F, 1410153031L},
            {10.55F, 1410500391L},
            {10.56F, 1411466160L},
            {10.57F, 1411724793L},
            {10.58F, 1412240103L},
            {10.59F, 1412696139L},
            {10.60F, 1414060487L},
            {10.61F, 1414060487L},
            {10.62F, 1415718456L},
            {10.63F, 1416319018L},
            {10.64F, 1417610686L},
            {10.70F, 1417788311L},
            {10.71F, 13135L},
            {10.72F, 14121L},
            {10.73F, 14157L},
            {10.74F, 14174L},
            {10.75F, 14197L},
            {10.76F, 14303L},
            {10.77F, 14558L},
            {10.78F, 14796L},
            {10.79F, 14961L},
            {10.80F, 15130L},
            {10.81F, 15220L},
            {10.82F, 15389L},
            {10.90F, 16166L}
        };

        private static int ReadUInt8(Stream fs)
        {
            return fs.ReadByte();
        }

        private static int ReadUInt16(Stream fs)
        {
            int val = 0;
            for (int i = 0; i < 2; i++)
            {
                val += (fs.ReadByte() << (i * 8));
            }
            return val;
        }

        private static long ReadULong32(Stream fs)
        {
            long val = 0L;
            for (int i = 0; i < 4; i++)
            {
                val += (fs.ReadByte() << (i * 8));
            }
            return val;
        }

        public float Version
        {
            get;
            set;
        }

        public int ItemCount
        {
            get;
            set;
        }

        public int OutfitCount
        {
            get;
            set;
        }

        public int EffectCount
        {
            get;
            set;
        }

        public int ProjectileCount
        {
            get;
            set;
        }


        [JsonIgnore]
        private int flagIsGroundWithSpeed = 0x00;
        [JsonIgnore]
        private int flagIsTopOrderOne = 0x01;
        [JsonIgnore]
        private int flagIsTopOrderTwo = 0x02;
        [JsonIgnore]
        private int flagIsTopOrderThree = 0x03;
        [JsonIgnore]
        private int flagIsContainer = 0x04;
        [JsonIgnore]
        private int flagIsStackable = 0x05;
        [JsonIgnore]
        private int flagIsCorpse = 0x06;
        [JsonIgnore]
        private int flagIsUsable = 0x07;
        [JsonIgnore]
        private int flagIsRewritable = 0x08;
        [JsonIgnore]
        private int flagIsWritable = 0x09;
        [JsonIgnore]
        private int flagIsFluidContainer = 0x0A;
        [JsonIgnore]
        private int flagIsFluid = 0x0B;
        [JsonIgnore]
        private int flagIsBlocking = 0x0C;
        [JsonIgnore]
        private int flagIsImmobile = 0x0D;
        [JsonIgnore]
        private int flagBlocksMissiles = 0x0E;
        [JsonIgnore]
        private int flagBlocksPath = 0x0F;
        [JsonIgnore]
        private int flagIsBlocking1000 = 0x10;
        [JsonIgnore]
        private int flagIsPickupable = 0x11;
        [JsonIgnore]
        private int flagIsHangable = 0x12;
        [JsonIgnore]
        private int flagIsHorizontal = 0x13;
        [JsonIgnore]
        private int flagIsVertical = 0x14;
        [JsonIgnore]
        private int flagIsRotatable = 0x15;
        [JsonIgnore]
        private int flagProvidesLight = 0x16;
        [JsonIgnore]
        private int flagIsUnknown23 = 0x17;
        [JsonIgnore]
        private int flagIsFloorChange = 0x18;
        [JsonIgnore]
        private int flagHasOffset = 0x19;
        [JsonIgnore]
        private int flagHasHeight = 0x1A;
        [JsonIgnore]
        private int flagIsBottomLayer = 0x1B;
        [JsonIgnore]
        private int flagHasMapColor = 0x1D;
        [JsonIgnore]
        private int flagHasActions = 0x1E;
        [JsonIgnore]
        private int flagIsGroundItem = 0x1F;
        [JsonIgnore]
        private int flagIsLookThrough = 0x20;
        [JsonIgnore]
        private int flagHasBodySlot = 0x21;
        [JsonIgnore]
        private int flagIsMarketable = 0x22;
        [JsonIgnore]
        private int flagUnknown35 = 0x23;
        [JsonIgnore]
        private int flagIsUnknown256 = 0xFE;

        public void InitializeVersion()
        {
            // 9.5 - 9.8 confirmed
            if (9.5F <= Version && Version <= 9.86F)
            {
                flagIsGroundWithSpeed = 0x00;
                flagIsTopOrderOne = 0x01;
                flagIsTopOrderTwo = 0x02;
                flagIsTopOrderThree = 0x03;
                flagIsContainer = 0x04;
                flagIsStackable = 0x05;
                flagIsCorpse = 0x06;
                flagIsUsable = 0x07;
                flagIsRewritable = 0x08;
                flagIsWritable = 0x09;
                flagIsFluidContainer = 0x0A;
                flagIsFluid = 0x0B;
                flagIsBlocking = 0x0C;
                flagIsImmobile = 0x0D;
                flagBlocksMissiles = 0x0E;
                flagBlocksPath = 0x0F;
                flagIsBlocking1000 = -1;
                flagIsPickupable = 0x10;
                flagIsHangable = 0x11;
                flagIsHorizontal = 0x12;
                flagIsVertical = 0x13;
                flagIsRotatable = 0x14;
                flagProvidesLight = 0x15;
                flagIsUnknown23 = 0x16;
                flagIsFloorChange = 0x17;
                flagHasOffset = 0x18;
                flagHasHeight = 0x19;
                flagIsBottomLayer = 0x1A;
                flagHasMapColor = 0x1C;
                flagHasActions = 0x1D;
                flagIsGroundItem = 0x1E;
                flagIsLookThrough = 0x1F;
                flagHasBodySlot = 0x20;
                flagIsMarketable = 0x21;
                flagUnknown35 = 0x22;
                flagIsUnknown256 = 0xFD;
            }

        }

        public List<DatItem> Items;

        public static long ReadHeader(Stream stream)
        {
            long datHeader = ReadULong32(stream);
            return datHeader;
        }

        public int ReadItemCount(Stream fs)
        {
            int itemCount = ReadUInt16(fs);
            return itemCount;
        }

        public int ReadOutfitCount(Stream fs)
        {
            int outfitCount = ReadUInt16(fs);
            return outfitCount;
        }

        public int ReadEffectCount(Stream fs)
        {
            int effectCount = ReadUInt16(fs);
            return effectCount;
        }

        public int ReadProjectileCount(Stream fs)
        {
            int projectileCount = ReadUInt16(fs);
            return projectileCount;
        }

        public void Parse(string src)
        {
            List<DatItem> items = new List<DatItem>();

            string debugMarketNameText = "";
            string marketName = "";

            using (FileStream fs = new FileStream(src, FileMode.Open, FileAccess.Read))
            {
                int val, innerVal;

                StringBuilder sb = new StringBuilder();

                Version = FindVersionByHeader(ReadHeader(fs));

                ItemCount = ReadItemCount(fs);
                OutfitCount = ReadOutfitCount(fs);
                EffectCount = ReadEffectCount(fs);
                ProjectileCount = ReadProjectileCount(fs);

                int c = 0;
                for (; c < 100; c++)
                {
                    items.Add(null);
                }

                for (; c <= ItemCount; c++)
                {
                    DatItem item = new DatItem();

                    items.Add(item);
                    sb.Clear();

                    marketName = "";

                    // Item flags.
                    while ((val = fs.ReadByte()) != 0xFF)
                    {
                        sb.Append(val.ToString("X2"));

                        if (val == flagIsGroundWithSpeed)
                        {
                            // Ground, speed.
                            item.IsGround = true;
                            item.Speed = ReadUInt16(fs);
                        }
                        else if (val == flagIsBlocking)
                        {
                            // Blocking.
                            item.IsBlocking = true;
                        }
                        else if (val == flagIsImmobile)
                        {
                            // Immobile
                            item.IsImmobile = true;
                        }
                        else if (val == flagBlocksMissiles)
                        {
                            item.BlocksMissiles = true;
                        }
                        else if (val == flagBlocksPath)
                        {
                            item.BlocksPath = true;
                        }
                        else if (val == flagProvidesLight)
                        {
                            item.ProvidesLight = true;
                            item.LightRadius = ReadUInt16(fs);
                            item.LightColor = ReadUInt16(fs);
                        }
                        else if (val == flagIsGroundItem)
                        {
                            item.IsGroundItem = true;
                        }
                        else if (val == flagHasOffset)
                        {
                            item.HasOffset = true;
                            item.OffsetX = ReadUInt16(fs);
                            item.OffsetY = ReadUInt16(fs);
                        }
                        else if (val == flagHasMapColor)
                        {
                            item.HasMapColor = true;
                            item.MapColor = ReadUInt16(fs);
                        }
                        else if (val == flagHasHeight)
                        {
                            item.HasHeight = true;
                            item.Height = ReadUInt16(fs);
                        }
                        else if (val == flagHasBodySlot)
                        {
                            item.HasBodySlot = true;
                            item.BodySlot = ReadUInt16(fs);
                        }
                        else if (val == flagUnknown35)
                        {
                            item.Unknown35 = ReadUInt16(fs);
                        }
                        else if (val == flagHasActions)
                        {
                            item.HasActions = true;
                            item.Actions = ReadUInt16(fs);
                        }
                        else if (val == flagIsWritable)
                        {
                            item.IsWritable = true;
                            item.Characters = ReadUInt16(fs);
                        }
                        else if (val == flagIsStackable)
                        {
                            item.IsStackable = true;
                        }
                        else if (val == flagIsMarketable)
                        {
                            // Market property - has a variable-length name.

                            // ?
                            int value;

                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            item.MarketUnknown1 = value;

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            item.MarketUnknown2 = value;

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            item.MarketUnknown3 = value;

                            // Market name length.
                            int attrMarketNameLength = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                attrMarketNameLength += (innerVal << (b * 8));

                            }
                            item.MarketNameLength = attrMarketNameLength;

                            // Market name.
                            marketName = "";
                            for (int b = 0; b < attrMarketNameLength; b++)
                            {
                                innerVal = fs.ReadByte();

                                marketName += Char.ConvertFromUtf32(innerVal);

                            }
                            item.MarketName = marketName;

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            item.MarketUnknown4 = value;

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            item.MarketUnknown5 = value;
                        }
                        else if (val == flagIsTopOrderOne)
                        {
                            item.IsTopOrderOne = true;
                        }
                        else if (val == flagIsTopOrderTwo)
                        {
                            item.IsTopOrderTwo = true;
                        }
                        else if (val == flagIsTopOrderThree)
                        {
                            item.IsTopOrderThree = true;
                        }
                        else if (val == flagIsContainer)
                        {
                            item.IsContainer = true;
                        }
                        else if (val == flagIsCorpse)
                        {
                            item.IsCorpse = true;
                        }
                        else if (val == flagIsPickupable)
                        {
                            item.IsPickupable = true;
                        }
                        else if (val == flagIsRotatable)
                        {
                            item.IsRotatable = true;
                        }
                        else if (val == flagIsUnknown23)
                        {
                            item.Unknown23 = true;
                        }
                        else if (val == flagIsUnknown256)
                        {
                            item.Unknown254 = true;
                        }
                        else if (val == flagIsUsable)
                        {
                            item.IsUsable = true;
                        }
                        else if (val == flagIsBottomLayer)
                        {
                            item.IsBottomLayer = true;
                        }
                        else if (val == flagIsLookThrough)
                        {
                            item.IsLookThrough = true;
                        }
                        else if (val == flagIsHangable)
                        {
                            item.IsHangable = true;
                        }
                        else if (val == flagIsFloorChange)
                        {
                            item.IsFloorChange = true;
                        }
                        else if (val == flagIsHorizontal)
                        {
                            item.IsHorizontal = true;
                        }
                        else if (val == flagIsVertical)
                        {
                            item.IsVertical = true;
                        }
                        else if (val == flagIsFluidContainer)
                        {
                            item.IsFluidContainer = true;
                        }
                        else if (val == flagIsFluid)
                        {
                            item.IsFluid = true;
                        }
                        else if (val == flagIsRewritable)
                        {
                            item.IsRewritable = true;
                            item.RewriteCharacters = ReadUInt16(fs);
                        }
                        else if (val == flagIsBlocking1000)
                        {
                            item.IsBlocking1000 = true;
                        }
                        else
                        {
                            Console.WriteLine("Unrecognized item flag: " + val + " (item ID: " + c + ")");
                        }

                    }

                    // Item global attributes.
                    item.SizeWidth = fs.ReadByte();

                    item.SizeHeight = fs.ReadByte();

                    // Weird value. No idea.
                    if (item.SizeWidth > 1 || item.SizeHeight > 1)
                    {
                        innerVal = fs.ReadByte();
                    }

                    item.SizeBase = fs.ReadByte();

                    item.SpriteSizeX = fs.ReadByte();

                    item.SpriteSizeY = fs.ReadByte();

                    item.SpriteSizeZ = fs.ReadByte();

                    item.Frames = fs.ReadByte();

                    item.SpriteCount = (item.SizeWidth * item.SizeHeight * item.SizeBase * item.SpriteSizeX * item.SpriteSizeY * item.SpriteSizeZ * item.Frames);

                    if (item.Frames > 1)
                    {
                        if (10.50F <= Version)
                        {
                            // Unknown animation header.
                            for (int i = 0; i < 6; i++)
                            {
                                innerVal = fs.ReadByte();

                            }

                            // Animation speeds (low quality).
                            for (int i = 0; i < item.Frames; i++)
                            {
                                for (int b = 0; b < 4; b++)
                                {
                                    innerVal = fs.ReadByte();

                                }
                            }

                            // Animation speeds (high quality).
                            for (int i = 0; i < item.Frames; i++)
                            {
                                for (int b = 0; b < 4; b++)
                                {
                                    innerVal = fs.ReadByte();

                                }
                            }

                        }
                    }

                    // Sprites
                    int bytesSprite = 0;
                    for (int i = 0; i < item.SpriteCount; i++)
                    {
                        if (9.5F <= Version && Version < 9.6F)
                        {
                            bytesSprite = 2;
                        }
                        else
                        {
                            bytesSprite = 4;
                        }

                        // Each sprite has 4 bytes.
                        string spr = "";
                        for (int b = 0; b < bytesSprite; b++)
                        {
                            innerVal = fs.ReadByte();

                            spr += innerVal.ToString("X2") + " ";

                        }
                    }
                    if (marketName != "")
                    {
                        debugMarketNameText += c + "\t" + marketName + Environment.NewLine;
                    }

                }
            }

            this.Items = items;
        }

        public static float FindVersionByHeader(long header)
        {
            foreach (KeyValuePair<float, long> kvp in VersionHeaders)
            {
                if (kvp.Value == header)
                {
                    return kvp.Key;
                }
            }
            return 0.0F;
        }

        public DatItem GetItem(int itemID)
        {
            return this.Items[itemID];
        }

        public static Dat Load(string src, float version)
        {
            Dat dat = new Dat(new List<DatItem>());
            dat.Version = version;
            dat.InitializeVersion();
            dat.Parse(src);

            return dat;
        }

        public static Dat Load(string src)
        {
            float version = 0.0F;
            using (FileStream fs = new FileStream(src, FileMode.Open, FileAccess.Read))
            {
                version = FindVersionByHeader(ReadHeader(fs));
            }

            return Load(src, version);
        }

        public Dat(List<DatItem> items)
        {
            this.Items = items;
        }
    }
}
