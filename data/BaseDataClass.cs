using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{
    public class BaseDataClass<T>
    {
        
       public Dictionary<int, T> Data;

       public JObject json;
       public string DATA_LOCATION = "undefined";
       
       
       // Determines the 'root' point where we can loop over to get each data point.
       // Override if certain json file does not work with this class' approach
       protected IJEnumerable<JToken> GetParseEntryPoint()
       {
           return json.Children().Children();
       }

       public void Load()
        {
            Data = new Dictionary<int, T>();
            
            json = JObject.Parse(File.ReadAllText(DATA_LOCATION));
            
            foreach (JToken child in GetParseEntryPoint())
            {
                T data = JsonConvert.DeserializeObject<T>(child.ToString());
                Data.Add(data.GetHashCode(), data);
            }
        }
        
        
        // Loads data from each json file into a main data dictionary, with primary ID's converted to hashcode for lookup.
        // forEachHook - lambda used to load or perform tasks in derived classes for each json token.
        public void Load(Func<T, bool> forEachHook)
        {
            Data = new Dictionary<int, T>();
            
            json = JObject.Parse(File.ReadAllText(DATA_LOCATION));
            
            foreach (JToken child in GetParseEntryPoint())
            {
                T data = JsonConvert.DeserializeObject<T>(child.ToString());
                Data.Add(data.GetHashCode(), data);
                forEachHook(data);
            }
        }
        

        public void PrintAll()
        {
            foreach (var keyValuePair in Data)
            {
                Console.WriteLine(keyValuePair.Value);
            }
        }

        public T GetById(int id)
        {
            return Data[id.GetHashCode()];
        }
    }
}