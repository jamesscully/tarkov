using System.Collections.Generic;

namespace TarkovAssistantWPF.data.models
{
    public class Ammo : BaseDataObjectClass
    {
        public string name;
        public string shortName;
        public float weight;
        public string caliber;
        public int stackMaxSize;
        public bool tracer;
        public string tracerColor;
        public string ammoType;
        public int projectileCount;
        
        // since our data describes ballistics as a dictionary,
        // we'll use functions to return needed fields
        public Dictionary<string, float> ballistics;
        
        public float damage()               { return ballistics["damage"]; }
        public float armorDamage()          { return ballistics["armorDamage"]; }
        public float fragmentationChance()  { return ballistics["fragmentationChance"]; }
        public float ricochetChance()       { return ballistics["ricochetChance"]; }
        public float penetrationChance()    { return ballistics["penetrationChance"]; }
        public float penetrationPower()     { return ballistics["penetrationPower"]; }
        public float accuracy()             { return ballistics["accuracy"]; }
        public float recoil()               { return ballistics["recoil"]; }
        public float initialSpeed()         { return ballistics["initialSpeed"]; }

    }
}