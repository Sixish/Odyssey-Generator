using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReader
{
    [Serializable()]
    class DatItem
    {
        public Dictionary<int, int> Flags = new Dictionary<int, int>();

        private bool isGround = false;
        private int speed = 0;

        private bool isBlocking = false;

        private bool isImmobile = false;

        private bool blocksMissiles = false;

        private bool blocksPath = false;

        private bool providesLight = false;
        private int lightRadius = 0;
        private int lightColor = 0;

        private bool isGroundItem = false;

        private bool hasOffset = false;
        private int offsetX = 0;
        private int offsetY = 0;

        private bool hasMapColor = false;
        private int mapColor = 0;

        private bool hasHeight = false;
        private int height = 0;

        private bool hasBodySlot = false;
        private int bodySlot = 0;

        private int unknown35 = 0;

        private bool hasActions = false;
        private int actions = 0;

        private bool isWritable = false;
        private int characters = 0;

        private bool isStackable = false;

        private bool isTopOrderOne = false;
        private bool isTopOrderTwo = false;
        private bool isTopOrderThree = false;

        private bool isContainer = false;

        private bool isCorpse = false;

        private bool isPickupable = false;

        private bool unknown23 = false;

        private bool isRotatable = false;

        private bool unknown254 = false;

        private bool isUsable = false;

        private bool isBottomLayer = false;

        private bool isLookThrough = false;

        private bool isHangable = false;

        private bool isFloorChange = false;

        private bool isHorizontal = false;

        private bool isVertical = false;

        private bool isFluidContainer = false;

        private bool isFluid = false;

        private bool isRewritable = false;

        private int rewriteCharacters = 0;

        private bool isBlocking1000 = false;


        public bool IsGround
        {
            get { return isGround; }
            set { isGround = value; }
        }

        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public bool IsBlocking
        {
            get { return isBlocking; }
            set { isBlocking = value; }
        }

        public bool IsImmobile
        {
            get { return isImmobile; }
            set { isImmobile = value; }
        }

        public bool BlocksMissiles
        {
            get { return blocksMissiles; }
            set { blocksMissiles = value; }
        }

        public bool BlocksPath
        {
            get { return blocksPath; }
            set { blocksPath = value; }
        }

        public bool ProvidesLight
        {
            get { return providesLight; }
            set { providesLight = value; }
        }

        public int LightRadius
        {
            get { return lightRadius; }
            set { lightRadius = value; }
        }

        public int LightColor
        {
            get { return lightColor; }
            set { lightColor = value; }
        }

        public bool IsGroundItem
        {
            get { return isGroundItem; }
            set { isGroundItem = value; }
        }

        public bool HasOffset
        {
            get { return hasOffset; }
            set { hasOffset = value; }
        }

        public int OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }

        public int OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }

        public bool HasMapColor
        {
            get { return hasMapColor; }
            set { hasMapColor = value; }
        }

        public int MapColor
        {
            get { return mapColor; }
            set { mapColor = value; }
        }

        public bool HasHeight
        {
            get { return hasHeight; }
            set { hasHeight = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public bool HasBodySlot
        {
            get { return hasBodySlot; }
            set { hasBodySlot = value; }
        }

        public int BodySlot
        {
            get { return bodySlot; }
            set { bodySlot = value; }
        }

        public int Unknown35
        {
            get { return unknown35; }
            set { unknown35 = value; }
        }

        public bool HasActions
        {
            get { return hasActions; }
            set { hasActions = value; }
        }

        public int Actions
        {
            get { return actions; }
            set { actions = value; }
        }

        public bool IsWritable
        {
            get { return isWritable; }
            set { isWritable = value; }
        }

        public int Characters
        {
            get { return characters; }
            set { characters = value; }
        }

        public bool IsStackable
        {
            get { return isStackable; }
            set { isStackable = value; }
        }

        public bool IsTopOrderOne
        {
            get { return isTopOrderOne; }
            set { isTopOrderOne = value; }
        }

        public bool IsTopOrderTwo
        {
            get { return isTopOrderTwo; }
            set { isTopOrderTwo = value; }
        }

        public bool IsTopOrderThree
        {
            get { return isTopOrderThree; }
            set { isTopOrderThree = value; }
        }

        public bool IsContainer
        {
            get { return isContainer; }
            set { isContainer = value; }
        }

        public bool IsCorpse
        {
            get { return isCorpse; }
            set { isCorpse = value; }
        }

        public bool IsPickupable
        {
            get { return isPickupable; }
            set { isPickupable = value; }
        }

        public bool Unknown23
        {
            get { return unknown23; }
            set { unknown23 = value; }
        }

        public bool IsRotatable
        {
            get { return isRotatable; }
            set { isRotatable = value; }
        }

        public bool Unknown254
        {
            get { return unknown254; }
            set { unknown254 = value; }
        }

        public bool IsUsable
        {
            get { return isUsable; }
            set { isUsable = value; }
        }

        public bool IsBottomLayer
        {
            get { return isBottomLayer; }
            set { isBottomLayer = value; }
        }

        public bool IsLookThrough
        {
            get { return isLookThrough; }
            set { isLookThrough = value; }
        }

        public bool IsHangable
        {
            get { return isHangable; }
            set { isHangable = value; }
        }

        public bool IsFloorChange
        {
            get { return isFloorChange; }
            set { isFloorChange = value; }
        }
        
        public bool IsHorizontal
        {
            get { return isHorizontal; }
            set { isHorizontal = value; }
        }

        public bool IsVertical
        {
            get { return isVertical; }
            set { isVertical = true; }
        }

        public bool IsFluidContainer
        {
            get { return isFluidContainer; }
            set { isFluidContainer = value; }
        }

        public bool IsFluid
        {
            get { return isFluid; }
            set { isFluid = value; }
        }

        public bool IsRewritable
        {
            get { return isRewritable; }
            set { isRewritable = value; }
        }

        public int RewriteCharacters
        {
            get { return rewriteCharacters; }
            set { rewriteCharacters = value; }
        }

        public bool IsBlocking1000
        {
            get { return isBlocking1000; }
            set { isBlocking1000 = value; }
        }

    }
}
