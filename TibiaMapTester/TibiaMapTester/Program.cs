using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TibiaDatReader;
using Newtonsoft.Json;
using TibiaCastRecordingParser;

namespace TibiaMapTester
{
    class Program
    {
        static Dat dat;

        public static StringBuilder DebugDump = new StringBuilder();

        public static int StartX = 124;//128;
        public static int StartY = 121;//125;
        public static int StartZ = 7;//7;

        public static int EndX = 132;
        public static int EndY = 130;
        public static int EndZ = 8;

        public static int BASELINE_ADD_MARK = 1;

        static HashSet<long> TileWalkedStates = new HashSet<long>();
        static HashSet<long> TileWalkableStates = new HashSet<long>();
        static HashSet<long> TileExploredStates = new HashSet<long>();
        static Dictionary<long, int> TileWalkValue = new Dictionary<long, int>();

        static TibiaMapFile ParseOdysseyMap(int baseX, int baseY, int baseZ)
        {
            TibiaMapFile mapFile = new TibiaMapFile(baseX, baseY, baseZ);
            List<string> filesToUse = new List<string>();

            // Construct the map.
            filesToUse = new List<string>();
            if (File.Exists(@"C:\Users\Reece\Recordings\JsonMaps\" + ((baseZ << 16) + (baseY << 8) + (baseX << 0)).ToString() + ".json"))
            {
                filesToUse.Add(@"C:\Users\Reece\Recordings\JsonMaps\" + ((baseZ << 16) + (baseY << 8) + (baseX << 0)).ToString() + ".json");
            }

            for (int n = 0; n < filesToUse.Count; n++)
            {
                MapView view = MapView.FromJSON(File.ReadAllText(filesToUse[n]));
                for (int x = 0; x < 256; x++)
                {
                    for (int y = 0; y < 256; y++)
                    {
                        MapTile tile = view.GetTile(x, y);
                        if (tile != null)
                        {

                            for (int i = 0; i < tile.Items.Count; i++)
                            {
                                DatItem dItem = dat.GetItem(tile.Items[i].ID);
                                if (dItem.IsStackable)
                                {
                                    // Map files don't care for stack count.
                                }
                                if (dItem.IsFluid)
                                {
                                    // Map files don't care for fluids.
                                }
                                if (dItem.IsFluidContainer)
                                {
                                    // Map files don't care for fluid containers.
                                }
                                if (dItem.IsAnimated)
                                {
                                    // Map files don't care for sprite animations.
                                }

                                if (dItem.HasMapColor)
                                {
                                    // Set the map color for the position.
                                    mapFile.SetMapColor(x, y, dItem.MapColor);
                                }

                                if (dItem.IsGround)
                                {
                                    mapFile.SetSpeed(x, y, dItem.Speed);
                                    mapFile.SetHasTile(x, y, true);
                                }

                                if (dItem.IsBlocking || dItem.BlocksPath)
                                {
                                    mapFile.SetIsUnwalkable(x, y, true);
                                }
                            }

                        }
                    }
                }

            }

            Console.WriteLine("Done " + TibiaMapFile.GetTibiaMapFileName(baseX, baseY, baseZ));

            return mapFile;
        }

        public static long GetPositionOffset(int posx, int posy, int posz)
        {
            return (
                (((long)posx & 0xFFFF) << 0) +
                (((long)posy & 0xFFFF) << 16) +
                (((long)posz & 0xFF) << 24)
            );
        }

        public static void SetRangeIsExplored(int sx, int sy, int sz, int ex, int ey, int ez)
        {
        }

        public static bool GetIsWalked(int posx, int posy, int posz)
        {
            long posOffset = GetPositionOffset(posx, posy, posz);
            return TileWalkedStates.Contains(posOffset);
        }

        public static void SetIsWalked(int posx, int posy, int posz)
        {
            long posOffset = GetPositionOffset(posx, posy, posz);
            if (!TileWalkedStates.Contains(posOffset))
            {
                TileWalkedStates.Add(posOffset);
            }
        }

        public static void GetWalkedTilesFromStructureFile(string src)
        {
            string[] rows = File.ReadAllLines(src);
            string[] fields;
            string[] pos;
            int posx, posy, posz;
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    fields = rows[i].Split('\t');
                    // Ensure we have a position (fields[3]).
                    if (fields.Length >= 4)
                    {
                        pos = fields[3].Split(',');
                        if (pos.Length == 3)
                        {
                            posx = Int32.Parse(pos[0]);
                            posy = Int32.Parse(pos[1]);
                            posz = Int32.Parse(pos[2]);

                            if (!GetIsWalked(posx, posy, posz))
                            {
                                SetIsWalked(posx, posy, posz);
                                int sx = posx - MapParser.POSITION_VIEWPORT_OFFSET_X;
                                int sy = posy - MapParser.POSITION_VIEWPORT_OFFSET_Y;
                                int sz = (posz <= 7 ? 0 : (posz - 2));

                                int ex = posx + MapParser.POSITION_VIEWPORT_OFFSET_X;
                                int ey = posy + MapParser.POSITION_VIEWPORT_OFFSET_Y;
                                int ez = (posz <= 7 ? 7 : (posz + 2));

                                int oz;
                                for (int x = sx; x <= ex; x++)
                                {
                                    for (int y = sy; y <= ey; y++)
                                    {
                                        for (int z = sz; z <= ez; z++)
                                        {
                                            oz = (z - posz);
                                            if (!TileExploredStates.Contains(GetPositionOffset(x + oz, y + oz, z)))
                                            {
                                                TileExploredStates.Add(GetPositionOffset(x + oz, y + oz, z));
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            // This was unexpected.
                            // Position doesn't have exactly 3 fields (x,y,z)?
                            Console.WriteLine("Position is invalid.");
                        }
                    }
                }
            }

        }

        public static void GetWalkedTilesFromStructures(string[] directories)
        {
            string[] filepaths;
            for (int i = 0; i < directories.Length; i++)
            {
                if (Directory.Exists(directories[i] + "\\Structure"))
                {
                    filepaths = Directory.GetFiles(directories[i] + "\\Structure");
                    for (int j = 0; j < filepaths.Length; j++)
                    {
                        GetWalkedTilesFromStructureFile(filepaths[j]);
                    }
                }
            }
        }

        public static void GetWalkedTiles(string[] directories)
        {
            GetWalkedTilesFromStructures(directories);
        }

        public static bool LoadWalkedTiles(string baseDirectory)
        {
            string[] directories = Directory.GetDirectories(baseDirectory);
            GetWalkedTiles(directories);
            return true;
        }

        public static TibiaMapFile[] GenerateWalkableTilesFromMapFiles(string outputDirectory)
        {
            // List to populate and return.
            List<TibiaMapFile> list = new List<TibiaMapFile>();

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            int x, y, z;
            for (z = 0; z < 16; z++)
            //for (z = 0; z < 16; z++)
            {
                //for (x = 124; x < 132; x++)
                for (x = (StartX - 1); x < (EndX - 1); x++)
                {
                    //for (y = 121; y < 130; y++)
                    for (y = (StartY - 1); y < (EndY + 1); y++)
                    {
                        TibiaMapFile mapFile = ParseOdysseyMap(x, y, z);
                        //mapFile.Save(outputDirectory);
                        GetWalkableTiles(mapFile);
                    }
                }
            }

            return list.ToArray<TibiaMapFile>();
        }

        static void GetWalkableTiles(TibiaMapFile map)
        {
            int posx, posy, posz;
            long posOffset;

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    posx = (map.BaseX << 8) + x;
                    posy = (map.BaseY << 8) + y;
                    posz = (map.BaseZ);

                    posOffset = GetPositionOffset(posx, posy, posz);
                    if (map.GetHasTile(x, y) && !map.GetIsUnwalkable(x, y) && !TileWalkableStates.Contains(posOffset))
                    {
                        TileWalkableStates.Add(posOffset);
                    }
                }
            }
        }

        public static bool IsReachable(int posx, int posy, int posz)
        {
            if (TileWalkedStates.Contains(GetPositionOffset(posx - 1, posy - 1, posz)))
            {
                return true;
            }

            if (TileWalkedStates.Contains(GetPositionOffset(posx, posy - 1, posz)))
            {
                return true;
            }

            if (TileWalkedStates.Contains(GetPositionOffset(posx + 1, posy - 1, posz)))
            {
                return true;
            }


            if (TileWalkedStates.Contains(GetPositionOffset(posx - 1, posy, posz)))
            {
                return true;
            }


            if (TileWalkedStates.Contains(GetPositionOffset(posx + 1, posy, posz)))
            {
                return true;
            }


            if (TileWalkedStates.Contains(GetPositionOffset(posx - 1, posy + 1, posz)))
            {
                return true;
            }

            if (TileWalkedStates.Contains(GetPositionOffset(posx, posy + 1, posz)))
            {
                return true;
            }

            if (TileWalkedStates.Contains(GetPositionOffset(posx + 1, posy + 1, posz)))
            {
                return true;
            }

            return false;
        }

        public static void MarkHighValueUnwalkedTiles(TibiaMapFile map)
        {
            int StartX = 0;
            int StartY = 0;

            int EndX = 256;
            int EndY = 256;

            int StartOffsetX = -TibiaCastRecordingParser.MapParser.POSITION_VIEWPORT_OFFSET_X;
            int StartOffsetY = -TibiaCastRecordingParser.MapParser.POSITION_VIEWPORT_OFFSET_Y;
            int EndOffsetX = TibiaCastRecordingParser.MapParser.POSITION_VIEWPORT_OFFSET_X;
            int EndOffsetY = TibiaCastRecordingParser.MapParser.POSITION_VIEWPORT_OFFSET_Y;
            int StartOffsetZ;
            int EndOffsetZ;

            long posOffset;
            long targetPosOffset;
            int value;

            bool mapChanged = false;

            StartOffsetZ = (map.BaseZ <= 7 ? 0 : Math.Max(0, map.BaseZ - 2));
            EndOffsetZ = (map.BaseZ <= 7 ? 7 : Math.Min(15, map.BaseZ + 2));

            StartOffsetZ = (map.BaseZ <= 7 ? (-map.BaseZ) : -2);
            EndOffsetZ = (map.BaseZ <= 7 ? (7 - map.BaseZ) : Math.Min(15 - map.BaseZ, 2));

            for (int x = StartX; x < EndX; x++)
            {
                for (int y = StartY; y < EndY; y++)
                {
                    posOffset = GetPositionOffset((map.BaseX << 8) + x, (map.BaseY << 8) + y, map.BaseZ);

                    if (TileWalkableStates.Contains(posOffset) && (!TileWalkedStates.Contains(posOffset)))
                    {
                        value = 0;
                        DebugDump.Append("Position: ");
                        DebugDump.Append(((map.BaseX << 8) + x).ToString());
                        DebugDump.Append(", ");
                        DebugDump.Append(((map.BaseY << 8) + y).ToString());
                        DebugDump.Append(", ");
                        DebugDump.Append(map.BaseZ.ToString());
                        DebugDump.Append(Environment.NewLine);

                        //33418, 32038, 7 :
                        //  33410, 32032, 0
                        //  33426, 32044, 0
                        DebugDump.Append(" Min: ");
                        DebugDump.Append((((map.BaseX << 8) + x) + (StartOffsetZ) + StartOffsetX).ToString());
                        DebugDump.Append(", ");
                        DebugDump.Append((((map.BaseY << 8) + y) + (StartOffsetZ) + StartOffsetY).ToString());
                        DebugDump.Append(", ");
                        DebugDump.Append((((map.BaseZ << 0) + 0) + StartOffsetZ).ToString());
                        DebugDump.Append(Environment.NewLine);

                        DebugDump.Append(" Max: ");
                        DebugDump.Append((((map.BaseX << 8) + x) + (EndOffsetZ) + EndOffsetX).ToString());
                        DebugDump.Append(", ");
                        DebugDump.Append((((map.BaseY << 8) + y) + (EndOffsetZ) + EndOffsetY).ToString());
                        DebugDump.Append(", ");
                        DebugDump.Append((((map.BaseZ << 0) + 0) + (EndOffsetZ)).ToString());
                        DebugDump.Append(Environment.NewLine);

                        for (int offX = StartOffsetX; offX <= EndOffsetX; offX++)
                        {
                            for (int offY = StartOffsetY; offY <= EndOffsetY; offY++)
                            {
                                for (int offZ = StartOffsetZ; offZ <= EndOffsetZ; offZ++)
                                {
                                    // TODO TEST
                                    targetPosOffset = GetPositionOffset(
                                        ((map.BaseX << 8) + x) + (offZ) + offX,
                                        ((map.BaseY << 8) + y) + (offZ) + offY,
                                        ((map.BaseZ << 0) + 0) + (offZ)
                                    );

                                    if (!TileExploredStates.Contains(targetPosOffset))
                                    {
                                        if (IsReachable(((map.BaseX << 8) + x) + (offZ) + offX, ((map.BaseY << 8) + y) + (offZ) + offY, ((map.BaseZ << 0) + 0) + (offZ)))
                                        {
                                            DebugDump.Append("  Can reveal: ");
                                            DebugDump.Append((((map.BaseX << 8) + x) + (offZ) + offX).ToString());
                                            DebugDump.Append(", ");
                                            DebugDump.Append((((map.BaseY << 8) + y) + (offZ) + offY).ToString());
                                            DebugDump.Append(", ");
                                            DebugDump.Append((((map.BaseZ << 0) + 0) + (offZ)).ToString());
                                            DebugDump.Append(Environment.NewLine);

                                            value++;
                                        }

                                    }
                                }
                            }
                        }

                        TileWalkValue[posOffset] = value;
                        if (value > BASELINE_ADD_MARK)
                        {
                            map.AddMapMarker(x, y, 14, "Value: " + value.ToString());
                            mapChanged = true;
                        }
                    }
                }
            }

            if (mapChanged || true)
            {
                map.Save(@"C:\Users\Reece\Recordings\MarkedMaps");
                LogDump();
            }
        }

        public static void FindHighValueUnwalkedTiles(string outDirectory)
        {
            int x, y, z;
            for (z = StartZ; z < EndZ; z++)
            //for (z = 0; z < 16; z++)
            {
                //for (x = 124; x < 132; x++)
                for (x = StartX; x < EndX; x++)
                {
                    for (y = StartY; y < EndY; y++)
                    //for (y = 121; y < 122; y++)
                    {
                        TibiaMapFile mapFile = ParseOdysseyMap(x, y, z);
                        MarkHighValueUnwalkedTiles(mapFile);
                    }
                }
            }

        }

        public static void LogDump()
        {
            using (FileStream fs = new FileStream(@"C:\Users\Reece\Recordings\Log.txt", FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(DebugDump.ToString());
                DebugDump.Clear();
            }
        }

        static void Main(string[] args)
        {
            dat = Dat.Load(@"C:\Users\Reece\Recordings\Meta\tibia1090\tibia.dat", 10.90F);

            string baseDirectory = @"C:\Users\Reece\Recordings\Output";

            string outputDirectory = @"C:\Users\Reece\Recordings\OutputMapFiles";

            string explorerOutputDirectory = @"C:\Users\Reece\Recordings\MarkedMaps";

            GenerateWalkableTilesFromMapFiles(outputDirectory);

            LoadWalkedTiles(baseDirectory);
            FindHighValueUnwalkedTiles(explorerOutputDirectory);

            LogDump();
        }
    }
}
