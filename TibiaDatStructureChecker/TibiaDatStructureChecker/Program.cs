using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiaDatStructureChecker
{
    class Program
    {
        static StringBuilder Versions = new StringBuilder();

        static void ExportDatHeader(string src)
        {

            using (FileStream fs = new FileStream(src, FileMode.Open, FileAccess.Read))
            {
                long value = 0;
                int b;

                Versions.Append(src);
                Versions.Append("\t");

                for (int i = 0; i < 4; i++)
                {
                    b = fs.ReadByte();
                    Versions.Append(b.ToString("X2") + " ");
                    value += (b << (i * 8));
                }

                Versions.Append("\t" + value.ToString());
                Versions.Append("\n");

            }
        }

        static void SaveExport(string src)
        {
            using (FileStream fs = new FileStream(src, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(Versions.ToString());
                }
            }
        }
        static void Main(string[] args)
        {
            string[] dirs = Directory.GetDirectories(@"C:\Users\Reece\Recordings\Meta");

            for (int i = 0; i < dirs.Length; i++)
            {
                ExportDatHeader(dirs[i] + "\\" + "Tibia.dat");
            }

            SaveExport(@"C:\Users\Reece\Recordings\DatStruture.exp");
        }
    }
}
