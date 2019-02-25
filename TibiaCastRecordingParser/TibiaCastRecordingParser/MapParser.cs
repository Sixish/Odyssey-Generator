using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TibiaDatReader;

namespace TibiaCastRecordingParser
{
    // Contains information relevant to the current
    // parse subject.
    // New MapParser objects are created when a new MapPosition is found.
    public class MapParser
    {
        // Tells us the offset to begin our parsing
        // (i.e. char position - viewport width / 2).
        public const int POSITION_VIEWPORT_OFFSET_X = 8;
        public const int POSITION_VIEWPORT_OFFSET_Y = 6;

        // How many bytes are used to convey position components.
        public const int PACKET_BYTES_POSX = 2;
        public const int PACKET_BYTES_POSY = 2;
        public const int PACKET_BYTES_POSZ = 1;

        public MapFragment Map
        {
            get;
            set;
        }

        // Debugging tool. Store the list of found creatures here.
        public static Dictionary<long, MapCreature> Creatures = new Dictionary<long, MapCreature>();

        // The byte currently being parsed.
        public int bPosition = 0;

        // The character's position on the map. The map information is
        // sent to the client based on its position.
        public Position3D charPosition = new Position3D(0, 0, 0);

        // The position offset within a packet.
        public Position3D currOffset = new Position3D(0, 0, 0);

        // Where we start writing on the map.
        public Position3D mapPosition = new Position3D(0, 0, 0);

        // The last refresh point (Packet 0x64).
        public Position3D mapStartPosition = new Position3D(0, 0, 0);

        // Pattern tiles. These sequence of bytes have special meaning
        // within Tibia packets.

        // PatternTile. This list of bytes mark a new tile.
        public static byte[] PatternTile = (new List<byte> { 0x00, 0x00 }).ToArray();

        // SkipAhead. This list of bytes marks a skip instruction.
        // The byte preceding this match is the size of the skip.
        public static byte[] SkipAhead = (new List<byte> { 0xFF }).ToArray();

        // EndTile. These bytes mark a tile's end on 9.5 map packets.
        public static byte[] EndTile = (new List<byte> { 0x00, 0xFF }).ToArray();

        // CreaturePatterns. These lists of bytes mark creatures.
        // These have varying lengths and formats.
        public static byte[] CreaturePattern = (new List<byte> { 0x63, 0x00 }).ToArray();
        public static byte[] CreaturePattern2 = (new List<byte> { 0x62, 0x00 }).ToArray();
        public static byte[] CreaturePattern3 = (new List<byte> { 0x61, 0x00 }).ToArray();

        // We need to initialize the map first, or the map will be 1 tile off.
        public bool isInitialized;

        // The direction we are going; known values: -1, +1.
        public int direction;

        // Dictionary for debugging. Used to record valid creature data.
        //public static Dictionary<int, List<int>> DebugDictionary = new Dictionary<int, List<int>>();

        public Dat DatContextInput
        {
            get;
            set;
        }

        public Dat DatContextOutput
        {
            get;
            set;
        }

        public string ErrorSource
        {
            get;
            set;
        }

        public int RecordingID
        {
            get;
            set;
        }

        // Parse the position bytes. X = 2b, Y = 2b, Z = 1b.
        public void ParseMapPosition(byte[] data, Position3D outPos)
        {
            // The byte index.
            int b, x, y, z;
            // Initialize all to 0 to avoid errors.
            x = y = z = 0;

            // Parse position X.
            for (b = 0; b < PACKET_BYTES_POSX; b++, bPosition++)
            {
                x += data[bPosition] << (8 * b);
            }

            // Parse Position Y.
            for (b = 0; b < PACKET_BYTES_POSY; b++, bPosition++)
            {
                y += data[bPosition] << (8 * b);
            }

            // Parse Position Z.
            for (b = 0; b < PACKET_BYTES_POSZ; b++, bPosition++)
            {
                z += data[bPosition] << (8 * b);
            }

            outPos.X = x;
            outPos.Y = y;
            outPos.Z = z;
        }

        // LookAhead to find a byte pattern.
        public bool lookAhead(byte[] data, byte[] match, int offset = 0)
        {
            // The index (i -> match).
            int i = 0;
            // The length of the match.
            int len = match.Length;

            // Ensure the position in the data + the length of the pattern
            // are within the bounds of our search.
            if ((data.Length - offset - bPosition) < len)
            {
                return false;
            }

            // Iterate over the data.
            for (i = 0; i < len; i++)
            {
                if (data[bPosition + offset + i] != match[i])
                {
                    // We found an unmatched byte. Pattern does not match.
                    return false;
                }
            }
            return true;
        }

        // Overloaded function.
        // Consume count bytes and return true.
        public bool ConsumeBytes(int count)
        {
            bPosition += count;
            return true;
        }

        // Consume count bytes and return the bytes consumed.
        public byte[] ConsumeBytes(int count, byte[] data)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++, bPosition++)
            {
                bytes[i] = data[bPosition];
            }
            return bytes;
        }

        // Width, Height, Depth of the cursor range.
        // Used to determine where to wrap around.
        public static Position3D CursorBoundaries = new Position3D(18, 14, 15);

        // Where the cursor should start.
        public static Position3D CursorOffset = new Position3D(0, 0, 0);

        // Computed DrawPosition
        public static Position3D DrawPosition = new Position3D(0, 0, 0);

        // Get the position where we are writing to, based on
        // DepthOffset, CurrOffset, CharPosition and CursorOffset.
        public void GetDrawPosition(Position3D outPos)
        {
            outPos.Z = mapPosition.Z + currOffset.Z + CursorOffset.Z;

            // TEST
            outPos.X = charPosition.X + currOffset.X + CursorOffset.X - outPos.Z + charPosition.Z - POSITION_VIEWPORT_OFFSET_X;
            outPos.Y = charPosition.Y + currOffset.Y + CursorOffset.Y - outPos.Z + charPosition.Z - POSITION_VIEWPORT_OFFSET_Y;

        }

        // Move the cursor diff tiles, respecting CursorBoundaries.
        public bool MoveCursor(int diff)
        {
            currOffset.Y += diff;
            while (currOffset.Y >= CursorBoundaries.Y)
            {
                currOffset.X += 1;
                currOffset.Y -= CursorBoundaries.Y;
                while (currOffset.X >= CursorBoundaries.X)
                {
                    currOffset.Z += 1 * direction;
                    currOffset.X -= CursorBoundaries.X;
                }
            }
            return true;
        }

        // Initialize the map. Returns true. Set isInitialized = InitializeMap();
        public bool InitializeMap()
        {
            isInitialized = true;
            return true;
        }

        // Parse an item.
        public ParseResult ParseItem(byte[] data)
        {
            // Can cause problems, especially when parsing
            // unsupported versions. Catch any errors.
            int id = 0;

            try
            {
                // The item we parsed.
                MapItem item = new MapItem();

                // The index, reused for all indexes.
                // See also bPosition.
                int i;

                // The item ID that we parsed.
                id = 0;
                // Amount of spaces to skip, if we found a skip.
                int skip = 0;

                // Get the Item ID (2b).
                for (i = 0; i < 2; i++, bPosition++)
                {
                    id += data[bPosition] << (i * 8);
                }
                item.ID = id;
                // If id is invalid (out of range), log and consume until we get a 0xFF.
                // Not a good idea, but better than throwing an error.
                if (id < 100 || id > DatContextInput.Items.Count)
                {
                    Console.WriteLine("Could not parse item with id=" + id.ToString());
                    if (ErrorSource != null)
                    {
                        using (FileStream fs = new FileStream(ErrorSource, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                sw.WriteLine(RecordingID + " : Could not parse item with ID : " + id);
                            }
                        }
                        
                    }
                    while (data[bPosition] != 0xFF)
                    {
                        ConsumeBytes(1);
                    }
                    // Consume the last 0xFF.
                    ConsumeBytes(1);
                    return new ParseResult(null, 0);
                }

                if (DatContextInput.Items.Count < id)
                {
                    Console.WriteLine("Could not parse item. Wrong dat?");
                }

                if (id < 100 || DatContextInput.Items.Count < id)
                {
                    Console.WriteLine("Input DAT is too small. ItemID=" + id);
                    return null;
                }
                if (id < 100 || DatContextOutput.Items.Count < id)
                {
                    Console.WriteLine("Output DAT is too small. ItemID=" + id);
                    return null;
                }

                DatItem datItemOut = DatContextOutput.GetItem(id);

                DatItem datItemIn = DatContextInput.GetItem(id);

                //if (9.5F <= DatContextInput.Version && DatContextInput.Version <= 10.90F)
                //if (9.6F < DatContextInput.Version && DatContextInput.Version <= 10.90F)
                //if (9.7F < DatContextInput.Version && DatContextInput.Version <= 10.90F)
                if (9.86F < DatContextInput.Version && DatContextInput.Version <= 10.90F)
                {
                    if (data[bPosition] != 0xFF)
                    {
                        Console.WriteLine("Did not find ending 0xFF.");
                    }
                    ConsumeBytes(1);
                }

                // Check if we are dealing with a stackable item
                if (datItemOut.IsStackable)
                {
                    item.IsStackable = true;
                }
                if (datItemIn.IsStackable)
                {
                    // Stackable items have an attribute byte. Set & Consume.
                    item.Count = data[bPosition];
                    ConsumeBytes(1);
                }

                // Check if we are dealing with an animated item.
                if (datItemOut.IsAnimated)
                {
                    item.IsAnimated = true;
                }
                if (datItemIn.IsAnimated)
                {
                    // Animated items have an attribute byte. Set & Consume.
                    item.AnimationStep = data[bPosition];
                    ConsumeBytes(1);
                }

                // Check if we are dealing with a splash item.
                if (datItemOut.IsFluid)
                {
                    item.IsSplash = true;
                }
                if (datItemIn.IsFluid)
                {
                    // Splash items have an attribute byte. Set & Consume.
                    item.Splash = data[bPosition];
                    ConsumeBytes(1);
                }

                // Check if we are dealing with a fluid item
                if (datItemOut.IsFluidContainer)
                {
                    item.IsFluid = true;
                }
                if (datItemIn.IsFluidContainer)
                {
                    // Fluid items have an attribute byte. Set & Consume.
                    item.Fluid = data[bPosition];
                    ConsumeBytes(1);
                }

                // Return a new ParseResult with the information.
                return new ParseResult(item, skip);
            }
            catch (Exception e)
            {
                // TODO Make more meaningful.
                //Console.WriteLine("Caught an error.");
                //Console.WriteLine(e);

                // Return a null ParseResult so that we don't do anything.
                return new ParseResult(null, 0);
            }
        }
        // Output data to text file.
        public static void OutputMapCreaturesToFile(string destination, string filename, Dictionary<long, MapCreature> dict)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<long, MapCreature> kvp in dict)
            {
                sb.Append(kvp.Value.ToString() + Environment.NewLine);
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            using (FileStream fs = new FileStream(destination + "\\" + filename, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sb.ToString());
                }
            }
        }

        public MapCreature ParseCreature98(byte[] data)
        {
            // 2b - Creature header.
            int header = 0;
            for (int i = 0; i < 2; i++, bPosition++)
            {
                header += data[bPosition] << (i * 8);
            }

            // 4b - Creature ID.
            int id = 0;
            for (int i = 0; i < 4; i++, bPosition++)
            {
                id += data[bPosition] << (i * 8);
            }

            // 1b - Creature Health %
            int healthPercent = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                healthPercent += data[bPosition] << (i * 8);
            }

            // 1b - Creature Facing Direction
            int facing = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                facing += data[bPosition] << (i * 8);
            }

            // 2b - Outfit
            int outfit = 0;
            for (int i = 0; i < 2; i++, bPosition++)
            {
                outfit += data[bPosition] << (i * 8);
            }

            // 4b - Outfit colors
            int outfitColors = 0;
            for (int i = 0; i < 4; i++, bPosition++)
            {
                outfitColors += data[bPosition] << (i * 8);
            }

            // 3b - Creature Addons and Mount, only if outfit != 0.
            int addons = 0;
            int mount = 0;
            // Testing for 9.5
            //if (outfit != 0 || true)
            if (outfit != 0)
            {
                // 1b - Addons.
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    addons += data[bPosition] << (i * 8);
                }

                // 2b - Mount.
                for (int i = 0; i < 2; i++, bPosition++)
                {
                    mount += data[bPosition] << (i * 8);
                }
            }
            
            // 1b - Light Radius.
            int lightRadius = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                lightRadius += data[bPosition] << (i * 8);
            }

            // 1b - Light Color.
            int lightColor = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                lightColor += data[bPosition] << (i * 8);
            }

            // 2b - Speed.
            int speed = 0;
            for (int i = 0; i < 2; i++, bPosition++)
            {
                speed += data[bPosition] << (i * 8);
            }

            // 4b - Unknown.
            int ukend1 = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                ukend1 += data[bPosition] << (i * 8);
            }

            int ukend2 = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                ukend2 += data[bPosition] << (i * 8);
            }

            int ukend3 = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                ukend3 += data[bPosition] << (i * 8);
            }

            int ukend4 = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                ukend4 += data[bPosition] << (i * 8);
            }

            // Testing.
            bPosition -= 1;
            // 10.01+
            int ukend5 = 0;
            int ukend6 = 0;
            int ukend7 = 0;
            int ukend8 = 0;
            if (10.01F <= DatContextInput.Version)
            {
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend5 += data[bPosition] << (i * 8);
                }

                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend6 += data[bPosition] << (i * 8);
                }

                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend7 += data[bPosition] << (i * 8);
                }

                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend8 += data[bPosition] << (i * 8);
                }
                //bPosition += 4;
            }

            // Experimental.
            int ukend9 = 0;
            if (10.39F <= DatContextInput.Version)
            {
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend9 += data[bPosition] << (i * 8);
                }
                //bPosition += 1;
            }

            return new MapCreature(
                header, 0, id, 0, 0, "", healthPercent, facing, outfit,
                outfitColors, outfitColors, outfitColors, outfitColors,
                addons, mount, lightRadius, lightColor, speed, 0, 0, 0, ukend1,
                ukend2, ukend3, ukend4, ukend5, ukend6, ukend7, ukend8
            );
        }
        public MapCreature ParseCreature(byte[] data)
        {
            // 2b - Creature header.
            int header = 0;
            for (int i = 0; i < 2; i++, bPosition++)
            {
                header += data[bPosition] << (i * 8);
            }

            // 4b - Unknown.
            int ukheader = 0;
            for (int i = 0; i < 4; i++, bPosition++)
            {
                ukheader += data[bPosition] << (i * 8);
            }

            // 4b - Creature ID.
            int id = 0;
            for (int i = 0; i < 4; i++, bPosition++)
            {
                id += data[bPosition] << (i * 8);
            }

            // 1b - unknown.
            int uk1 = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                uk1 += data[bPosition] << (i * 8);
            }

            // 1b - character name length.
            int charNameLength = 0;
            for (int i = 0; i < 2; i++, bPosition++)
            {
                charNameLength += data[bPosition] << (i * 8);
            }

            // [charNameLength]b - char name.
            string charName = "";
            for (int i = 0; i < charNameLength; i++, bPosition++)
            {
                charName += char.ConvertFromUtf32(data[bPosition]);
            }

            // 1b - health percentage.
            int healthPercent = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                healthPercent += data[bPosition] << (i * 8);
            }

            // 1b - direction.
            int direction = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                direction += data[bPosition] << (i * 8);
            }

            // 2b - outfit.
            int outfit = 0;
            for (int i = 0; i < 2; i++, bPosition++)
            {
                outfit += data[bPosition] << (i * 8);
            }

            // 1b - color head.
            int colorHead = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                colorHead += data[bPosition] << (i * 8);
            }
            
            // 1b - color body.
            int colorBody = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                colorBody += data[bPosition] << (i * 8);
            }
            
            // 1b - color legs.
            int colorLegs = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                colorLegs += data[bPosition] << (i * 8);
            }

            // 1b - color feet.
            int colorFeet = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                colorFeet += data[bPosition] << (i * 8);
            }

            // If there's no oufit then addons and mount are not included.
            int addons = 0;
            int mount = 0;
            if (outfit != 0)
            {
                // 1b - addons.
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    addons += data[bPosition] << (i * 8);
                }

                // 1b - mount.
                for (int i = 0; i < 2; i++, bPosition++)
                {
                    mount += data[bPosition] << (i * 8);
                }
            }

            // 1b - light radius.
            int lightRadius = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                lightRadius += data[bPosition] << (i * 8);
            }

            // 1b - light color.
            int lightColor = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                lightColor += data[bPosition] << (i * 8);
            }

            // 2b - speed.
            int speed = 0;
            for (int i = 0; i < 2; i++, bPosition++)
            {
                speed += data[bPosition] << (i * 8);
            }

            // 1b - skull.
            int skull = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                skull += data[bPosition] << (i * 8);
            }

            // 1b - party shield.
            int partyShield = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                partyShield += data[bPosition] << (i * 8);
            }

            // 1b - guild shield.
            int guildShield = 0;

            int ukend1 = 0;

            // 1b - unknown.
            int ukend2 = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                ukend2 += data[bPosition] << (i * 8);
            }

            // 1b - unknown.
            int ukend3 = 0;
            for (int i = 0; i < 1; i++, bPosition++)
            {
                ukend3 += data[bPosition] << (i * 8);
            }

            // 10.01+, 10.0?
            int ukend4 = 0;
            int ukend5 = 0;
            int ukend6 = 0;
            int ukend7 = 0;
            if (10.01F <= DatContextInput.Version)
            {
                // Possibly PvP frames.
                // 1b - unknown.
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend4 += data[bPosition] << (i * 8);
                }
                // 1b - unknown.
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend5 += data[bPosition] << (i * 8);
                }
                // 1b - unknown.
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend6 += data[bPosition] << (i * 8);
                }
                // 1b - unknown.
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend7 += data[bPosition] << (i * 8);
                }

                //bPosition += 4;
            }

            // Experimental.
            int ukend8 = 0;
            if (10.39F <= DatContextInput.Version)
            {
                // 1b - unknown.
                for (int i = 0; i < 1; i++, bPosition++)
                {
                    ukend8 += data[bPosition] << (i * 8);
                }
                //bPosition += 1;
            }

            MapCreature creature = new MapCreature(
                header: header,
                ukheader: ukheader,
                id: id,
                type: uk1,
                charNameLength: charNameLength,
                charName: charName,
                healthPercent: healthPercent,
                direction: direction,
                outfit: outfit,
                colorHead: colorHead,
                colorBody: colorBody,
                colorLegs: colorLegs,
                colorFeet: colorFeet,
                addons: addons,
                mount: mount,
                lightRadius: lightRadius,
                lightColor: lightColor,
                speed: speed,
                skull: skull,
                partyShield: partyShield,
                guildShield: guildShield,
                ukend1: ukend1,
                ukend2: ukend2,
                ukend3: ukend3,
                ukend4: ukend4,
                ukend5: ukend5,
                ukend6: ukend6,
                ukend7: ukend7,
                ukend8: ukend8
            );
            return creature;
        }

        // Set the floor, and adjust necessary variables when floor changes occur.
        public void SetFloor(int posz)
        {
            // Set the variables that need to change when PosZ changes.
            charPosition.Z = posz;
            if (posz <= 7)
            {
                // Respect the z-direction.
                direction = -1;

                // Set the map position, the origin of our parsing.
                mapPosition.Z = 7;

                //CursorOffset.X = CursorOffset.Y = 0;
                //CursorOffset.Set(0, 0, 0);
                CursorBoundaries.Set(18, 14, 15);
                CursorOffset.Z = 0;
            }
            else
            {
                // Respect the z-direction.
                direction = +1;

                // Set the map position, the origin of our parsing.
                //mapPosition.Z = Math.Max(charPosition.Z - 2, 0);
                //mapPosition.Z = Math.Max(charPosition.Z, 0);
                mapPosition.Z = charPosition.Z;

                //CursorOffset.X = CursorOffset.Y = 0;
                CursorOffset.Z = -2;
                //CursorOffset.Set(0, 0, 0);
                CursorBoundaries.Set(18, 14, 15);
            }
            return;
        }
        public void Parse(byte[] data, int type, bool isUsed = true)
        {
            // Guard against packet size = 0.
            if (data.Length == 0 && type != 0xBF && type != 0xBE)
            {
                Console.WriteLine("Packet of size 0?");
                return;
            }

            // List of items found on the map.
            List<MapItem> list = new List<MapItem>();

            // New Parse = reset cursor position.
            bPosition = 0;

            // Current Position on the map; cursor position for the packet data.
            currOffset.Set(0, 0, 0);

            // Current Offset on the grid; position relative to the origin for the cursor.
            DrawPosition.Set(0, 0, 0);

            // Reset isInitialized,
            isInitialized = false;

            if (type == 0x64)
            {
                // Parse target = complete map with position header.

                // TEST ONLY
                //isInitialized = InitializeMap();

                // Parse the MapPosition and set the CharPosition.
                ParseMapPosition(data, mapStartPosition);
                charPosition.Set(mapStartPosition.X, mapStartPosition.Y, mapStartPosition.Z);

                // Set the floor to set appropriate variables.
                SetFloor(mapStartPosition.Z);

                // Shift the position in the direction we just moved.
                charPosition.Shift(0, 0, 0); // Moved none

                // Set the boundaries the cursor should have (w x h x d).
                CursorBoundaries.Set(18, 14, 15);

                // Set the offset the cursor should have.
                CursorOffset.X = CursorOffset.Y = 0;
                //CursorOffset.Set(0, 0, 0);

            }
            else if (type == 0x65)
            {
                // Moved north.

                // North movements don't initialize map.
                //InitializeMap();

                // Shift the position in the direction we just moved.
                charPosition.Shift(0, -1, 0);
                //mapPosition.Shift(0, -1, 0);

                // Set the boundaries the cursor should have (w x h x d).
                CursorBoundaries.Set(18, 1, 15);

                // Set the offset the cursor should have.

                // These were the expected values.
                CursorOffset.X = CursorOffset.Y = 0;
                CursorOffset.Set(0, 0, (charPosition.Z > 7 ? -2 : 0));

                // I don't understand why we need these values though.
                //CursorOffset.Set(-1, -1, 0);
            }
            else if (type == 0x66)
            {
                // Moved east.

                // East movements don't initialize map.
                //InitializeMap();

                // Shift the position in the direction we just moved.
                charPosition.Shift(1, 0, 0);
                //mapPosition.Shift(1, 0, 0);

                // Set the offset the cursor should have.

                // These were the expected values.
                CursorOffset.X = 17;
                CursorOffset.Y = 0;
                CursorOffset.Set(17, 0, (charPosition.Z > 7 ? -2 : 0));

                // Set the boundaries the cursor should have (w x h x d).
                CursorBoundaries.Set(1, 14, 15);
            }
            else if (type == 0x67)
            {
                // Moved south.

                // South movements don't initialize map.
                //InitializeMap();

                // Shift the position in the direction we just moved.
                charPosition.Shift(0, 1, 0);
                //mapPosition.Shift(0, 1, 0);

                // Set the offset the cursor should have.
                CursorOffset.X = 0;
                CursorOffset.Y = 13;
                CursorOffset.Set(0, 13, (charPosition.Z > 7 ? -2 : 0));

                // Set the boundaries the cursor should have (w x h x d).
                CursorBoundaries.Set(18, 1, 15);
            }
            else if (type == 0x68)
            {
                // Moved west.

                // West movements don't initialize map.
                //isInitialized = InitializeMap();

                // Shift the position in the direction we just moved.
                charPosition.Shift(-1, 0, 0);
                //mapPosition.Shift(-1, 0, 0);

                // Set the offset the cursor should have.
                CursorOffset.X = CursorOffset.Y = 0;
                CursorOffset.Set(0, 0, (charPosition.Z > 7 ? -2 : 0));

                // Set the boundaries the cursor should have (w x h x d).
                CursorBoundaries.Set(1, 14, 15);
            }
            else if (type == 0xBF)
            {
                // Moved down a floor.
                SetFloor(charPosition.Z + 1);
                charPosition.Shift(-1, -1, 0);

                if (mapPosition.Z <= 7)
                {
                    // ???
                    CursorOffset.Z = 0;
                }
                else
                {
                    // ???
                    CursorOffset.Set(0, 0, 0);
                }
            }
            else if (type == 0xBE)
            {
                // Moved up a floor.
                SetFloor(charPosition.Z - 1);
                charPosition.Shift(1, 1, 0);
                if (mapPosition.Z <= 7)
                {
                    // ???
                    CursorOffset.Z = -2;
                }
                else
                {
                    // ???
                    CursorOffset.Z = -2;
                }
            }
            if (!isUsed)
            {
                return;
            }

            // Create a tile, to avoid potential errors & compiler issues.
            GetDrawPosition(DrawPosition);
            MapTile tile = new MapTile();

            // Parse the data
            while (bPosition < data.Length)
            {
                // Ensure CurrPosition has the current position.
                GetDrawPosition(DrawPosition);

                // Perform checks on the following data.
                if (lookAhead(data, CreaturePattern3))
                {
                    // We found a creature.
                    // Don't care about creatures, just consume & dispose.
                    MapCreature creature = ParseCreature(data);

                    // Record the creature.
                    if (!Creatures.ContainsKey(creature.ID))
                    {
                        Creatures[creature.ID] = creature;
                    }

                    tile.Creatures.Add(creature);

                    // Experimental...
                    if (lookAhead(data, EndTile))
                    {
                        ConsumeBytes(EndTile.Length);
                    }
                }
                else if (lookAhead(data, CreaturePattern))
                {
                    // We found a creature, fixed width 8b.
                    // Don't care about creatures, just consume, record & dispose.
                    // Debug only.
                    //ByteRecordStream.RecordByteStream(ConsumeBytes(8, data), "frag-99.txt");
                    ConsumeBytes(8, data);
                }
                else if (lookAhead(data, CreaturePattern2))
                {
                    // Creature with info, length depends on contents.
                    // Don't care about creatures, just consume & dispose.
                    MapCreature creature = ParseCreature98(data);

                    // Record the creature.
                    if (Creatures.ContainsKey(creature.ID))
                    {
                        tile.Creatures.Add(creature);
                    }
                    else
                    {
                        // This happens because(?) the first block, 0x07, contains a list of creatures.
                        //Console.WriteLine("Found unknown creature. Bug?");
                    }

                    // Experimental...
                    if (lookAhead(data, EndTile))
                    {
                        ConsumeBytes(EndTile.Length);
                    }
                }
                else if (lookAhead(data, PatternTile))
                {
                    // We are dealing with a new tile. Initialize or move the cursor.
                    if (!isInitialized)
                    {
                        InitializeMap();
                    }
                    else
                    {
                        MoveCursor(1);
                    }

                    // Ensure we have the current position.
                    GetDrawPosition(DrawPosition);

                    // Consume the bytes of the pattern.
                    ConsumeBytes(PatternTile.Length);

                    // New tile = new Tile()!
                    tile = new MapTile();

                    Map.AddTile(DrawPosition, tile);
                }
                else if (lookAhead(data, SkipAhead, 1))
                {
                    // If the map is not initialized, initialize it.
                    InitializeMap();

                    // We are dealing with a skip 0x__ 0xFF.
                    // Move the cursor based on the skip type.
                    MoveCursor(data[bPosition]);

                    // Consume the pattern.
                    ConsumeBytes(2);
                }
                else if (lookAhead(data, EndTile))
                {

                }
                else
                {
                    // We found an item.
                    GetDrawPosition(DrawPosition);
                    ParseResult result = ParseItem(data);
                    if (result.Item != null)
                    {
                        if (DrawPosition.Z < 0 || 15 < DrawPosition.Z)
                        {
                            Console.WriteLine("Z out of bounds>");
                        }
                        tile.Items.Add(result.Item);
                        // Experimental...
                        if (lookAhead(data, EndTile))
                        {
                            ConsumeBytes(EndTile.Length);
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Found no item...");
                    }
                }
            }
            return;
        }

        public MapParser(Dat datContextIn, Dat datContextOut)
        {
            DatContextInput = datContextIn;
            DatContextOutput = datContextOut;
            Map = new MapFragment();
        }
    }
}
