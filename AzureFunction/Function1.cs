using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using AzureTestingProject;
namespace AzureFunction
{
    public static class Function1
    {
        
        [FunctionName("Function1")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {

            log.LogInformation($"Webhook was triggered!");
            HttpContent requestContent = req.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            string resource = jsonContent.Substring(jsonContent.IndexOf("resource"));
            }
    }
}
