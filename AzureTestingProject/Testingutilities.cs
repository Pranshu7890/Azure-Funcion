using System;
using System.Data.SqlClient;
using CRMnextServiceAPI;

using System.Net.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using WebApplication1;

namespace AzureTestingProject
{
    class Testingutilities
    {

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                });
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();


        }
    }
        
}
