using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Csharp_Process_Main.upload
{
    /// <summary>
    /// Summary description for Single
    /// </summary>
    public class Single : ITest
    {
        public Single(int number_process)
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
                foreach (var item in arr)
                {
                    Helper.Upload(item).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error run_batch:" + ex.Message);
            }
        }
    }
}