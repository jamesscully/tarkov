namespace TarkovAssistantWPF.data.models
{
    public class Trader
    {
        public enum NameID
        {
            PRAPOR = 0,
            THERAPIST,
            SKIER,
            PEACEKEEPER,
            MECHANIC,
            RAGMAN,
            JAEGER,
            FENCE
        }
        public class TraderLoyalty
        {
            public int level = -1;
            public int requiredLevel = -1;
            public float requiredReputation = -1.0f;
            public int requiredSales = -1;
        }
        
        public int id = -1;
        public string name = "undefined";
        public string locale = "undefined";
        public string wiki_url = "undefined";

        public string description = "undefined";

        public string[] currencies;

        public TraderLoyalty[] loyalty;


        public override string ToString()
        {
            return $"trader: {name}";
        }
    }
}