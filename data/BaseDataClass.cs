using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{
    public class BaseDataClass<T>
    {
        
       public Dictionary<int, T> allData;

       public JObject json;
       public string DATA_LOCATION = "undefined";
       
       private IJEnumerable<JToken> GetParseEntryPoint()
       {
           return json.Children().Children();
       }
        
        public void Load()
        {
            allData = new Dictionary<int, T>();
            
            json = JObject.Parse(File.ReadAllText(DATA_LOCATION));
            
            foreach (JToken child in GetParseEntryPoint())
            {
                T data = JsonConvert.DeserializeObject<T>(child.ToString());
                allData.Add(data.GetHashCode(), data);
            }
        }
        
        public void Load(Func<T, bool> forEachHook)
        {
            allData = new Dictionary<int, T>();
            
            json = JObject.Parse(File.ReadAllText(DATA_LOCATION));
            
            foreach (JToken child in GetParseEntryPoint())
            {
                T data = JsonConvert.DeserializeObject<T>(child.ToString());
                allData.Add(data.GetHashCode(), data);
                forEachHook(data);
            }
        }
        

        public void PrintAll()
        {
            foreach (var keyValuePair in allData)
            {
                Console.WriteLine(keyValuePair.Value);
            }
        }

        public T GetById(int id)
        {
            return allData[id.GetHashCode()];
        }
    }
}