using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Csharp_Process_Main.upload
{
    /// <summary>
    /// Summary description for Concurrency1
    /// </summary>
    public class Concurrency1 : ITest
    {
        public Concurrency1(int number_process)
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

        readonly object sync_get = new object();
        public void run_batch(List<object> arr)
        {
            int index = -1;
            Func<object> GetItem = () =>
            {
                lock (sync_get)
                {
                    index += 1;
                    if (index < arr.Count)
                        return arr[index];

                    return null;
                }
            };

            Task.Run(async () =>
            {
                try
                {
                    List<Task<bool>> tasks = new List<Task<bool>>();
                    object item = null;
                    for (int i = 0; i < max_allow; i++)
                    {
                        item = GetItem();
                        if (item != null)
                            tasks.Add(Helper.Upload(item));
                        else
                            break;
                    }

                    while (tasks.Count > 0)
                    {
                        var finishedTask = await Task.WhenAny(tasks);
                        tasks.Remove(finishedTask);

                        item = GetItem();
                        if (item != null)
                            tasks.Add(Helper.Upload(item));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error run_batch:" + ex.Message);
                }
            }).Wait();
        }
    }
}