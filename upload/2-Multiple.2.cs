using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Csharp_Process_Main
{
    /// <summary>
    /// Summary description for Multiple2
    /// </summary>
    public class Multiple2 : ITest
    {
        public Multiple2(int number_process)
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

            run_batch(arr);
        }

        public void run_batch(List<object> arr)
        {
            try
            {
                /* old way: using Thread
                List<Thread> threads = new List<Thread>();

                foreach (var item in arr)
                {
                    Thread thread = new System.Threading.Thread(() =>
                    {
                        Helper.Upload_With_Retry(item).Wait();
                    });

                    thread.Start();
                    threads.Add(thread);
                }

                foreach (Thread thread in threads)
                    thread.Join();
                */

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