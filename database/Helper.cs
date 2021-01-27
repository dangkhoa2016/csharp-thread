using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Csharp_Process_Main.database
{
    class Helper
    {
        const string table_name = "user";
        static DbHelper db = new DbHelper("database.sqlite3");
        public static void set_up(bool clear_data, bool seed)
        {
            db.Execute(@"
  CREATE TABLE IF NOT EXISTS user (email TEXT UNIQUE, username TEXT, point INTEGER);
");

            if (clear_data)
            {
                Console.WriteLine("Delete all {0}", table_name);
                db.Execute(string.Format("delete from {0} where rowid > ?", table_name), 0);
            }

            if (seed)
            {
                Console.WriteLine("Create 2 {0}", table_name);
                create_user(JObject.FromObject(new { email = "email_1@test.local", username = "user_1", point = 1 })).Wait();
                create_user(JObject.FromObject(new { email = "email_2@test.local", username = "user_2", point = 2 })).Wait();
            }
        }


        static bool email_exist(string email)
        {
            var arr = db.Detail("email", table_name, "email = ?", email);
            return arr != null && arr.HasValues;
        }

        public static async Task<int> create_user(JObject user)
        {
            if (user == null || user.ContainsKey("email") == false)
                return 0;

            Console.WriteLine("[{0}] Start create {1} {2}", DateTime.Now, table_name, JsonConvert.SerializeObject(user));

            System.Threading.Thread.Sleep(1000);

            if (email_exist(user.Value<string>("email")))
            {
                Console.WriteLine("Email {0} already existed.", user["email"]);
                return 0;
            }

            return await process_create(user);
        }

        static Task<int> process_create(JObject user)
        {
            return Task.Run(() =>
            {
                System.Threading.Thread.Sleep(new Random().Next(4));

                try
                {
                    int u_id = db.Create(table_name, "?, ?, ?",
                        user.Value<string>("email"), user.Value<string>("username"), user.Value<int>("point"));
                    Console.WriteLine("Created id: {0}", u_id);
                    return u_id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return 0;
                }
            });
        }

        /*
        static readonly object sync_create = new object();
        public static Task<int> create_user_with_lock(JObject user)
        {
            return Task.Run(() =>
            {
                int u_id = 0;

                lock (sync_create)
                {
                    u_id = create_user(user).Result;
                }

                return u_id;
            });
        }
        */

        static SemaphoreSlim sync_create = new SemaphoreSlim(1);
        public static Task<int> create_user_with_lock(JObject user)
        {
            return Task.Run(async () =>
            {
                await sync_create.WaitAsync();
                try
                {
                    return await create_user(user);
                }
                finally
                {
                    sync_create.Release();
                }
            });
        }
    }
}
