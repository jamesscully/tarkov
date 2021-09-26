using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TarkovAssistantWPF.interfaces;

namespace TarkovAssistantWPF.data
{
    public class BaseDataClass<T>
    {
        public readonly object id;
        public Dictionary<int, T> Data = new Dictionary<int, T>();

       public JToken json;
       public string DATA_LOCATION = "undefined";
       
       
       // Determines the 'root' point where we can loop over to get each data point.
       // Override if certain json file does not work with this class' approach
       virtual public IJEnumerable<JToken> GetParseEntryPoint()
       {
           Console.WriteLine("Stuff: " + json.Children());
           return json.Children().Children();
       }

       virtual public void Load()
        {
            Data = new Dictionary<int, T>();
            
            json = JObject.Parse(File.ReadAllText(DATA_LOCATION));
            
            foreach (JToken child in GetParseEntryPoint())
            {
                T data = JsonConvert.DeserializeObject<T>(child.ToString());
                Console.WriteLine("Loading data: " + data);
                Data.Add(data.GetHashCode(), data);
            }
        }
        
        
        // Loads data from each json file into a main data dictionary, with primary ID's converted to hashcode for lookup.
        // forEachHook - lambda used to load or perform tasks in derived classes for each json token.
        virtual public void Load(Func<T, bool> forEachHook)
        {
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
        
        public T GetById(string id)
        {
            return Data[id.GetHashCode()];
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}