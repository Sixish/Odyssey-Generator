using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DatReader
{
    class Program
    {
        static long bPosition = 0;

        static Dictionary<int, int> FlagValueSize = new Dictionary<int, int>()
        {
            //{ 0x00, 0 },
            { 0x00, 1 },
            { 0x08, 1 },
            { 0x09, 1 },
            { 0x0C, 0 },
            { 0x0D, 0 },
            { 0x0E, 0 },
            //{ 0x15, 2 },
            { 0x16, 2 },

            { 0x1F, 0 },
            { 0x19, 2 },
            { 0x1A, 1 },
            { 0x1D, 1 },
            //{ 0x1E, 2 },
            { 0x1E, 1 },
            { 0x21, 1 },
            { 0x23, 1 }
        };

        static void OutputHexDump(string filepath, string destination)
        {
            StringBuilder sb = new StringBuilder();

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                int val;
                while ((val = fs.ReadByte()) != -1)
                {
                    sb.Append(val.ToString("X2") + " ");
                }
            }

            using (FileStream fs = new FileStream(destination, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sb);
                }
            }
        }
        static void Main(string[] args)
        {
            string src;
            string dst;
            //src = @"C:\Users\Reece\Downloads\tibia1022\tibia1082\Tibia.dat";
            src = @"C:\Users\Reece\Downloads\tibia1022\tibia1022\Tibia.dat";

            dst = @"C:\Users\Reece\Recordings\Dat.dump";
            OutputHexDump(src, dst);
            Dat dat = Dat.Load(src);
            Console.WriteLine(dat);
            Console.ReadLine();
       }
    }
}