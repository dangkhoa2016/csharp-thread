using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Csharp_Process_Main.upload
{
    /// <summary>
    /// Summary description for Multiple3
    /// </summary>
    public class Multiple3 : ITest
    {
        public Multiple3(int number_process)
        {
            //
            // TODO: Add constructor logic here
            //

            if (number_process < 1)
                number_process = 1;

            Number_Process = number_process;
        }

        public int Number_Process { get; } = -1;

        string TOKEN = Environment.GetEnvironmentVariable("TOKEN");
        string ENDPOINT = Environment.GetEnvironmentVariable("ENDPOINT");
        const int max_allow = 5;

        public void Start()
        {
            if (string.IsNullOrWhiteSpace(ENDPOINT))
            {
                Console.WriteLine("No api endpoint to upload.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TOKEN))
            {
                Console.WriteLine("No token to upload.");
                return;
            }

            List<object> arr = new List<object>();
            for (int i = 0; i < Number_Process; i++)
            {
                arr.Add(i + 1);
            }

            var length = arr.Count;
            var number_batch = (int)Math.Ceiling((decimal)length / max_allow);
            for (int n = 0; n < number_batch; n++)
            {
                var batch = new List<object>();
                var from = n * max_allow;
                var to = from + max_allow;
                for (int i = from; i < to; i++)
                {
                    if (i < length)
                        batch.Add(i + 1);
                    else
                        break;
                }

                run_batch(batch);
            }
        }

        public void run_batch(List<object> arr)
        {
            try
            {
                //new way: using Task
                var tasks = new List<Task>();
                foreach (var item in arr)
                {
                    tasks.Add(Task.Run(async () => { await Helper.Upload_With_Retry(item); }));
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