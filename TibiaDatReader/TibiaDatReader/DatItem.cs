using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TibiaDatReader
{
    public class DatItem
    {
        [JsonIgnore]
        public bool IsGround
        {
            get;
            set;
        }

        [JsonIgnore]
        public int Speed
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsBlocking
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsImmobile
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool BlocksMissiles
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool BlocksPath
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool ProvidesLight
        {
            get;
            set;
        }

        [JsonIgnore]
        public int LightRadius
        {
            get;
            set;
        }

        [JsonIgnore]
        public int LightColor
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsGroundItem
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool HasOffset
        {
            get;
            set;
        }

        [JsonProperty("DX")]
        public int OffsetX
        {
            get;
            set;
        }

        [JsonProperty("DY")]
        public int OffsetY
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool HasMapColor
        {
            get;
            set;
        }

        [JsonProperty("Map")]
        public int MapColor
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool HasHeight
        {
            get;
            set;
        }

        [JsonProperty("H")]
        public int Height
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool HasBodySlot
        {
            get;
            set;
        }

        [JsonIgnore]
        public int BodySlot
        {
            get;
            set;
        }

        [JsonIgnore]
        public int Unknown35
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool HasActions
        {
            get;
            set;
        }

        [JsonIgnore]
        public int Actions
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsWritable
        {
            get;
            set;
        }

        [JsonIgnore]
        public int Characters
        {
            get;
            set;
        }

        [JsonProperty("S")]
        public bool IsStackable
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsTopOrderOne
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsTopOrderTwo
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsTopOrderThree
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsContainer
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsCorpse
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsPickupable
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool Unknown23
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsRotatable
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool Unknown254
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsUsable
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsBottomLayer
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsLookThrough
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsHangable
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsFloorChange
        {
            get;
            set;
        }

        [JsonProperty("WH")]
        public bool IsHorizontal
        {
            get;
            set;
        }

        [JsonProperty("WV")]
        public bool IsVertical
        {
            get;
            set;
        }

        [JsonProperty("FC")]
        public bool IsFluidContainer
        {
            get;
            set;
        }

        [JsonProperty("F")]
        public bool IsFluid
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsRewritable
        {
            get;
            set;
        }

        [JsonIgnore]
        public int RewriteCharacters
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsBlocking1000
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SizeWidth
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SizeHeight
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SizeBase
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SizeActual
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SpriteSizeX
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SpriteSizeY
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SpriteSizeZ
        {
            get;
            set;
        }

        [JsonIgnore]
        public int Frames
        {
            get;
            set;
        }

        [JsonProperty("A")]
        public bool IsAnimated
        {
            get { return (Frames > 1); }
        }

        [JsonIgnore]
        public int SpriteCount
        {
            get;
            set;
        }

        [JsonIgnore]
        public int MarketUnknown1
        {
            get;
            set;
        }

        [JsonIgnore]
        public int MarketUnknown2
        {
            get;
            set;
        }

        [JsonIgnore]
        public int MarketUnknown3
        {
            get;
            set;
        }

        [JsonIgnore]
        public int MarketNameLength
        {
            get;
            set;
        }

        [JsonIgnore]
        public string MarketName
        {
            get;
            set;
        }

        [JsonIgnore]
        public int MarketUnknown4
        {
            get;
            set;
        }

        [JsonIgnore]
        public int MarketUnknown5
        {
            get;
            set;
        }

    }
}
