using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace GoodAdmin.Core
{
    struct Global_Configurations
    {
        public string token;
    }

    struct LocalGuild_Configurations
    {

    }

    public class configC
    {
        public string TOKEN { get; set; }
        public string PREFIX { get; set; }
    }

    public class Config
    {
        public static configC config;
        public static async Task LoadGlobalConfig()
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


        }

        public static async Task LoadGuildConfig(IGuild guild)
        {

        }
    }
}
