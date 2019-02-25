using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DatReader
{
    class Dat
    {
        private static int ReadUInt16(Stream fs)
        {
            int val = 0;
            for (int i = 0; i < 2; i++)
            {
                val += (fs.ReadByte() << (i * 8));
            }
            return val;
        }

        public List<DatItem> Items;

        public static List<DatItem> Parse(string src)
        {
            List<DatItem> items = new List<DatItem>();

            string debugMarketNameText = "";
            string marketName = "";

            using (FileStream fs = new FileStream(src, FileMode.Open, FileAccess.Read))
            {
                int val, innerVal;

                long datVersion = 0L;

                int countItems = 0;
                int countOutfits = 0;
                int countEffects = 0;
                int countProjectiles = 0;

                // File header?
                for (int b = 0; b < 4; b++)
                {
                    datVersion += (fs.ReadByte() << (b * 8));
                }

                // Item count.
                for (int b = 0; b < 2; b++)
                {
                    countItems += (fs.ReadByte() << (b * 8));
                }

                // Outfit count.
                for (int b = 0; b < 2; b++)
                {
                    countOutfits += (fs.ReadByte() << (b * 8));
                }

                // Effect count.
                for (int b = 0; b < 2; b++)
                {
                    countEffects += (fs.ReadByte() << (b * 8));
                }

                // Projectile count.
                for (int b = 0; b < 2; b++)
                {
                    countProjectiles += (fs.ReadByte() << (b * 8));
                }

                for (int c = 100; c < countItems; c++)
                {
                    DatItem item = new DatItem();
                    items.Add(item);

                    marketName = "";

                    // Item flags.
                    while ((val = fs.ReadByte()) != 0xFF)
                    {


                        if (val == 0x00)
                        {
                            // Ground, speed.
                            item.IsGround = true;
                            item.Speed = ReadUInt16(fs);
                        }
                        else if (val == 0x0C)
                        {
                            // Blocking.
                            item.IsBlocking = true;
                        }
                        else if (val == 0x0D)
                        {
                            // Immobile
                            item.IsImmobile = true;
                        }
                        else if (val == 0x0E)
                        {
                            item.BlocksMissiles = true;
                        }
                        else if (val == 0x0F)
                        {
                            item.BlocksPath = true;
                        }
                        else if (val == 0x16)
                        {
                            item.ProvidesLight = true;
                            item.LightRadius = ReadUInt16(fs);
                            item.LightColor = ReadUInt16(fs);
                        }
                        else if (val == 0x1F)
                        {
                            item.IsGroundItem = true;
                        }
                        else if (val == 0x19)
                        {
                            item.HasOffset = true;
                            item.OffsetX = ReadUInt16(fs);
                            item.OffsetY = ReadUInt16(fs);
                        }
                        else if (val == 0x1D)
                        {
                            item.HasMapColor = true;
                            item.MapColor = ReadUInt16(fs);
                        }
                        else if (val == 0x1A)
                        {
                            item.HasHeight = true;
                            item.Height = ReadUInt16(fs);
                        }
                        else if (val == 0x21)
                        {
                            item.HasBodySlot = true;
                            item.BodySlot = ReadUInt16(fs);
                        }
                        else if (val == 0x23)
                        {
                            item.Unknown35 = ReadUInt16(fs);
                        }
                        else if (val == 0x1E)
                        {
                            item.HasActions = true;
                            item.Actions = ReadUInt16(fs);
                        }
                        else if (val == 0x09)
                        {
                            item.IsWritable = true;
                            item.Characters = ReadUInt16(fs);
                        }
                        else if (val == 0x05)
                        {
                            item.IsStackable = true;
                        }
                        else if (val == 0x22)
                        {
                            // Market property - has a variable-length name.

                            // ?
                            int value;
                            string marketFlag = "";

                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            marketFlag += value + "\t";

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            marketFlag += value + "\t";

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            marketFlag += value + "\t";

                            // Market name length.
                            int attrMarketNameLength = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                attrMarketNameLength += (innerVal << (b * 8));

                            }
                            marketFlag += attrMarketNameLength + "\t";

                            // Market name.
                            marketName = "";
                            for (int b = 0; b < attrMarketNameLength; b++)
                            {
                                innerVal = fs.ReadByte();

                                marketName += Char.ConvertFromUtf32(innerVal);

                            }
                            marketFlag += marketName + "\t";

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            marketFlag += value + "\t";

                            // ?
                            value = 0;
                            for (int b = 0; b < 2; b++)
                            {
                                innerVal = fs.ReadByte();

                                value += (innerVal << (b * 8));

                            }
                            marketFlag += value + "\t";
                        }
                        else if (val == 0x01)
                        {
                            item.IsTopOrderOne = true;
                        }
                        else if (val == 0x02)
                        {
                            item.IsTopOrderTwo = true;
                        }
                        else if (val == 0x03)
                        {
                            item.IsTopOrderThree = true;
                        }
                        else if (val == 0x04)
                        {
                            item.IsContainer = true;
                        }
                        else if (val == 0x06)
                        {
                            item.IsCorpse = true;
                        }
                        else if (val == 0x11)
                        {
                            item.IsPickupable = true;
                        }
                        else if (val == 0x15)
                        {
                            item.IsRotatable = true;
                        }
                        else if (val == 0x17)
                        {
                            item.Unknown23 = true;
                        }
                        else if (val == 0xFE)
                        {
                            item.Unknown254 = true;
                        }
                        else if (val == 0x07)
                        {
                            item.IsUsable = true;
                        }
                        else if (val == 0x1B)
                        {
                            item.IsBottomLayer = true;
                        }
                        else if (val == 0x20)
                        {
                            item.IsLookThrough = true;
                        }
                        else if (val == 0x12)
                        {
                            item.IsHangable = true;
                        }
                        else if (val == 0x18)
                        {
                            item.IsFloorChange = true;
                        }
                        else if (val == 0x13)
                        {
                            item.IsHorizontal = true;
                        }
                        else if (val == 0x14)
                        {
                            item.IsVertical = true;
                        }
                        else if (val == 0x0A)
                        {
                            item.IsFluidContainer = true;
                        }
                        else if (val == 0x0B)
                        {
                            item.IsFluid = true;
                        }
                        else if (val == 0x08)
                        {
                            item.IsRewritable = true;
                            item.RewriteCharacters = ReadUInt16(fs);
                        }
                        else if (val == 0x10)
                        {
                            item.IsBlocking1000 = true;
                        }
                        else
                        {
                            Console.WriteLine("Unrecognized item flag: " + val + " (item ID: " + c + ")");
                        }

                    }

                    // Item global attributes.
                    int attrWidth = fs.ReadByte();

                    int attrHeight = fs.ReadByte();

                    // Weird value. No idea.
                    if (attrWidth > 1 || attrHeight > 1)
                    {
                        innerVal = fs.ReadByte();

                    }

                    int attrBase = fs.ReadByte();

                    int attrSpritesX = fs.ReadByte();

                    int attrSpritesY = fs.ReadByte();

                    int attrSpritesZ = fs.ReadByte();

                    int frameCount = fs.ReadByte();

                    int spriteCount = (attrWidth * attrHeight * attrBase * attrSpritesX * attrSpritesY * attrSpritesZ * frameCount);

                    if (frameCount > 1)
                    {
                        if (datVersion == 15389L)
                        {

                            // Unknown animation header.
                            for (int i = 0; i < 6; i++)
                            {
                                innerVal = fs.ReadByte();

                            }

                            // Animation speeds (low quality).
                            for (int i = 0; i < frameCount; i++)
                            {
                                for (int b = 0; b < 4; b++)
                                {
                                    innerVal = fs.ReadByte();

                                }
                            }

                            // Animation speeds (high quality).
                            for (int i = 0; i < frameCount; i++)
                            {
                                for (int b = 0; b < 4; b++)
                                {
                                    innerVal = fs.ReadByte();

                                }
                            }

                        }
                    }

                    // Sprites
                    for (int i = 0; i < spriteCount; i++)
                    {
                        // Each sprite has 4 bytes.
                        string spr = "";
                        for (int b = 0; b < 4; b++)
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

            return items;
        }

        public static Dat Load(string src)
        {
            return new Dat(Parse(src));
        }

        public Dat(List<DatItem> items)
        {
            this.Items = items;
        }
    }
}
