using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Servize.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Servize
{
    public class Program
    {   
        public static void Main(string[] args)
        {
            try
            {
                Logger.LogInformation(0, "Servize.Com Application Started");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Logger.LogFatal(e);
                Console.WriteLine(e);
            }
            finally {
                Logger.Flush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(logger:Logger.GetLogger())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
