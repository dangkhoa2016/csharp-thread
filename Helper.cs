using Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;

namespace Csharp_Process_Main
{
    /// <summary>
    /// Summary description for Helper
    /// </summary>
    public class Helper
    {
        public Helper()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static string QueryString(IDictionary<string, object> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
                list.Add(item.Key + "=" + Uri.EscapeDataString(item.Value.ToString()));
            return string.Join("&", list);
        }

        static string TOKEN = Environment.GetEnvironmentVariable("TOKEN");
        static string ENDPOINT = Environment.GetEnvironmentVariable("ENDPOINT");

        public static async Task<bool> Upload(object index)
        {
            Console.WriteLine("[{0}] Start post file {1}", DateTime.Now, index);
            //Console.WriteLine("[{0}] {1}", TOKEN, ENDPOINT);

            try
            {
                Dictionary<string, object> list_params = new Dictionary<string, object>();
                list_params.Add("client", "csharp");
                list_params.Add("name", string.Format("file {0}", index));
                list_params.Add("file", "content");
                string result = await HttpX.Post(string.Format("{0}/api/upload", ENDPOINT), QueryString(list_params), TOKEN);
                Console.WriteLine(result);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error get data: " + ex.Message);

                if (ex is WebException)
                {
                    using (var errorResponse = (HttpWebResponse)(ex as WebException).Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();
                            Console.WriteLine(error);
                        }
                    }
                }

                return false;
            }
        }


        public static async Task<bool> Upload_With_Retry(object index)
        {
            Console.WriteLine("[{0}] Start post file {1}", DateTime.Now, index);
            //Console.WriteLine("[{0}] {1}", TOKEN, ENDPOINT);

            Func<Task<bool>> Start = async () =>
            {
                Dictionary<string, object> list_params = new Dictionary<string, object>();
                list_params.Add("client", "csharp");
                list_params.Add("name", string.Format("file {0}", index));
                list_params.Add("file", "content");
                string result = await HttpX.Post(string.Format("{0}/api/upload", ENDPOINT), QueryString(list_params), TOKEN);
                Console.WriteLine(result);

                return true;
            };

            try
            {
                return await Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error get data: " + ex.Message);

                if (ex is WebException)
                {
                    WebException webException = ex as WebException;
                    using (var errorResponse = (HttpWebResponse)webException.Response)
                    {
                        try
                        {
                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                string error = reader.ReadToEnd();
                                Console.WriteLine(error);
                            }

                            string retry = errorResponse.Headers.Get("retry-after");
                            if (string.IsNullOrWhiteSpace(retry) == false)
                            {
                                var number_seconds = int.Parse(retry) + 1;
                                Console.WriteLine("[{0}] Wait for limit time: {1}", index, number_seconds);
                                System.Threading.Thread.Sleep(number_seconds * 1000);

                                return Start().Result;
                            }
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("Error read response: " + ex2.Message);
                        }
                    }
                }

                return false;
            }
        }
    }
}