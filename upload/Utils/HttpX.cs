using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;


namespace Utils
{
    public class HttpX
    {

        public static async Task<string> Post(string url, string parameters, string token = "")
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Can not post empty data.");
                return string.Empty;
            }

            return await Task.Run(() =>
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;
                if (string.IsNullOrWhiteSpace(token) == false)
                    webRequest.Headers.Add("token", token);
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36 OPR/73.0.3856.284";
                Stream postStream = webRequest.GetRequestStream();
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();
                WebResponse response = webRequest.GetResponse();
                postStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(postStream);
                return reader.ReadToEnd();
            });
        }


        public static async Task<string> Get(string url, string parameters = "")
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(parameters) == false)
                    url += "?" + parameters;
                Console.WriteLine("Get " + url + " : " + DateTime.Now);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "Get";
                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd();
                }
            });
        }


        public static async Task<bool> Delete(string url, string parameters = "")
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(parameters) == false)
                    url += "?" + parameters;
                Console.WriteLine("Delete " + url + " : " + DateTime.Now);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "Delete";
                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                    return response.StatusCode == HttpStatusCode.NoContent;
            });
        }


        public static async Task<object> GetObject(string url, string parameters = "")
        {
            string s = await Get(url, parameters);
            object result = null;
            try
            {
                result = SimpleJson.DeserializeObject(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

    }
}