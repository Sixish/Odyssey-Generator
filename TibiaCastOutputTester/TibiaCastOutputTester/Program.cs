using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiaCastOutputTester
{
    class Program
    {
        static int GetOffsetX(long offset)
        {
            return (int)((offset >> 0) & 0xFFFF);
        }
        static int GetOffsetY(long offset)
        {
            return (int)((offset >> 16) & 0xFFFF);
        }
        static int GetOffsetZ(long offset)
        {
            return (int)((offset >> 32) & 0xFF);
        }

        static long GetOffset(int x, int y, int z)
        {
            // We have to convert the inputs or << will not work properly
            long ox = (long)x, oy = (long)y, oz = (long)z;
            long offset = (ox & 0xFFFF) + ((oy & 0xFFFF) << 16) + ((long)(Convert.ToInt16(oz)) << 32);
            return offset;
        }
        static void AnalyzeFile(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            string[] sepLine;
            long offset, compareOffset;
            int offX, offY, offZ;
            string signature;
            List<string> signatureList;
            List<string> compareSignatureList;
            List<long> dupList;
            string output = "";

            Dictionary<long, List<long>> Duplicates = new Dictionary<long, List<long>>();

            Dictionary<long, List<string>> TileDictionary = new Dictionary<long, List<string>>();

            for (int i = 0, len = lines.Length; i < len; i++)
            {
                sepLine = lines[i].Split('\t');
                offset = long.Parse(sepLine[0]);
                signature = sepLine[1];

                if (!TileDictionary.ContainsKey(offset))
                {
                    TileDictionary[offset] = new List<string>();
                }
                TileDictionary[offset].Add(signature);

            }

            foreach (KeyValuePair<long, List<string>> kvp in TileDictionary)
            {
                offset = kvp.Key;
                signatureList = TileDictionary[offset];

                offX = GetOffsetX(offset);
                offY = GetOffsetY(offset);
                offZ = GetOffsetZ(offset);

                for (int i = 0, len = signatureList.Count; i < len; i++)
                {
                    for (int x = -7; x < 8; x++)
                    {
                        for (int y = -7; y < 8; y++)
                        {
                            compareOffset = GetOffset(offX + x, offY + y, offZ);
                            if (compareOffset != offset)
                            {
                                if (TileDictionary.ContainsKey(compareOffset))
                                {
                                    compareSignatureList = TileDictionary[compareOffset];

                                    for (int j = 0, lenj = compareSignatureList.Count; j < lenj; j++)
                                    {
                                        if (compareSignatureList[j] == signatureList[i])
                                        {
                                            if (!Duplicates.ContainsKey(offset))
                                            {
                                                Duplicates[offset] = new List<long>();
                                            }
                                            Duplicates[offset].Add(compareOffset);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Dictionary<string, int> OffsetValueCount = new Dictionary<string, int>();
            int dx, dy, dz;
            string key;

            foreach (KeyValuePair<long, List<long>> kvp in Duplicates)
            {
                offset = kvp.Key;
                dupList = Duplicates[offset];
                output += offset.ToString() + "\t";
                for (int i = 0, len = dupList.Count; i < len; i++)
                {
                    output += dupList[i].ToString() + " ";

                    dx = (int)(GetOffsetX(offset) - GetOffsetX(dupList[i]));
                    dy = (int)(GetOffsetY(offset) - GetOffsetY(dupList[i]));
                    dz = (int)(GetOffsetZ(offset) - GetOffsetZ(dupList[i]));

                    key = dx + "," + dy + "," + dz;
                    if (!OffsetValueCount.ContainsKey(key))
                    {
                        OffsetValueCount[key] = 0;
                    }
                    OffsetValueCount[key]++;
                }
                output += Environment.NewLine;
            }

            string statsOutput = "";
            
            foreach (KeyValuePair<string, int> kvp in OffsetValueCount)
            {
                statsOutput += kvp.Key + "\t" + kvp.Value + Environment.NewLine;
            }

            using (FileStream fs = new FileStream(@"C:\Users\Reece\Recordings\Duplicates.txt", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(output);
                }
            }

            using (FileStream fs = new FileStream(@"C:\Users\Reece\Recordings\DuplicateOffsetCounts.txt", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(statsOutput);
                }
            }
        }
        static void Main(string[] args)
        {
            AnalyzeFile(@"C:\Users\Reece\Recordings\Tiles.txt");
        }
    }
}
