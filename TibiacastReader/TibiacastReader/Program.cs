using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using System.Dynamic;

namespace TibiacastReader
{
    class Program
    {
        const string DIRECTORY_BASE = "C:\\Users\\Reece\\Recordings";
        const string DIRECTORY_ORIGINAL = DIRECTORY_BASE + "\\Original";
        const string DIRECTORY_OUTPUT = DIRECTORY_BASE + "\\Output";

        const int PACKET_ID_MAPINFO = 0x64;
        const int PACKET_ID_MAPINFO_CONTINUED = 0x6D;

        static string ConstructFilePath(params string[] list)
        {
            string output = list[0];
            for (int i = 1; i < list.Length; i++)
            {
                output += "\\" + list[i];
            }
            return output;
        }

        static void OutputCreaturesToFile(string destination, List<TCCreature> creatures)
        {
            // Create the output directory if it does not exist.
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // Structure the output.
            // TODO consider adding more data to the output?
            Dictionary<long, string> CreatureDictionary = new Dictionary<long, string>();
            for (int c = 0; c < creatures.Count; c++)
            {
                CreatureDictionary[creatures[c].creatureID] = creatures[c].charName;
            }

            string outputString = "";
            foreach (KeyValuePair<long, string> kvp in CreatureDictionary)
            {
                outputString += kvp.Key + "\t" + kvp.Value + Environment.NewLine;
            }

            // We'll write to Creatures.txt within the destination.
            destination = destination + "\\Creatures.txt";

            // Output the text to the file.
            using (FileStream fs = new FileStream(destination, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(outputString);
                }
            }
        }

        static void SaveUnexpectedBlock(string destination, string filename, Stream stream, int size)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            using (FileStream fs = new FileStream(destination + "\\" + filename, FileMode.Create, FileAccess.Write))
            {
                for (int i = 0; i < size; i++)
                {
                    fs.WriteByte((byte)stream.ReadByte());
                }
            }
        }

        static bool StructureFileExists(string baseDirectory, string filename)
        {
            return File.Exists(baseDirectory + "\\Structure\\" + filename + ".structure");
        }

        static void DecompressRecordings(string source, string destination)
        {
            string[] recordings = Directory.GetFiles(source);
            int len = recordings.Length;
            string versionDirectory = "";

            Dictionary<int, string> PacketNames = new Dictionary<int, string>();
            Dictionary<int, Dictionary<string, int>> PacketCounts = new Dictionary<int, Dictionary<string, int>>();
            Dictionary<int, Dictionary<string, List<int>>> PacketSizes = new Dictionary<int, Dictionary<string, List<int>>>();

            PacketNames[PACKET_ID_MAPINFO] = "MapInfoInit";
            PacketNames[PACKET_ID_MAPINFO_CONTINUED] = "MapInfoCont";

            PacketNames[0x0A] = "UK1";
            PacketNames[0x0F] = "UK2";
            PacketNames[0x17] = "UK3";
            PacketNames[0x1D] = "UK4";
            PacketNames[0x65] = "UK5";
            PacketNames[0x66] = "UK6";
            PacketNames[0x67] = "UK7";
            PacketNames[0x68] = "UK8";
            PacketNames[0x69] = "UK9";
            PacketNames[0x6A] = "UK10";
            PacketNames[0x6B] = "UK11";
            PacketNames[0x6C] = "UK12";
            PacketNames[0x6E] = "UK13";
            PacketNames[0x6F] = "UK14";
            PacketNames[0x78] = "UK15";
            PacketNames[0x79] = "UK16";
            PacketNames[0x82] = "UK17";
            PacketNames[0x83] = "UK18";
            PacketNames[0x85] = "UK19";
            PacketNames[0x8C] = "UK20";
            PacketNames[0x8D] = "UK21";
            PacketNames[0x8E] = "UK22";
            PacketNames[0x8F] = "UK23";
            PacketNames[0x90] = "UK24";
            PacketNames[0x92] = "UK25";
            PacketNames[0x93] = "UK26";
            PacketNames[0x94] = "UK27";
            PacketNames[0x9F] = "UK28";
            PacketNames[0xA0] = "UK29";
            PacketNames[0xA1] = "UK30";
            PacketNames[0xA2] = "UK31";
            PacketNames[0xA4] = "UK32";
            PacketNames[0xA5] = "UK33";
            PacketNames[0xA6] = "UK34";
            PacketNames[0xA7] = "UK35";
            PacketNames[0xAA] = "UK36";
            PacketNames[0xB3] = "UK37";
            PacketNames[0xB4] = "UK38";
            PacketNames[0xB7] = "UK39";
            PacketNames[0xB8] = "UK40";
            PacketNames[0xBE] = "UK41";
            PacketNames[0xBF] = "UK42";
            PacketNames[0xF3] = "UK43";
            PacketNames[0xF5] = "UK44";
            // New
            PacketNames[0x70] = "UK45";
            PacketNames[0x71] = "UK46";
            PacketNames[0x91] = "UK47";
            PacketNames[0x72] = "UK48";
            // Newer
            PacketNames[0x7D] = "UK49";
            PacketNames[0x7E] = "UK50";
            PacketNames[0x7F] = "UK51";
            // Even newer
            PacketNames[0x7B] = "UK52";
            PacketNames[0x7C] = "UK53";

            //List<int> RecordingPackets = new List<int> { 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6E, 0xAA, 0xB4, 0xBE, 0xBF, 0xF3, 0xF5 };
            List<int> RecordingPackets = new List<int> { 0x64, 0x65, 0x66, 0x67, 0x68, 0xBE, 0xBF };

            Position pos = new Position(0, 0, 0);

            Dictionary<string, Dictionary<int, int>> UnexpectedBlockTypeCount = new Dictionary<string, Dictionary<int, int>>();

            TCRecording recording = new TCRecording(-1);

            for (int i = 0; i < len; i++)
            {
                try
                {
                    string filename = Path.GetFileNameWithoutExtension(recordings[i]);
                    string recordingID = filename.Split('.')[0];

                    recording = new TCRecording(Convert.ToInt32(recordingID));

                    using (MemoryStream ms = recording.Decompress(recordings[i]))
                    {
                        versionDirectory = destination + "\\" + recording.Version;
                        if (!Directory.Exists(versionDirectory))
                        {
                            Directory.CreateDirectory(versionDirectory);
                        }
                        // TEMPORARY TO TEST FILE HEADERS.
                        if (StructureFileExists(versionDirectory, filename))
                        {
                            // TODO refactor. Do not like this.
                            Console.WriteLine("Skipped because already done: " + filename);
                            continue;
                        }

                        ms.Position = 0;

                        using (FileStream debugOutFs = new FileStream(versionDirectory + "\\" + "debug-" + filename + ".recording", FileMode.Create, FileAccess.Write))
                        {
                            ms.CopyTo(debugOutFs);
                        }
                        ms.Position = 0;

                        Console.WriteLine("Reading Recording #" + filename);
                        string txtOutput = "";
                        while (ms.Position < ms.Length)
                        {
                            TCBlock block = TCBlock.ReadBlock(ms);
                            MemoryStream ms_block = block.Stream;
                            if (block.blockSize > 0)
                            {

                                List<TCCreature> creatures;

                                if (block.blockType == TCBlock.BLOCK_TYPE_INIT)
                                {
                                    // The initial block has an array of creatures before the
                                    //   array of packets. Parse the creatures and handle the
                                    //   packets as we do for the packet block.

                                    creatures = Packet.ParseCreatureList(ms_block, recording.Version);

                                    // For analysis, output the creatures to a text file.

                                    // Ensure the directory exists.
                                    string creaturesDirectory = ConstructFilePath(versionDirectory, "Extra");

                                    // Output to file.
                                    OutputCreaturesToFile(creaturesDirectory, creatures);

                                }

                                if (block.blockType == TCBlock.BLOCK_TYPE_TIBIACASTMESSAGE)
                                {
                                    ms_block.Position += block.blockSize - 1;
                                }

                                if (block.blockType == TCBlock.BLOCK_TYPE_UNKNOWN)
                                {
                                    ms_block.Position += block.blockSize - 1;
                                }

                                if (!TCBlock.ExpectedBlockTypes.Contains(block.blockType))
                                {
                                    // Either we encountered an error, or the block type is not supported.
                                    Console.WriteLine("UnexpectedType=" + block.blockType.ToString());

                                    // Increment the counter to generate a unique filename.
                                    if (!UnexpectedBlockTypeCount.ContainsKey(filename))
                                    {
                                        UnexpectedBlockTypeCount[filename] = new Dictionary<int, int>();
                                    }
                                    if (!UnexpectedBlockTypeCount[filename].ContainsKey(block.blockType))
                                    {
                                        UnexpectedBlockTypeCount[filename][block.blockType] = 0;
                                    }
                                    UnexpectedBlockTypeCount[filename][block.blockType]++;

                                    // The size is part of the block, so go back 1 byte.
                                    ms_block.Position -= 1;

                                    SaveUnexpectedBlock(versionDirectory + "\\UnexpectedBlocks\\", filename + "-" + block.blockType + "-" + UnexpectedBlockTypeCount[filename][block.blockType] + ".blk", ms_block, block.blockSize);
                                }

                                if (block.blockType == TCBlock.BLOCK_TYPE_INIT || block.blockType == TCBlock.BLOCK_TYPE_PACKETS)
                                {
                                    // Handle the array of packets.
                                    int count = 0;

                                    for (int bCounter = 0; bCounter < 2; bCounter++)
                                    {
                                        count += ms_block.ReadByte() << (8 * bCounter);
                                    }

                                    for (int bCounter = 0; bCounter < count; bCounter++)
                                    {
                                        Packet packet = Packet.ReadPacket(ms_block);

                                        int packetType = packet.packetType;

                                        if (PacketNames.ContainsKey(packetType))
                                        {
                                            // For debugging purposes, track the character's position
                                            // within the map.

                                            if (packetType == Packet.ID_MAP_WHOLE)
                                            {
                                                pos = Packet.GetMapPosition(packet);
                                            }
                                            else if (packetType == Packet.ID_MAP_NORTH)
                                            {
                                                pos.y--;
                                            }
                                            else if (packetType == Packet.ID_MAP_EAST)
                                            {
                                                pos.x++;
                                            }
                                            else if (packetType == Packet.ID_MAP_SOUTH)
                                            {
                                                pos.y++;
                                            }
                                            else if (packetType == Packet.ID_MAP_WEST)
                                            {
                                                pos.x--;
                                            }
                                            else if (packetType == Packet.ID_MAP_DOWN)
                                            {
                                                pos.z++;
                                            }
                                            else if (packetType == Packet.ID_MAP_UP)
                                            {
                                                pos.z--;
                                            }

                                            // Position = 1 (we don't care for the header
                                            // at index 0).
                                            //ms_packet.Position = 1;

                                            string dir = ConstructFilePath(versionDirectory + "\\PacketDumpTest", PacketNames[packetType]);
                                            if (!Directory.Exists(dir))
                                            {
                                                Directory.CreateDirectory(dir);
                                            }
                                            if (!PacketCounts.ContainsKey(packetType))
                                            {
                                                PacketCounts[packetType] = new Dictionary<string, int>();
                                            }
                                            if (!PacketCounts[packetType].ContainsKey(filename))
                                            {
                                                PacketCounts[packetType][filename] = 0;
                                            }
                                            PacketCounts[packetType][filename]++;

                                            // Packet Sizes
                                            if (!PacketSizes.ContainsKey(packetType))
                                            {
                                                PacketSizes[packetType] = new Dictionary<string, List<int>>();
                                            }
                                            if (!PacketSizes[packetType].ContainsKey(filename))
                                            {
                                                PacketSizes[packetType][filename] = new List<int>();
                                            }
                                            PacketSizes[packetType][filename].Add((int)packet.data.Length);

                                            // Record the data
                                            if (RecordingPackets.IndexOf(packetType) != -1)
                                            {
                                                string packetDirectory = versionDirectory;
                                                if (!Directory.Exists(packetDirectory))
                                                {
                                                    Directory.CreateDirectory(packetDirectory);
                                                }

                                                packetDirectory = ConstructFilePath(packetDirectory, "Packets");
                                                if (!Directory.Exists(packetDirectory))
                                                {
                                                    Directory.CreateDirectory(packetDirectory);
                                                }

                                                packetDirectory = ConstructFilePath(packetDirectory, packetType.ToString());
                                                if (!Directory.Exists(packetDirectory))
                                                {
                                                    Directory.CreateDirectory(packetDirectory);
                                                }

                                                string fn = filename + "-" + PacketCounts[packetType][filename].ToString() + ".tpk";
                                                using (FileStream fs = new FileStream(ConstructFilePath(packetDirectory, fn), FileMode.Create, FileAccess.Write))
                                                {
                                                    fs.Write(packet.data, 0, packet.data.Length);
                                                    txtOutput += packetType.ToString() + "\t" + fn + "\t" + block.blockTime.ToString() + "\t" + pos.x + ", " + pos.y + ", " + pos.z + "\t" + Environment.NewLine;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Could not find PacketNames dictionary entry for key=" + packetType.ToString());
                                        }
                                    }
                                }
                                //ms_block.Position = 0;
                            }
                            else
                            {
                                Console.WriteLine("Skipped a block.");
                            }
                        }
                        if (txtOutput != "")
                        {
                            // Structure file.

                            // Create the /Structure directory, if it doesn't exist.
                            string structureDirectory = versionDirectory + "\\Structure";
                            if (!Directory.Exists(structureDirectory))
                            {
                                Directory.CreateDirectory(structureDirectory);
                            }
                            using (FileStream fs = new FileStream(ConstructFilePath(structureDirectory, filename + ".structure"), FileMode.Create, FileAccess.Write))
                            {
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.Write(txtOutput);
                                    txtOutput = "";
                                }
                            }
                        }
                        Console.WriteLine("Finished reading recording.");
                        // Record the PacketSizes for future references
                        string packetSizeDirectory = ConstructFilePath(versionDirectory, "PacketSizes");
                        if (!Directory.Exists(packetSizeDirectory))
                        {
                            Directory.CreateDirectory(packetSizeDirectory);
                        }
                        foreach (KeyValuePair<int, Dictionary<string, List<int>>> packet in PacketSizes)
                        {
                            int packetID = packet.Key;
                            List<int> sizes = new List<int>();
                            Dictionary<string, List<int>> filedata = packet.Value;
                            foreach (KeyValuePair<string, List<int>> packetSizesInFile in filedata)
                            {
                                sizes.AddRange(packetSizesInFile.Value);
                            }
                            using (FileStream fs = new FileStream(ConstructFilePath(packetSizeDirectory, packetID.ToString() + ".pktsz"), FileMode.Create, FileAccess.Write))
                            {
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.Write(String.Join("\n", sizes.ToArray()));
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    string dir = destination + "\\" + recording.Version + "\\FailedDecompression";
                    string dest = dir + "\\" + Path.GetFileName(recordings[i]);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    if (recordings[i] != dest)
                    {
                        using (FileStream fs = new FileStream(recordings[i], FileMode.Open, FileAccess.Read))
                        {
                            using (FileStream fsOut = new FileStream(dest, FileMode.Create, FileAccess.Write))
                            {
                                fs.CopyTo(fsOut);
                            }
                        }
                    }
                    //Console.WriteLine(e);
                    Console.WriteLine("Failed to decompress recording " + recordings[i] + ". Stream might be corrupted.");
                }
            }
        }
        static void Main(string[] args)
        {
            Int32 timeStart = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string directory = DIRECTORY_ORIGINAL;
            string outDirectory = DIRECTORY_OUTPUT;
            int i = 0;

            if (args.Length > 0)
            {
                while (i < args.Length)
                {
                    if (args[i] == "-dir")
                    {
                        i++;
                        directory = args[i];
                    }
                    else if (args[i] == "-outdir")
                    {
                        i++;
                        outDirectory = args[i];
                    }
                    else
                    {
                        // Ignored token.
                        i++;
                    }
                }
            }

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Could not find directory for recordings.");
                Console.ReadLine();
                return;
            }

            //directory = @"C:\Users\Reece\Recordings\Output\FailedDecompression";
            // Decompress the recordings.
            DecompressRecordings(directory, outDirectory);

            Int32 timeEnd = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Console.WriteLine("Finished all recordings in " + (timeEnd - timeStart) + " seconds.");
            Console.ReadLine();
        }
    }
}
