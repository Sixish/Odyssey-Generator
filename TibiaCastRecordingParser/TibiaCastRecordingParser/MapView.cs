using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TibiaCastRecordingParser
{
    public class MapView
    {
        public const int SIZEX = 256;
        public const int SIZEY = 256;

        public const int BITS_X = 8;
        public const int BITS_Y = 8;

        public const int BITFLAG_X = 0xFF;
        public const int BITFLAG_Y = 0xFF;

        [JsonProperty(Order = 1)]
        public int BaseX
        {
            get;
            set;
        }

        [JsonProperty(Order = 2)]
        public int BaseY
        {
            get;
            set;
        }

        [JsonProperty(Order = 3)]
        public int BaseZ
        {
            get;
            set;
        }

        [JsonProperty(Order = 4)]
        public HashSet<int> Items
        {
            get;
            set;
        }

        /*
        [JsonProperty(Order = 5, PropertyName = "Explored")]
        public int[] _JsonExplored
        {
            get
            {

            }
        }
        */

        [JsonProperty(Order = 5)]
        public int[] Explored
        {
            get;
            set;
        }

        [JsonIgnore]
        public MapTile[] Map
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Map", ObjectCreationHandling = ObjectCreationHandling.Replace, Order = 6)]
        public MapTile[] _JsonMap
        {
            get
            {
                List<MapTile> retMap = new List<MapTile>();
                for (int i = 0; i < Map.Length; i++)
                {
                    if (Map[i] != null)
                    {
                        retMap.Add(Map[i]);
                    }
                }
                return retMap.ToArray<MapTile>();
            }
            set
            {
                int i = 0;
                int exploredArrayIndex = -1;
                int yOffset = -1;
                int xOffset = -1;
                for (int x = 0; x < SIZEX; x++)
                {
                    for (int y = 0; y < SIZEY; y++)
                    {
                        yOffset = (int)(y / 32);
                        xOffset = (int)((SIZEY / 32) * x);
                        exploredArrayIndex = xOffset + yOffset;
                        if (((Explored[exploredArrayIndex] >> (y % 32)) & 0x1) == 1)
                        {
                            Map[(x << BITS_Y) + y] = value[i++];
                            if (i >= value.Length)
                            {
                                return;
                            }
                        }

                    }
                }
            }
        }

        public bool AddItem(int id)
        {
            if (!this.Items.Contains(id))
            {
                this.Items.Add(id);
                return true;
            }
            return false;
        }

        public bool AddMapTile(int x, int y, MapTile tile)
        {
            if (x >= SIZEX)
            {
                Console.WriteLine("Invalid x : " + x);
            }
            if (x >= SIZEY)
            {
                Console.WriteLine("Invalid y : " + y);
            }

            // Does not perform validation to ensure pos
            // has the correct BaseX/Y/Z.
            int posX = x & BITFLAG_X;
            int posY = y & BITFLAG_Y;

            int yOffset = -1;
            int xOffset = -1;
            int exploredArrayIndex;

            //int posZ = pos.Z; // A MapView is 2D, we don't care for the Z-Pos.

            //int offset = (posX << BITS_Y) + posY;

            if (!HasTile(posX, posY))
            {
                Map[(posX << BITS_Y) + posY] = tile;

                // 1 int = 32 vals
                // 256Y = 8 ints (a[0], ..., a[7]) = 3 bits.
                yOffset = (int)(y / 32);
                xOffset = (int)(x << 3);
                exploredArrayIndex = xOffset + yOffset;

                Explored[exploredArrayIndex] |= (1 << (posY % 32));
                for (int i = 0; i < tile.Items.Count; i++)
                {
                    AddItem(tile.Items[i].ID);
                }
                return true;
            }

            return false;
        }

        public MapTile GetTile(int x, int y)
        {
            return Map[(x << BITS_Y) + (y)];
        }

        public bool HasTile(int x, int y)
        {
             return GetTile(x, y) != null;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static MapView FromJSON(string json)
        {
            return JsonConvert.DeserializeObject<MapView>(json);
        }

        public bool IsEmpty()
        {
            return (this.Items.Count == 0);
        }

        public MapView(int bx, int by, int bz)
        {
            this.BaseX = bx;
            this.BaseY = by;
            this.BaseZ = bz;
            this.Items = new HashSet<int>();
            this.Map = new MapTile[SIZEX * SIZEY];
            this.Explored = new int[((SIZEX * SIZEY) / 32)];
        }

        public MapView()
        {
            this.Items = new HashSet<int>();
            this.Map = new MapTile[SIZEX * SIZEY];
            this.Explored = new int[((SIZEX * SIZEY) / 32)];
        }
    }
}
