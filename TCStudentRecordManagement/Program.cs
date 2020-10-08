using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace TCStudentRecordManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* TODO: Add parsing of the args string to include the following parameters:
                
            -p --port = run on http port
            -f --log = log to a file
            -d --debug = activate debug mode
            -nd --no-detach = log to console
            
            */
            


            Console.WriteLine("TECHCareers Student Record Management API Server");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console()
                .CreateLogger();
            

            CreateHostBuilder(args).Build().Run();



            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
