using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiaCastCreatureDataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\Reece\Recordings\NameForID.txt");
            string[] data;

            string cName;
            int cSpeed;
            int cSpeedCount;

            string output = "";

            Dictionary<string, Dictionary<int, int>> CreatureSpeeds = new Dictionary<string, Dictionary<int, int>>();
            for (int i = 0; i < lines.Length; i++)
            {
                data = lines[i].Split('\t');

                cName = data[1];
                cSpeed = Int32.Parse(data[2]);


                if (!CreatureSpeeds.ContainsKey(cName))
                {
                    CreatureSpeeds[cName] = new Dictionary<int, int>();
                }

                if (!CreatureSpeeds[cName].ContainsKey(cSpeed))
                {
                    CreatureSpeeds[cName][cSpeed] = 0;
                }
                CreatureSpeeds[cName][cSpeed]++;
            }

            foreach (KeyValuePair<string, Dictionary<int, int>> cKvp in CreatureSpeeds)
            {
                cName = cKvp.Key;

                foreach (KeyValuePair<int, int> sKvp in cKvp.Value)
                {
                    cSpeed = sKvp.Key;
                    cSpeedCount = sKvp.Value;

                    output += cName + "\t" + cSpeed + "\t" + cSpeedCount + "\n";
                }
            }

            using (FileStream fs = new FileStream(@"C:\Users\Reece\Recordings\CreatureSpeeds.csv", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(output);
                }
            }

        }
    }
}
