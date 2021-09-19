namespace TarkovAssistantWPF.data
{
    
    
    public class QuestData
    {
        private QuestData _instance;

        private QuestData()
        {
            
        }

        public QuestData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new QuestData();
            }

            return _instance;
        }
    }
}