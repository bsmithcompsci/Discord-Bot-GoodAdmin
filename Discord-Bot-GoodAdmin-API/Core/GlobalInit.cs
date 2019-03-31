using GoodAdmin_API.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core
{
    public class GlobalInit
    {
        public static Controllers.ControllerHandler controllerHandler;

        public static void Init()
        {
            controllerHandler = new Controllers.ControllerHandler();
        }

        private async Task test()
        {
            int uid = 1;
            await SQL.ExecuteAsync($"SELECT * FROM mytable WHERE uid=@uid", (x) => { }, new List<System.Data.SQLite.SQLiteParameter>() {
                new System.Data.SQLite.SQLiteParameter() {
                    ParameterName = "@uid",
                    Value = uid
            } });
            // SELECT * FROM mytable WHERE uid=1; //DELETE DATABASES;
        }

    }
}
