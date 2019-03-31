using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Database
{
    public class SQL
    {
        private static string connectionString = "";

        /// <summary>
        /// Will connect to the database, either by creating a new one or by purely connecting to an existing one.
        /// </summary>
        /// <returns></returns>
        public static Task Verify(string filename, string path = "")
        {
            string reference = Path.Combine(Path.GetFullPath("./"), path) + filename + ".sqlite";
            if (!File.Exists(reference))
                SQLiteConnection.CreateFile(reference);

            SQL.connectionString = "Data Source=" + reference;

            return Task.CompletedTask;
        }

        public static int Execute(string query, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return new Execute(SQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<int> ExecuteAsync(string query, Action<int> callback, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return await new Execute(SQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static List<Dictionary<string, object>> FetchAll(string query, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return new FetchAll(SQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<List<Dictionary<string, object>>> FetchAllAsync(string query, Action<List<Dictionary<string, object>>> callback, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return await new FetchAll(SQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static object FetchScalar(string query, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return new FetchScalar(SQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<object> FetchScalarAsync(string query, Action<object> callback, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return await new FetchScalar(SQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static object Insert(string query, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return new Insert(SQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<object> InsertAsync(string query, Action<object> callback, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return await new Insert(SQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }
    }
}
