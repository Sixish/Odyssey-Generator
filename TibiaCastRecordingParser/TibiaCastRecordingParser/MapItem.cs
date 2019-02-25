using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TibiaCastRecordingParser
{
    public class MapItem : MapEntity
    {
        public int ID
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsStackable
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsAnimated
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsSplash
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsFluid
        {
            get;
            set;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Nullable<int> Count
        {
            get;
            set;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Nullable<int> AnimationStep
        {
            get;
            set;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Nullable<int> Splash
        {
            get;
            set;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Nullable<int> Fluid
        {
            get;
            set;
        }

    }
}
