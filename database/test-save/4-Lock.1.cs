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
    /// Summary description for Lock1
    /// </summary>
    public class Lock1 : ITest
    {
        List<JObject> _arr = null;
        public Lock1(List<JObject> arr)
        {
            _arr = arr;
        }

        public void Start()
        {
            if (_arr == null || _arr.Count == 0)
                return;

            try
            {
                var tasks = new List<Task>();
                foreach (var item in _arr)
                {
                    tasks.Add(Helper.create_user_with_lock(item));
                }

                Task.WhenAll(tasks).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error run_batch:" + ex.Message);
            }
        }
    }
}