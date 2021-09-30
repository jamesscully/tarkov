namespace TarkovAssistantWPF.data.models
{
    public class Item : BaseDataObjectClass
    {
        public string name = "undefined";
        public string shortName = "undefined";

        public override string ToString()
        {
            return $"item: {name}, {shortName}; id = {id}";
        }
    }
}