using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace GoodAdmin_API.Core.GameViewers.FiveM
{
    public class ServerViewer
    {
        public static async Task<object> GetAllServers()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://servers-live.fivem.net/api/servers/");
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

                return JsonConvert.DeserializeObject(await reader.ReadToEndAsync());
            }
        }

        public static async Task<object> GetServerInfo(string ipAddress)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://"+ipAddress+"/info.json");
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

                return JsonConvert.DeserializeObject(await reader.ReadToEndAsync());
            }
        }

        public static async Task<object> GetPlayersInfo(string ipAddress)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + ipAddress + "/players.json");
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

                return JsonConvert.DeserializeObject(await reader.ReadToEndAsync());
            }
        }
    }
}
/*
    USE `database`;
    SELECT * FROM `table`
    INSERT INTO `table` () VALUES ()
    DELETE 
    CREATE
     
*/
