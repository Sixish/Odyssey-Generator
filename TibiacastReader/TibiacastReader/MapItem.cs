using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiacastReader
{
    class MapItem
    {
        public int itemid;
        public int itemcount;
        public int itemattr;

        public MapItem(int itemid, int itemcount, int itemattr)
        {
            this.itemid = itemid;
            this.itemcount = itemcount;
            this.itemattr = itemattr;
        }
    }
}
