using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCastRecordingParser
{
    public class MapCreature : MapEntity
    {
        public int Header
        {
            get;
            set;
        }

        public int UKHeader
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        // Type:
        //   0: Player
        //   1: Monster
        //   2: NPC
        public int CreatureType
        {
            get;
            set;
        }

        public int CharNameLength
        {
            get;
            set;
        }

        public string CharName
        {
            get;
            set;
        }

        public int HealthPercent
        {
            get;
            set;
        }

        public int Direction
        {
            get;
            set;
        }

        public int Outfit
        {
            get;
            set;
        }

        public int ColorHead
        {
            get;
            set;
        }

        public int ColorBody
        {
            get;
            set;
        }

        public int ColorLegs
        {
            get;
            set;
        }

        public int ColorFeet
        {
            get;
            set;
        }

        public int Addons
        {
            get;
            set;
        }

        public int Mount
        {
            get;
            set;
        }

        public int LightRadius
        {
            get;
            set;
        }

        public int LightColor
        {
            get;
            set;
        }

        public int Speed
        {
            get;
            set;
        }

        public int Skull
        {
            get;
            set;
        }

        public int PartyShield
        {
            get;
            set;
        }

        public int GuildShield
        {
            get;
            set;
        }

        // Unknown.
        //   0: Everything has this.
        public int UKEnd1
        {
            get;
            set;
        }

        // Unknown.
        //   5: Some players have this.
        //   0: Everything else has this.
        public int UKEnd2
        {
            get;
            set;
        }

        // Possibly "interaction type" (Flash)
        // Old:
        //   0: Players (None)
        //   1: NPCs, Monsters (Attack?)
        // New:
        //   0: Players (None)
        //   1: Monsters (Attack)
        //   2: NPCs (Talk)
        public int UKEnd3
        {
            get;
            set;
        }

        // Unknown. Probably related to NPC bubbles.
        //   0: All(?) players and monsters have this.
        //   1: All(?) NPCs have this.
        public int UKEnd4
        {
            get;
            set;
        }

        // Unknown.
        //   255: Everything(?) has this.
        public int UKEnd5
        {
            get;
            set;
        }

        // Unknown. Seems to be linked to UKEnd2.
        // Might be position in stack.
        //   0: Some players have this, all? NPCs have this.
        //   1: Some players have this.
        //   2: Some players have this.
        //   3: Some players have this.
        //   4: Some players have this.
        //   6: Some players have this.
        //   7: Some players have this.
        //   9: Some players have this.
        //   10: Some players have this.
        //   11: Some players have this.
        //   12: Some players have this.
        //   13: Some players have this.
        public int UKEnd6
        {
            get;
            set;
        }

        // Unknown.
        //   0: Everything(?) has this.
        public int UKEnd7
        {
            get;
            set;
        }

        // Unknown.
        //   0: All(?) players have this.
        //   1: All(?) NPCs, monsters have this.
        public int UKEnd8
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append("Header: " + this.Header + "\t");
            sb.Append("UKHeader: " + this.UKHeader + "\t");
            //sb.Append("ID: " + this.ID + "\t");
            sb.Append("Type: " + this.CreatureType + "\t");
            //sb.Append("CharNameLength: " + this.CharNameLength + "\t");
            sb.Append("CharName: " + this.CharName + "\t");
            //sb.Append("HealthPercent: " + this.HealthPercent + "\t");
            //sb.Append("Direction: " + this.Direction + "\t");
            //sb.Append("Outfit: " + this.Outfit + "\t");
            //sb.Append("CHead: " + this.ColorHead + "\t");
            //sb.Append("CBody: " + this.ColorBody + "\t");
            //sb.Append("CLegs: " + this.ColorLegs + "\t");
            //sb.Append("CFeet: " + this.ColorFeet + "\t");
            //sb.Append("Addons: " + this.Addons + "\t");
            //sb.Append("Mount: " + this.Mount + "\t");
            //sb.Append("LightRadius: " + this.LightRadius + "\t");
            //sb.Append("LightColor: " + this.LightColor + "\t");
            //sb.Append("Speed: " + this.Speed + "\t");
            sb.Append("Skull: " + this.Skull + "\t");
            sb.Append("PartyShield: " + this.PartyShield + "\t");
            sb.Append("GuildShield:" + this.GuildShield + "\t");
            sb.Append("UKEnd1: " + this.UKEnd1 + "\t");
            sb.Append("UKEnd2: " + this.UKEnd2 + "\t");
            sb.Append("UKEnd3: " + this.UKEnd3 + "\t");
            sb.Append("UKEnd4: " + this.UKEnd4 + "\t");
            sb.Append("UKEnd5: " + this.UKEnd5 + "\t");
            sb.Append("UKEnd6: " + this.UKEnd6 + "\t");
            sb.Append("UKEnd7: " + this.UKEnd7 + "\t");
            sb.Append("UKEnd8: " + this.UKEnd8 + "\t");
            return sb.ToString();
        }

        public MapCreature(int header, int ukheader, int id,
            int type, int charNameLength, string charName,
            int healthPercent, int direction, int outfit,
            int colorHead, int colorBody, int colorLegs, int colorFeet,
            int addons, int mount, int lightRadius, int lightColor,
            int speed, int skull, int partyShield, int guildShield,
            int ukend1, int ukend2, int ukend3, int ukend4, int ukend5,
            int ukend6, int ukend7, int ukend8)
        {
            Header = header;
            UKHeader = ukheader;
            ID = id;
            CreatureType = type;
            CharNameLength = charNameLength;
            CharName = charName;
            HealthPercent = healthPercent;
            Direction = direction;
            Outfit = outfit;
            ColorHead = colorHead;
            ColorBody = colorBody;
            ColorLegs = colorLegs;
            ColorFeet = colorFeet;
            Addons = addons;
            Mount = mount;
            LightRadius = lightRadius;
            LightColor = lightColor;
            Speed = speed;
            Skull = skull;
            PartyShield = partyShield;
            GuildShield = guildShield;
            UKEnd1 = ukend1;
            UKEnd2 = ukend2;
            UKEnd3 = ukend3;
            UKEnd4 = ukend4;
            UKEnd5 = ukend5;
            UKEnd6 = ukend6;
            UKEnd7 = ukend7;
            UKEnd8 = ukend8;
        }
    }
}
