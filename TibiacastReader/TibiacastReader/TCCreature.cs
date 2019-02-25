using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiacastReader
{
    class TCCreature
    {
        public long creatureID = 0;
        public int unknown1 = 0;
        public int charNameLength = 0;
        public int unknown2 = 0;
        public string charName = "";
        public int healthpercent = 0;
        public int direction = 0;
        public int outfit = 0;
        public int colorHead = 0;
        public int colorBody = 0;
        public int colorLegs = 0;
        public int colorFeet = 0;
        public int addons = 0;
        public int mount = 0;
        public int lightRadius = 0;
        public int lightColor = 0;
        public int speed = 0;
        public int skull = 0;
        public int partyShield = 0;
        public int guildShield = 0;
        public int unknown3 = 0;

        public TCCreature(long id, int unknown1, int charNameLength, int unknown2, string charName, int healthPercent, int direction, int outfit, int colorHead, int colorBody, int colorLegs, int colorFeet, int addons, int mount, int lightRadius, int lightColor, int speed, int skull, int partyShield, int guildShield, int unknown3)
        {
            this.creatureID = id;
            this.unknown1 = unknown2;
            this.charNameLength = charNameLength;
            this.unknown2 = unknown2;
            this.charName = charName;
            this.healthpercent = healthPercent;
            this.direction = direction;
            this.outfit = outfit;
            this.colorHead = colorHead;
            this.colorBody = colorBody;
            this.colorLegs = colorLegs;
            this.colorFeet = colorFeet;
            this.addons = addons;
            this.mount = mount;
            this.lightRadius = lightRadius;
            this.lightColor = lightColor;
            this.speed = speed;
            this.skull = skull;
            this.partyShield = partyShield;
            this.guildShield = guildShield;
            this.unknown3 = unknown3;
        }
    }
}
