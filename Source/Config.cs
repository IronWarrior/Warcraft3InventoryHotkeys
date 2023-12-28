using Newtonsoft.Json;

namespace Warcraft3InventoryHotkeys.Source
{
    public class Config
    {
        public Point WindowLocation;

        private const string name = "config.json";

        public static Config Load()
        {
            if (!File.Exists(name))
            {
                return new();
            }
            else
            {
                string json = File.ReadAllText(name);                
                return JsonConvert.DeserializeObject<Config>(json);
            }
        }

        public static void Save(Config config)
        {
            string json = JsonConvert.SerializeObject(config);

            File.WriteAllText(name, json);
        }
    }
}
