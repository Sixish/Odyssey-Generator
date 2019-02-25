using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace TibiaDataFetcher
{
    class SpriteSheet
    {
        public string src;
        public int from;
        public int to;
        public int size;

        public SpriteSheet(string src, int from, int to, int size)
        {
            this.src = src;
            this.from = from;
            this.to = to;
            this.size = size;
        }
    }
    class Program
    {
        public const string URL_CATALOG_CONTENT = "https://secure.tibia.com/flash-regular-bin/catalog-content.xml";
        public const string CATALOG_FILENAME = "catalog-content.xml";
        public const string TIBIA_RESOURCE_PREFIX = "http://static.tibia.com/flash-regular-data/";
        public static bool HandleSpriteSheets(List<SpriteSheet> list)
        {
            if (!Directory.Exists("Resources"))
            {
                Directory.CreateDirectory("Resources");
            }
            using (WebClient wClient = new WebClient())
            {
                // Iterate over the SpriteSheets
                int i, len = list.Count;
                string destinationFilename;
                SpriteSheet sheet;
                wClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0");
                for (i = 0; i < len; i++)
                {
                    sheet = list[i];
                    if (sheet.src != "")
                    {
                        if (File.Exists("Resources/" + sheet.src))
                        {
                            Console.WriteLine("Skipping " + sheet.src);
                        }
                        else
                        {
                            wClient.DownloadFile(TIBIA_RESOURCE_PREFIX + sheet.src, "Resources/" + sheet.src);
                            Console.WriteLine("Downloaded: #" + i.ToString() + " (" + sheet.src + ")");
                        }
                    }
                }
            }
            return true;
        }
        static void Main(string[] args)
        {
            using (WebClient wClient = new WebClient())
            {
                // Results
                List<SpriteSheet> results = new List<SpriteSheet>();

                // Download the catalog
                wClient.DownloadFile(URL_CATALOG_CONTENT, CATALOG_FILENAME);

                // Read the catalog
                string data = Encoding.UTF8.GetString(File.ReadAllBytes(CATALOG_FILENAME));

                // Construct the regular expressions
                Regex spriteRegex = new Regex("<sprites>(.+?)</sprites>", RegexOptions.Singleline);
                Regex mURLRegex = new Regex("<url>(.+?)</url>", RegexOptions.Singleline);
                Regex mFirstSpriteIDRegex = new Regex("<firstSpriteID>(.+?)</firstSpriteID>", RegexOptions.Singleline);
                Regex mLastSpriteIDRegex = new Regex("<lastSpriteID>(.+?)</lastSpriteID>", RegexOptions.Singleline);
                Regex mSpriteTypeRegex = new Regex("<spriteType>(.+?)</spriteType>", RegexOptions.Singleline);

                MatchCollection sprites = spriteRegex.Matches(data);
                Match mURL;
                Match mFirstSpriteID;
                Match mLastSpriteID;
                Match mSpriteType;

                string sSprite;
                string sURL = "";
                string sFirstSpriteID = "";
                string sLastSpriteID = "";
                string sSpriteType = "";

                foreach (Match m in sprites)
                {
                    sSprite = m.Value;
                    // Perform the tag matches
                    mURL = mURLRegex.Match(sSprite);
                    mFirstSpriteID = mFirstSpriteIDRegex.Match(sSprite);
                    mLastSpriteID = mLastSpriteIDRegex.Match(sSprite);
                    mSpriteType = mSpriteTypeRegex.Match(sSprite);

                    sURL = mURL.Groups.Count > 0 ? mURL.Groups[1].ToString() : "";
                    sFirstSpriteID = mFirstSpriteID.Groups.Count > 0 ? mFirstSpriteID.Groups[1].ToString() : "";
                    sLastSpriteID = mLastSpriteID.Groups.Count > 0 ? mLastSpriteID.Groups[1].ToString() : "";
                    sSpriteType = mSpriteType.Groups.Count > 0 ? mSpriteType.Groups[1].ToString() : "";

                    sURL = sURL.Replace(TIBIA_RESOURCE_PREFIX, "");
                    results.Add(new SpriteSheet(sURL, Convert.ToInt32(sFirstSpriteID), Convert.ToInt32(sLastSpriteID), Convert.ToInt32(sSpriteType)));
                }
                SpriteSheet[] arr = results.ToArray();
                string json = JsonConvert.SerializeObject(arr);
                // Save JSON
                using (FileStream fs = new FileStream("SpriteSheetIndex.json", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(json);
                    }
                }
                HandleSpriteSheets(results);
            }
        }
    }
}