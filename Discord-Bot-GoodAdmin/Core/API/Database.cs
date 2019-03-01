using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin.Core.API
{
    public class Database
    {
        private static SQLiteConnection con;

        /// <summary>
        /// Will connect to the database, either by creating a new one or by purely connecting to an existing one.
        /// </summary>
        /// <returns></returns>
        public static async Task Connect()
        {
            if (!File.Exists("./localdb.sqlite"))
            {
                con = new SQLiteConnection();
                SQLiteConnection.CreateFile(Path.GetFullPath("./") + "localdb.sqlite");
            }
            else
                using (con = new SQLiteConnection("Data Source=" + Path.GetFullPath("./") + "localdb.sqlite"))
                    await con.OpenAsync();
        }

        /// <summary>
        /// Running a direct SQL Command to the Database.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static async Task Query(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, con);
            await cmd.ExecuteNonQueryAsync();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// TABLES
        /// <summary>
        /// Creates a Safe Table, that will not override another table.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static async Task CreateTable(string name, string[] objects)
        {
            await Database.Query("CREATE TABLE " + name + " (" + String.Join(",", objects) + ")");
        }

        /// <summary>
        /// Removes a Table that exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task RemoveTable(string name)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Checks if the Table exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task TableExists(string name)
        {
            await Task.CompletedTask;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// ROWS
        /// <summary>
        /// Appends a row into an existing table with its keys and values.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static async Task AppendRow(string table, string[] keys, object[] values)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Removes a row from the table by it's id.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task RemoveRow(string table, uint id)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Modifies an existing table by it's id, with all of it's keys and values.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static async Task ModifyRow(string table, uint id, string[] keys, object[] values)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Finds a row with in a table that have similiar keys and values.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static async Task FindRow(string table, string[] keys, object[] values)
        {
            await Task.CompletedTask;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// Databases/Schemas
        /// <summary>
        /// Creates a new Database/Schema.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task CreateSchema(string name)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Removes a Database/Schema.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task RemoveSchema(string name)
        {
            await Task.CompletedTask;
        }
    }
}
