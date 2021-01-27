
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Csharp_Process_Main
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(string.Join(",", args));

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please provide project to run");
                return;
            }

            ITest test = null;

            if (args[0].ToLower() == "upload")
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Please provide class to run and number of items.");
                    return;
                }

                string TOKEN = Environment.GetEnvironmentVariable("TOKEN");
                if (string.IsNullOrWhiteSpace(TOKEN))
                    DotNetEnv.Env.Load();

                int number_process = -1;
                int.TryParse(args[2], out number_process);
                if (number_process < 1)
                    number_process = 1;

                test = GetUploadClass(args[1], number_process);
            }
            else if (args[0].ToLower() == "database")
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Please provide class to run.");
                    return;
                }

                database.Helper.set_up(true, false);

                test = GetDatabaseClass(args[1]);
            }

            if (test == null)
            {
                Console.WriteLine("No test class.");
                return;
            }

            var start_time = DateTime.Now;
            Console.WriteLine("Start at: {0}", start_time);

            test.Start();

            var end_time = DateTime.Now;
            Console.WriteLine("End at: {0}", end_time);

            var seconds_diff = (end_time - start_time).TotalSeconds;
            Console.WriteLine("Total: {0} second(s)", seconds_diff);
        }

        static ITest GetUploadClass(string test_number, int number_process)
        {
            ITest test = null;
            switch (test_number)
            {
                case "1":
                    test = new upload.Single(number_process);
                    break;
                case "2.1":
                    test = new upload.Multiple1(number_process);
                    break;
                case "2.2":
                    test = new upload.Multiple2(number_process);
                    break;
                case "2.3":
                    test = new upload.Multiple3(number_process);
                    break;
                case "3.1":
                    test = new upload.Concurrency1(number_process);
                    break;
                case "3.2":
                    test = new upload.Concurrency2(number_process);
                    break;
                default:
                    break;
            }
            return test;
        }

        static ITest GetDatabaseClass(string test_number)
        {
            //read csv
            List<JObject> arr = new List<JObject>();
            string file_path = Path.Combine(Directory.GetCurrentDirectory(), "user.csv");

            using (var reader = new StreamReader(file_path))
            {
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture, hasHeaderRecord: true)))
                {
                    var user_type = new
                    {
                        email = string.Empty,
                        username = string.Empty,
                        point = default(int)
                    };
                    var records = csv.GetRecords(user_type);
                    arr.AddRange(records.Select(x => JObject.FromObject(x)));
                }
            }


            ITest test = null;
            switch (test_number)
            {
                case "1":
                    test = new database.Single(arr);
                    break;
                case "2.1":
                    test = new database.Multiple1(arr);
                    break;
                case "3.1":
                    test = new database.Concurrency1(arr);
                    break;
                case "4.1":
                    test = new database.Lock1(arr);
                    break;
                default:
                    break;
            }
            return test;
        }
    }
}