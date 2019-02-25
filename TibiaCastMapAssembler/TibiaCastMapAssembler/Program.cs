using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TibiaDatReader;
using Newtonsoft.Json;
using TibiaCastRecordingParser;
using System.Drawing;

namespace TibiaCastMapAssembler
{
    class Program
    {
        //public const int START_X = (124 * 256) & 0xFFF0;//124
        public const int START_X = (124 * 256) & 0xFF00;//124
        //public const int START_Y = (121 * 256) & 0xFFF0;//121
        public const int START_Y = (121 * 256) & 0xFF00;//121
        //public const int START_Z = 0;
        public const int START_Z = 0;
        public const int END_X = (132 * 256) & 0xFF00;//131
        public const int END_Y = (130 * 256) & 0xFF00;//129
        public const int END_Z = 16;

        public const int SIZE_X = 256;
        public const int SIZE_Y = 256;
        public const int SIZE_Z = 1;

        public const int IMAGE_SIZE_X = (END_X - START_X) + 1;
        public const int IMAGE_SIZE_Y = (END_Y - START_Y) + 1;

        public const int BITMASK_BASE_X = 0xFF;
        public const int BITMASK_BASE_Y = 0xFF;
        public const int BITMASK_BASE_Z = 0xF;

        static Dictionary<long, Dictionary<string, int>> Map;

        static int bPosition = 0;

        public static Dat DatContext = null;

        static Bitmap[] MapImages = new Bitmap[16];
        static string[] stores = Directory.GetDirectories(@"C:\Users\Reece\Recordings\Output");
        //static string[] stores = new string[] { @"C:\Users\Reece\Recordings\Output\1028" };
        //static string[] stores = new string[] { @"C:\Users\Reece\Recordings\Output\1028" };

        static string GetMapFileName(int fileX, int fileY, int fileZ)
        {
            return (
                ((fileZ >> 0) << 16) +
                ((fileY >> 8) << 8) +
                ((fileX >> 8) << 0)
            ).ToString();
        }

        public static void ReadMapFile(string filename, MapView Map)
        {
            MapView view = MapView.FromJSON(File.ReadAllText(filename));

            byte[] bytes = File.ReadAllBytes(filename);

            bPosition = 0;

            Dictionary<long, bool> DoneOffsets = new Dictionary<long, bool>();

            for (int x = 0; x < MapView.SIZEX; x++)
            {
                for (int y = 0; y < MapView.SIZEY; y++)
                {
                    if (view.HasTile(x, y) && !Map.HasTile(x, y))
                    {
                        // Ensure the Map dictionary has this index.
                        Map.AddMapTile(x, y, view.GetTile(x, y));
                    }
                }
            }
        }

        static Color UnknownMapColor = Color.FromArgb(0xFF, 0x00, 0xFF);
        static Dictionary<int, Color> ItemMapColors = new Dictionary<int, Color>()
        {
            {0,   Color.FromArgb(0x00, 0x00, 0x00)},
            {12,  Color.FromArgb(0x00, 0x66, 0x00)},
            {24,  Color.FromArgb(0x00, 0xCC, 0x00)},
            {30,  Color.FromArgb(0x00, 0xFF, 0x00)},
            {40,  Color.FromArgb(0x33, 0x00, 0xCC)},
            {51,  Color.FromArgb(0x33, 0x00, 0xcc)},
            {86,  Color.FromArgb(0x66, 0x66, 0x66)},
            {114, Color.FromArgb(0x99, 0x33, 0x00)},
            {121, Color.FromArgb(0x99, 0x66, 0x33)},
            {129, Color.FromArgb(0x99, 0x99, 0x99)},
            {140, Color.FromArgb(0x99, 0xff, 0x66)},
            {179, Color.FromArgb(0xcc, 0xff, 0xff)},
            {186, Color.FromArgb(0xff, 0x33, 0x00)},
            {192, Color.FromArgb(0xff, 0x66, 0x00)},
            {207, Color.FromArgb(0xff, 0xcc, 0x99)},
            {210, Color.FromArgb(0xff, 0xff, 0x00)},
            {215, Color.FromArgb(0xff, 0xff, 0xff)}
        };

        static Color GetMapColor(int itemColor)
        {
            if (ItemMapColors.ContainsKey(itemColor))
            {
                return ItemMapColors[itemColor];
            }
            return UnknownMapColor;
        }

        static void GenerateMapFile(int posX, int posY, int posZ, string outDirectory)
        {
            string filename = GetMapFileName(posX, posY, posZ);
            string[] files;
            string file;

            // The map to be filled.
            MapView Maps = new MapView(posX, posY, posZ);
            Bitmap image = MapImages[posZ];

            // Go backward across all the stores; last = most recent.
            for (int s = stores.Length - 1; s >= 0; s--)
            {
                if (Directory.Exists(stores[s] + "\\MapJson") && Directory.Exists(stores[s] + "\\MapJson\\" + filename))
                {
                    files = Directory.GetFiles(stores[s] + "\\MapJson\\" + filename);
                    for (int f = files.Length - 1; f >= 0; f--)
                    {
                        file = files[f];

                        Console.WriteLine("Loading " + filename + " : " + stores[s]);

                        ReadMapFile(file, Maps);
                    }
                }
            }

            if (!Maps.IsEmpty())
            {

                if (!Directory.Exists(outDirectory))
                {
                    Directory.CreateDirectory(outDirectory);
                }

                for (int x = 0; x < SIZE_X; x++)
                {
                    for (int y = 0; y < SIZE_Y; y++)
                    {
                        MapTile t = Maps.GetTile(x, y);
                        if (t != null)
                        {
                            for (int i = 0; i < t.Items.Count; i++)
                            {
                                int id = t.Items[i].ID;
                                DatItem item = DatContext.GetItem(id);
                                if (item.HasMapColor)
                                {
                                    image.SetPixel((posX + x) - START_X, (posY + y) - START_Y, GetMapColor(item.MapColor));
                                }
                            }
                        }
                    }
                }
                using (FileStream fs = new FileStream(outDirectory + "\\" + filename + ".json", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(Maps.ToJSON());
                    }
                }

            }
        }

        static void Main(string[] args)
        {
            Int32 timeStart = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string baseDirectory = @"C:\Users\Reece\Recordings";
            DatContext = Dat.Load(@"C:\Users\Reece\Recordings\Meta\tibia1090\Tibia.dat", 10.90F);

            Map = new Dictionary<long, Dictionary<string, int>>();

            // Create the map files.
            for (int z = START_Z; z < END_Z; z++)
            {
                MapImages[z] = new Bitmap(IMAGE_SIZE_X, IMAGE_SIZE_Y);
            }

            // Ensure the output directory exists.
            //string outDirectory = baseDirectory + "\\ParsedMaps";
            string outDirectory = baseDirectory + "\\JsonMaps";
            if (!Directory.Exists(outDirectory))
            {
                Directory.CreateDirectory(outDirectory);
            }

            for (int x = START_X; x < END_X; x += SIZE_X)
            {
                for (int y = START_Y; y < END_Y; y += SIZE_Y)
                {
                    for (int z = START_Z; z < END_Z; z += SIZE_Z)
                    {
                        GenerateMapFile(x, y, z, outDirectory);
                    }
                }
            }

            // Save the map files.
            if (!Directory.Exists(baseDirectory + "\\PNG"))
            {
                Directory.CreateDirectory(baseDirectory + "\\PNG");
            }

            for (int z = START_Z; z < END_Z; z++)
            {
                Console.WriteLine("Creating PNG: " + z);
                MapImages[z].Save(baseDirectory + "\\PNG\\" + "Map" + z.ToString().PadLeft(2, '0') + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Int32 timeEnd = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            Console.WriteLine("Completed in " + (timeEnd - timeStart) + " seconds.");
            Console.ReadLine();
        }
    }
}
