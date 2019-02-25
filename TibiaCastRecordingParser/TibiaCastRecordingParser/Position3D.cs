using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCastRecordingParser
{
    // Position3D class.
    // Used to resolve and manipulate matrices.
    public class Position3D
    {
        public const int BITS_PER_BYTE = 8;

        public const int BITMASK_POSX = 0xFFFF;
        public const int BITMASK_POSY = 0xFFFF;
        public const int BITMASK_POSZ = 0XF;

        public const int BITS_POSX = BITS_PER_BYTE * 4;
        public const int BITS_POSY = BITS_PER_BYTE * 4;
        public const int BITS_POSZ = BITS_PER_BYTE * 1;

        public const int OFFSET_POSX = 0;
        public const int OFFSET_POSY = OFFSET_POSX + BITS_POSX;
        public const int OFFSET_POSZ = OFFSET_POSY + BITS_POSY;

        // Position x, y, z components.
        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public int Z
        {
            get;
            set;
        }

        // Override ToString for custom output.
        public override string ToString()
        {
            return X.ToString("X4") + Y.ToString("X4") + Z.ToString("X2");
        }

        // Shift the current matrix.
        public void Shift(int ox, int oy, int oz)
        {
            X += ox;
            Y += oy;
            Z += oz;
        }

        // Set the current matrix values.
        public void Set(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static long GetOffset(int x, int y, int z)
        {
            return
                (((long)x) << OFFSET_POSX) +
                (((long)y) << OFFSET_POSY) +
                (((long)z) << OFFSET_POSZ);
        }

        // Constructor.
        public Position3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
