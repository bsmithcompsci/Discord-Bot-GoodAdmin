using Discord;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace GoodAdmin.Core
{
    public class ConfigC
    {
        public string TOKEN { get; set; }
        public string PREFIX { get; set; }
    }

    public class Config
    {
        public static ConfigC config;
        public static Task LoadGlobalConfig()
        {
            string configRaw = "";
            if (!File.Exists("./gConfig.json"))
            {
                configRaw = JsonConvert.SerializeObject(new ConfigC());
                File.WriteAllText("./gConfig.json", configRaw);
            }
            else
                configRaw = File.ReadAllText("./gConfig.json");
            config = JsonConvert.DeserializeObject<ConfigC>(configRaw);

            return Task.CompletedTask;
        }

        public static Task LoadGuildConfig(IGuild guild)
        {

            return Task.CompletedTask;
        }
    }
}
