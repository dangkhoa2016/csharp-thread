using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Csharp_Process_Main.database
{
    /// <summary>
    /// Summary description for Multiple1
    /// </summary>
    public class Multiple1 : ITest
    {
        List<JObject> _arr = null;
        public Multiple1(List<JObject> arr)
        {
            _arr = arr;
        }

        public void Start()
        {
            if (_arr == null || _arr.Count == 0)
                return;

            try
            {
                List<Thread> threads = new List<Thread>();

                foreach (var item in _arr)
                {
                    Thread thread = new Thread(() =>
                    {
                        Helper.create_user(item).Wait();
                    });

                    thread.Start();
                    threads.Add(thread);
                }

                foreach (Thread thread in threads)
                    thread.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error run_batch:" + ex.Message);
            }
        }
    }
}