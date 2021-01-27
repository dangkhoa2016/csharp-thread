using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Csharp_Process_Main.upload
{
    /// <summary>
    /// Summary description for Concurrency2
    /// </summary>
    public class Concurrency2 : ITest
    {
        public Concurrency2(int number_process)
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

            run_batch(arr);
        }

        public void run_batch(List<object> arr)
        {
            using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(max_allow))
            {
                List<Task> tasks = new List<Task>();
                foreach (var item in arr)
                {
                    semaphoreSlim.Wait();

                    var t = Task.Run(async () =>
                    {
                        try
                        {
                            await Helper.Upload(item);
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
    }
}