using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TibiaCastRecordingParser
{
    public class MapTile
    {
        [JsonIgnore]
        public List<MapItem> Items
        {
            get;
            set;
        }

        [JsonIgnore]
        public List<MapCreature> Creatures
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Items", NullValueHandling = NullValueHandling.Ignore, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<MapItem> _JsonItems
        {
            get
            {
                if (Items.Count < 1)
                {
                    return null;
                }
                return Items;
            }
            set
            {
                Items = value;
            }
        }

        [JsonProperty(PropertyName="Creatures", NullValueHandling = NullValueHandling.Ignore, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<MapCreature> _JsonCreatures
        {
            get
            {
                if (Creatures.Count < 1)
                {
                    return null;
                }
                return Creatures;
            }
            set
            {
                Creatures = value;
            }
        }

        public static long GetOffset(Position3D pos)
        {
            // We have to convert the inputs or << will not work properly
            long ox = (long)pos.X, oy = (long)pos.Y, oz = (long)pos.Z;
            long offset = (ox & 0xFFFF) + ((oy & 0xFFFF) << 16) + ((oz & 0xF) << 32);
            return offset;
        }

        public override string ToString()
        {
            string output = "";
            List<MapItem> items = Items;
            output += items.Count.ToString("X2");
            MapItem item;
            for (int i = 0, len = items.Count; i < len; i++)
            {
                item = items[i];
                output += item.ID.ToString("X4");

                if (item.IsStackable)
                {
                    output += ((int)item.Count).ToString("X2");
                }

                if (item.IsAnimated)
                {
                    output += ((int)item.AnimationStep).ToString("X2");
                }

                if (item.IsFluid)
                {
                    output += ((int)item.Fluid).ToString("X2");
                }

                if (item.IsSplash)
                {
                    output += ((int)item.Splash).ToString("X2");
                }
            }
            return output;
        }

        public string GetSpawnList()
        {
            string output = "";
            string outputCreatures = "";
            int creatureCount = 0;
            for (int i = 0; i < this.Creatures.Count; i++)
            {
                if (this.Creatures[i].CreatureType != 0)
                {
                    outputCreatures += (this.Creatures[i].CharNameLength.ToString("X2"));
                    outputCreatures += (this.Creatures[i].CharName);
                    creatureCount++;
                }
            }
            if (creatureCount > 0)
            {
                output += creatureCount.ToString("X2");
                output += outputCreatures;
            }

            return output;
        }

        public MapTile()
        {
            Items = new List<MapItem>();
            Creatures = new List<MapCreature>();
        }
    }
}
