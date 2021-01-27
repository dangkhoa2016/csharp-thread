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
    /// Summary description for Single
    /// </summary>
    public class Single : ITest
    {
        List<JObject> _arr = null;
        public Single(List<JObject> arr)
        {
            _arr = arr;
        }

        public void Start()
        {
            if (_arr == null || _arr.Count == 0)
                return;

            try
            {
                foreach (var item in _arr)
                {
                    Helper.create_user(item).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error run_batch:" + ex.Message);
            }
        }
    }
}