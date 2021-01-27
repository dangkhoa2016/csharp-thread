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
    /// Summary description for Concurrency1
    /// </summary>
    public class Concurrency1 : ITest
    {
        List<JObject> _arr = null;
        public Concurrency1(List<JObject> arr)
        {
            _arr = arr;
        }

        const int max_allow = 3;

        public void Start()
        {
            if (_arr == null || _arr.Count == 0)
                return;

            try
            {
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(max_allow))
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var item in _arr)
                    {
                        semaphoreSlim.Wait();

                        var t = Task.Run(async () =>
                        {
                            try
                            {
                                await Helper.create_user(item);
                            }
                            finally
                            {
                                semaphoreSlim.Release();
                            }
                        });

                        tasks.Add(t);
                    }

                    Task.WaitAll(tasks.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error run_batch:" + ex.Message);
            }
        }
    }
}