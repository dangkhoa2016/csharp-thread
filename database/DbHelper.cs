using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SQLite;

namespace Csharp_Process_Main.database
{
    class DbHelper
    {

        SQLiteConnection db = null;

        string _dbname = string.Empty;
        public DbHelper(string dbname)
        {
            _dbname = dbname;
            db = new SQLiteConnection(Path.Combine(Folder, dbname));
        }


        string Folder
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }


        public int Execute(string sql, params object[] args)
        {
            return db.Execute(sql, args);
        }

        public JArray Detail(string select_fields, string table, string where_conditions, params object[] values)
        {
            var obj = db.Query<dynamic>(string.Format("select {0} from {1} where {2};", select_fields, table, where_conditions), values);
            if (obj != null)
                return JArray.FromObject(obj);
            else
                return null;
        }

        public int Create(string table_with_columns, string columns, params object[] values)
        {
            db.ExecuteScalar<int>(string.Format("insert into {0} values ({1});select last_insert_rowid();", table_with_columns, columns),
                 values);
            return db.ExecuteScalar<int>("select last_insert_rowid()");
        }

        public int Delete(string table, string where_conditions, params object[] values)
        {
            return db.ExecuteScalar<int>(string.Format("delete from {0} where {1};", table, where_conditions),
                values);
        }

        public int Update(string table, string set_conditions, string where_conditions, params object[] values)
        {
            return db.ExecuteScalar<int>(string.Format("update {0} set {1} where {2};", table, set_conditions, where_conditions),
                values);
        }

    }
}
