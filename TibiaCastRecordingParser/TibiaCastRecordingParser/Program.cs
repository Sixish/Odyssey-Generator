using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TibiaDatReader;

namespace TibiaCastRecordingParser
{
    class Program
    {
        public static float DebugVersion = 0.52F;
        public const string DIRECTORY_BASE = "C:\\Users\\Reece\\Recordings\\Output";


        static void Main(string[] args)
        {
            Int32 uTimeBefore = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // Our base directory.
            string baseDirectory = DIRECTORY_BASE;

            // Where we load the .structure file from.
            string structureDirectory = "";

            // Where we save the map files to.
            string mapDirectory = "";

            // Where we store the metadata (dat etc.).
            string metaDirectory = "";

            // Where we store the time for last completion.
            string doneDirectory = "";

            // Only load every [multiple] recordings.
            int multiple = 1;

            // Indexer for the command line args.
            int argIndex = 0;

            // Search the arg tokens for configuration arguments.
            while (argIndex < args.Length)
            {
                if (args[argIndex] == "-dir")
                {
                    argIndex++;
                    baseDirectory = args[argIndex];
                }
                else if (args[argIndex] == "-donedir")
                {
                    argIndex++;
                    doneDirectory = args[argIndex];
                }
                else if (args[argIndex] == "-multiple")
                {
                    argIndex++;
                    multiple = Int32.Parse(args[argIndex]);
                }
                else
                {
                    // Failed to parse the token at argIndex. Go to next.
                    argIndex++;
                }
            }

            StructureReader sr = new StructureReader();

            foreach (KeyValuePair<long, float> kvp in TibiaCastReader.TibiacastDatVersions)
            {
                float version = kvp.Value;

                if (TibiaCastReader.TibiaDatDirectoryNames.ContainsKey(version))
                {
                    string dirVersion = baseDirectory + "\\" + kvp.Key;

                    // Configuration variables.
                    sr.BaseDirectory = dirVersion;

                    // Structure files.
                    sr.StructureDirectory = dirVersion + "\\Structure";

                    // Map output directory.
                    sr.MapDirectory = dirVersion + "\\Map";

                    // Tibia.dat directory.
                    sr.MetaDirectory = dirVersion + "\\Meta";

                    // Status directory.
                    sr.DoneDirectory = (doneDirectory != "" ? (doneDirectory) : (dirVersion)) + "\\Done";

                    // Load the Tibia.dat file corresponding to the version.
                    // We need to do this first because the structure of the packets
                    // depends on the data inside the file.

                    //Dat.Load(metaDirectory + "\\dat.json");
                    //Dat datContext = Dat.Load(metaDirectory + "\\Tibia.dat");
                    //Dat datContext = Dat.Load(@"C:\Users\Reece\Recordings\Meta\tibia1010\Tibia.dat");
                    sr.DatContextInput = Dat.Load(@"C:\Users\Reece\Recordings\Meta\" + TibiaCastReader.TibiaDatDirectoryNames[kvp.Value] + @"\Tibia.dat", kvp.Value);
                    //sr.DatContextOutput = Dat.Load(@"C:\Users\Reece\Recordings\Meta\tibia1082\Tibia.dat", 10.82F);
                    sr.DatContextOutput = Dat.Load(@"C:\Users\Reece\Recordings\Meta\tibia1090\Tibia.dat", 10.90F);

                    sr.ReadStructuredRecordings(sr.GetPendingStructures(), DebugVersion);
                }
            }

            Int32 uTimeAfter = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Console.WriteLine("Completed in " + (uTimeAfter - uTimeBefore) + " seconds.");
            Console.ReadLine();
        }
    }
}