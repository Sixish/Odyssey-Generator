using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCastRecordingParser
{
    public class MapFragment
    {
        public List<MapTile> Tiles
        {
            get;
            set;
        }

        public Dictionary<long, List<int>> TileMap
        {
            get;
            set;
        }

        public void AddTile(Position3D position, MapTile tile)
        {
            long offset = MapTile.GetOffset(position);
            int index = Tiles.Count;

            // Only record the first tile for efficiency.
            if (!TileMap.ContainsKey(offset))
            {
                TileMap[offset] = new List<int>();
                Tiles.Add(tile);
                TileMap[offset].Add(index);
            }
        }

        public MapFragment()
        {
            Tiles = new List<MapTile>();
            TileMap = new Dictionary<long, List<int>>();
        }
    }
}
