using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TibiaDatReader;

namespace TibiaCastRecordingParser
{
    class StructureReader
    {

        public string BaseDirectory
        {
            get;
            set;
        }

        public string DoneDirectory
        {
            get;
            set;
        }

        public string StructureDirectory
        {
            get;
            set;
        }

        public string MapDirectory
        {
            get;
            set;
        }

        public string MetaDirectory
        {
            get;
            set;
        }

        public Dat DatContextInput
        {
            get;
            set;
        }

        public Dat DatContextOutput
        {
            get;
            set;
        }

        public static string[] SPLIT_STRUCTURE_ROW = new string[] { Environment.NewLine };
        public static string[] SPLIT_STRUCTURE_COL = new string[] { "\t" };
        
        // Determines if we allow skipping based on done recordings.
        // False = read all recordings, even old, done recordings.
        public const bool ALLOW_SKIP = true;

        public string[] GetPendingStructures()
        {
            return FilterDoneStructures(GetStructures(), GetDoneStructures());
        }

        static string[] FilterDoneStructures(string[] structures, string[] done)
        {
            List<string> list = new List<string>();

            // Whether or not we found the item.
            bool found;

            // Current search item.
            string searchPath;
            string searchItem;

            for (int i = 0; i < structures.Length; i++)
            {
                found = false;

                searchPath = structures[i];

                searchItem = Path.GetFileNameWithoutExtension(searchPath);

                for (int j = 0; j < done.Length; j++)
                {
                    if (searchItem == Path.GetFileNameWithoutExtension(done[j]))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    list.Add(searchPath);
                }

            }

            return list.ToArray();
        }

        static string[] GetStructures(string directory)
        {
            if (Directory.Exists(directory))
            {
                return Directory.GetFiles(directory);
            }
            return new string[0];
        }

        public string[] GetStructures()
        {
            return GetStructures(StructureDirectory);
        }

        static bool IsDone(string src)
        {
            string text = File.ReadAllText(src);
            float value = 0;
            float.TryParse(text, out value);
            if (value >= Program.DebugVersion)
            {
                return true;
            }
            return false;
        }
        static string[] GetDoneStructures(string directory)
        {
            List<string> done = new List<string>();

            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory);
                for (int i = 0; i < files.Length; i++)
                {
                    if (ALLOW_SKIP && IsDone(files[i]))
                    {
                        done.Add(files[i]);
                    }
                }

                return done.ToArray<string>();
            }
            return new string[0];
        }

        public string[] GetDoneStructures()
        {
            return GetDoneStructures(DoneDirectory);
        }

        static void SaveDone(string destination, string recordingID, float debugVersion)
        {
            // Ensure the directory for done recordings exists.
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            using (FileStream fs = new FileStream(destination + "\\" + recordingID, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(debugVersion.ToString());
                }
            }
        }

        //static Dictionary<int, Dictionary<int, List<MapTile>>> FormatMapData(Dictionary<long, List<MapTile>> data)
        static Dictionary<int, MapView> FormatMapData(Dictionary<long, List<MapTile>> data)
        {
            Dictionary<int, MapView> MasterTable = new Dictionary<int, MapView>();
            //Dictionary<int, Dictionary<int, List<MapTile>>> MasterTable = new Dictionary<int, Dictionary<int, List<MapTile>>>();

            List<MapView> views = new List<MapView>();

            long offset;

            int posx, posy, posz;
            int sposx, sposy, sposz;
            int osposx, osposy, osposz;

            List<MapTile> tiles;

            int filenameId, tileId;
            MapView currList;

            foreach (KeyValuePair<long, List<MapTile>> kvp in data)
            {
                offset = kvp.Key;

                // Position is derived from offset + basepos
                posx = (int)((offset & 0xFFFF));
                posy = (int)(((offset >> 16) & 0xFFFF));
                posz = (int)(((offset >> 32) & 0xF));

                // Filename x, y components
                sposx = (posx / 256) & 0xFF;
                sposy = (posy / 256) & 0xFF;
                //sposx = (posx / 16) & 0xFFF;
                //sposy = (posy / 16) & 0xFFF;
                sposz = (posz) & 0xF;

                filenameId = (sposx) + (sposy << 8) + (sposz << 16);
                //filenameId = (sposx) + (sposy << 12) + (sposz << 24);
                // Check if the dictionary contains a dictionary for the filename
                if (!MasterTable.ContainsKey(filenameId))
                {
                    MasterTable[filenameId] = new MapView(sposx, sposy, sposz);
                    //MasterTable[filenameId] = new Dictionary<int, List<MapTile>>();
                }

                // Offsets from filename components
                osposx = posx - 256 * sposx;
                osposy = posy - 256 * sposy;
                //osposx = posx - 16 * sposx;
                //osposy = posy - 16 * sposy;
                osposz = posz - sposz;

                if (!MasterTable[filenameId].HasTile(osposx, osposy))
                {
                    // Only do one.
                    MapTile tile = kvp.Value[0];

                    MasterTable[filenameId].AddMapTile(osposx, osposy, tile);
                }

            }
            return MasterTable;
        }

        //public bool SaveMap(Dictionary<int, Dictionary<int, List<MapTile>>> data, string recordingID, string outputDirectory)
        public bool SaveMap(Dictionary<int, MapView> data, string recordingID, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            long filenameOffset;

            long offset;

            int oposx, oposy, oposz;

            long tileOffset;
            long toposx, toposy, toposz;
            MapView DictMapTile;

            // Ensure the output directory exists.
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (KeyValuePair<int, MapView> file in data)
            {
                sb.Clear();
                filenameOffset = file.Key;
                toposx = ((long)(filenameOffset) >> 0) & 0xFF;
                toposy = ((long)(filenameOffset) >> 8) & 0xFF;
                toposz = ((long)(filenameOffset) >> 16) & 0xF;
                //toposz = ((long)(0) >> 0) & 0xF;
                //toposx = ((long)(filenameOffset) >> 0) & 0xFF;
                //toposy = ((long)(filenameOffset) >> 12) & 0xFF;
                //toposz = ((long)(filenameOffset) >> 24) & 0xF;

                DictMapTile = file.Value;
                /*
                foreach (KeyValuePair<int, List<MapTile>> tile in DictMapTile)
                {
                    offset = tile.Key;

                    //oposx = (int)((offset >> 0) & 0xFF);
                    //oposy = (int)((offset >> 8) & 0xFF);
                    //oposz = (int)((offset >> 16) & 0xF);
                    oposx = (int)((offset >> 0) & 0xF);
                    oposy = (int)((offset >> 4) & 0xF);
                    oposz = (int)((offset >> 8) & 0xF);

                    //tileOffset = ((long)(((toposx << 8) + oposx) & 0xFFFF) << 0)
                    //           + ((long)(((toposy << 8) + oposy) & 0xFFFF) << 16)
                    //           + ((long)(((toposz << 8) + oposz) & 0xF) << 32);
                    tileOffset = ((long)(((toposx << 4) + oposx) & 0xFFFF) << 0)
                               + ((long)(((toposy << 4) + oposy) & 0xFFFF) << 16)
                               + ((long)(((toposz << 0) + oposz) & 0xF) << 32);

                    List<MapTile> tileList = tile.Value;
                    int i, len;
                    for (i = 0, len = tileList.Count; i < len; i++)
                    {
                        sb.Append(((tileOffset >> 0) & 0xFFFF).ToString("X4"));
                        sb.Append(((tileOffset >> 16) & 0xFFFF).ToString("X4"));
                        sb.Append(((tileOffset >> 32) & 0xFF).ToString("X2"));
                        sb.Append(tileList[i].ToString());
                    }
                }
                 */

                if (!Directory.Exists(outputDirectory + "\\" + filenameOffset.ToString()))
                {
                    Directory.CreateDirectory(outputDirectory + "\\" + filenameOffset.ToString());
                }

                //using (FileStream fs = new FileStream(outputDirectory + "\\" + filenameOffset.ToString() + "\\" + recordingID, FileMode.Create, FileAccess.Write))
                using (FileStream fs = new FileStream(outputDirectory + "\\" + filenameOffset.ToString() + "\\" + recordingID + ".json", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string json = DictMapTile.ToJSON();
                        sw.Write(json);
                        //sw.Write(sb);
                    }
                }
            }
            return true;
        }

        public bool SaveSpawnMap(Dictionary<int, Dictionary<int, List<MapTile>>> data, string recordingID, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            long filenameOffset;

            long offset;

            int oposx, oposy, oposz;

            long tileOffset;
            long toposx, toposy, toposz;
            Dictionary<int, List<MapTile>> DictMapTile;
            foreach (KeyValuePair<int, Dictionary<int, List<MapTile>>> file in data)
            {
                sb.Clear();
                filenameOffset = file.Key;
                toposx = ((long)(filenameOffset) >> 0) & 0xFF;
                toposy = ((long)(filenameOffset) >> 8) & 0xFF;
                toposz = ((long)(0) >> 0) & 0xF;

                DictMapTile = file.Value;
                foreach (KeyValuePair<int, List<MapTile>> tile in DictMapTile)
                {
                    offset = tile.Key;

                    oposx = (int)((offset >> 0) & 0xFF);
                    oposy = (int)((offset >> 8) & 0xFF);
                    oposz = (int)((offset >> 16) & 0xF);

                    //tileOffset = ((long)(((toposx << 8) + oposx) & 0xFFFF)) + ((long)(((toposy << 8) + oposy) & 0xFFFF) << 16) + ((long)((toposz + oposz) & 0xF) << 32);
                    tileOffset = ((long)(((toposx << 8) + oposx) & 0xFFFF) << 0)
                               + ((long)(((toposy << 8) + oposy) & 0xFFFF) << 16)
                               + ((long)(((toposz << 8) + oposz) & 0xF) << 32);

                    //output += oposx.ToString() + "-" + oposy.ToString() + "-" + oposz.ToString() + ":";

                    List<MapTile> tileList = tile.Value;
                    int i, len;
                    string spawnList;
                    for (i = 0, len = tileList.Count; i < len; i++)
                    {
                        spawnList = tileList[i].GetSpawnList();
                        if (spawnList != "")
                        {
                            sb.Append(((tileOffset >> 0) & 0xFFFF).ToString("X4"));
                            sb.Append(((tileOffset >> 16) & 0xFFFF).ToString("X4"));
                            sb.Append(((tileOffset >> 32) & 0xFF).ToString("X2"));
                            sb.Append(spawnList);
                        }
                    }
                }

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                using (FileStream fs = new FileStream(outputDirectory + "\\" + filenameOffset.ToString() + "-" + recordingID, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(sb);
                    }
                }
            }
            return true;
        }

        public void ReadStructuredRecordings(string[] filepaths, float debugVersion)
        {
            Dictionary<int, PacketStructureData[]> structures = LoadStructureFiles(filepaths);

            int recID;
            PacketStructureData[] structure;

            foreach (KeyValuePair<int, PacketStructureData[]> kvp in structures)
            {
                recID = kvp.Key;
                structure = kvp.Value;

                Console.WriteLine("Reading " + recID);

                Dictionary<long, List<MapTile>> MapTileList = ReadRecordingsFromStructure(recID, structure);
                //Dictionary<int, Dictionary<int, List<MapTile>>> mapData = ReadRecordingsFromStructure(recID, datContext, datContextOutput, structure);

                // Format the map data.
                //Dictionary<int, Dictionary<int, List<MapTile>>> mapData = FormatMapData(List);
                Dictionary<int, MapView> mapData = FormatMapData(MapTileList);

                // Save the map data for later use.
                Console.WriteLine("Writing to \\Map...");
                SaveMap(mapData, recID.ToString(), BaseDirectory + "\\MapJson");
                //SaveSpawnMap(mapData, recID.ToString(), BaseDirectory + "\\SpawnMap");

                SaveDone(DoneDirectory, recID.ToString(), debugVersion);

                // Debug: write all the creature data to a text file.
                MapParser.OutputMapCreaturesToFile(BaseDirectory, "creatures." + recID.ToString() + ".log", MapParser.Creatures);
                MapParser.Creatures.Clear();
            }
        }

        static PacketStructureData ParsePacketRow(string row)
        {
            string[] data = row.Split(SPLIT_STRUCTURE_COL, StringSplitOptions.None);

            int type = -1;
            string filename = "";
            int time = -1;
            string pos = "";

            if (data.Length > 1)
            {
                type = Convert.ToInt32(data[0]);

                if (data.Length > 2)
                {
                    filename = data[1];

                    if (data.Length > 3)
                    {
                        time = Convert.ToInt32(data[2]);

                        if (data.Length > 4)
                        {
                            pos = data[3];
                        }
                    }
                }
                return new PacketStructureData(type, filename, time, pos);
            }
            return null;
        }

        static PacketStructureData[] ParsePacketStructure(string input)
        {
            string[] rows = input.Split(SPLIT_STRUCTURE_ROW, StringSplitOptions.None);

            List<PacketStructureData> list = new List<PacketStructureData>();

            PacketStructureData data;

            for (int i = 0; i < rows.Length; i++)
            {
                data = ParsePacketRow(rows[i]);

                if (data != null)
                {
                    list.Add(data);
                }

            }
            return list.ToArray();
        }

        public Dictionary<int, PacketStructureData[]> LoadStructureFiles(string[] structures)
        {
            Dictionary<int, PacketStructureData[]> dict = new Dictionary<int, PacketStructureData[]>();

            List<PacketStructureData[]> structureData = new List<PacketStructureData[]>();

            // Recording ID of the file we're currently working on.
            int filename;

            string filedata;

            // List of maps parsed.
            List<MapFragment> MapList = new List<MapFragment>();

            for (int fileNum = 0, fileCount = structures.Length; fileNum < fileCount; fileNum++)
            {
                // Filename = recording ID
                filename = Convert.ToInt32(Path.GetFileNameWithoutExtension(structures[fileNum]));

                //map = new MapFragment(filename.ToString());
                //MapList.Add(map);

                //Console.WriteLine(filename);

                // Read the .structure file and parse it as a string.
                filedata = Encoding.UTF8.GetString(File.ReadAllBytes(structures[fileNum]));

                PacketStructureData[] packets = ParsePacketStructure(filedata);

                dict[filename] = packets;
            }
            return dict;
        }

        public Dictionary<long, List<MapTile>> ReadRecordingsFromStructure(int recID, PacketStructureData[] packets)
        {
            // Create a MasterList for each recording.
            Dictionary<long, List<MapTile>> MasterList = new Dictionary<long, List<MapTile>>();
            List<MapTile> tiles;
            Dictionary<long, List<int>> tileMap;

            byte[] packetData;

            MapParser parser = new MapParser(DatContextInput, DatContextOutput);
            parser.ErrorSource = this.BaseDirectory + "\\Error.log";
            parser.RecordingID = recID;

            for (int i = 0; i < packets.Length; i++)
            {

                packetData = File.ReadAllBytes(this.BaseDirectory + "\\Packets\\" + packets[i].Type + "\\" + packets[i].Filename);
                //parser.Parse(packetData, packets[i].Type, packets[i].Type != 0xBE && packets[i].Type != 0xBF);
                //parser.Parse(packetData, packets[i].Type, packets[i].Type == 0x64); // OK
                //parser.Parse(packetData, packets[i].Type, packets[i].Type == 0xBF);
                parser.Parse(packetData, packets[i].Type, true);
                //MapParser.Parse(map, packetData, packets[i].type, datContext, datContextOutput, true);
            }

            tiles = parser.Map.Tiles;
            tileMap = parser.Map.TileMap;

            foreach (KeyValuePair<long, List<int>> kvp in tileMap)
            {
                long offset = kvp.Key;
                List<int> TileIndexList = kvp.Value;
                if (!MasterList.ContainsKey(offset))
                {
                    MasterList[offset] = new List<MapTile>();
                }
                for (int k = 0, klen = TileIndexList.Count; k < klen; k++)
                {
                    MasterList[offset].Add(tiles[TileIndexList[k]]);
                }
            }

            // Write the data to a text file so that we can decide
            // later which tiles to use and which to discard.

            return MasterList;
        }
    }
}
