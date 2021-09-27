using System.Collections.Generic;

namespace TarkovAssistantWPF.data.models
{
    public class Quest : BaseDataObjectClass
    {
        public int giver;
        public int turnin;
        public string title;

        public string wiki;

        public int exp;
        public string[] unlocks;

        public Quest.Requirements require;
        public Quest.Reputation[] reputation;
        public Quest.Objective[] objectives;

        public string gameId;

        public Dictionary<string, string> locales;

        public Quest.Reputation[] reputationFailure;
        public int[] alternatives;
        public bool nokappa;

        public class Objective
        {
            public string type;
            public string tool;
            public object target;
            public string hint;
            public string[] with;
            public int number;
            public int location;
            public int id;
            public int have;
        }

        public class Reputation
        {

            public int trader;
            public float rep;
        }

        public class Requirements
        {
            public int level;
            public int[] quests;
            
            public class Loyalty
            {
                public int trader;
                public int stage;
            }
            
            public Loyalty[] loyalty;
        }
        
        public override string ToString()
        {
            float x = -1;
            if (reputation.Length > 0)
            {
                x = reputation[0].rep;
            }
            return $"{title}, from {giver}, {wiki} {x}";
        }

    }
}