using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCastRecordingParser
{
    // Result of parsing.
    // ParseResult objects will be created for every
    // item and skip found in the map packets.
    public class ParseResult
    {
        // If we found an item, this will not be null.
        public MapItem Item
        {
            get;
            set;
        }

        // If we found a skip, this will not be null.
        public int Skip
        {
            get;
            set;
        }

        // Constructor
        public ParseResult(MapItem item, int skip)
        {
            // Set the object's properties.
            Item = item;
            Skip = skip;
        }
    }

}
