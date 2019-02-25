using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaDatReader;
using Newtonsoft.Json;
using System.IO;

namespace TibiaDatJsonCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            Dat dat = Dat.Load(@"C:\Users\Reece\Recordings\Meta\tibia1090\Tibia.dat", 10.90F);
            string s = JsonConvert.SerializeObject(dat);
            using (FileStream fs = new FileStream(@"C:\Users\Reece\Recordings\Meta\tibia1090\Tibia.dat.json", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(s);
                }
            }
        }
    }
}
