namespace TarkovAssistantWPF.data
{
    public class BaseDataObjectClass
    {
        public object id;
        
        // Since our data source uses both string and int ids,
        // we can overload the hashcode to unify ID lookups.
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}