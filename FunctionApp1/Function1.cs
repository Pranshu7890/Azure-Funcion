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
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp1
{
    public static class Function1

    {

        [FunctionName("HttpExample")]
    public static async void run([httptrigger(authorizationlevel.anonymous, "get", "post", route = null)], httprequestmessage req,ilogger log)
      {

    log.loginformation($"webhook was triggered!");
    httpcontent requestcontent = req.content;
  string jsoncontent = requestcontent.readasstringasync().result;
    string resource = jsoncontent.substring(jsoncontent.indexof("resource"));
          int ind = resource.indexof("reason") + 9;
         int ind2 = resource.indexof("states") - 3;
         string reason = resource.substring(ind, ind2 - ind);
          ind = resource.indexof("states") + 9;
           ind2 = resource.indexof("drop") - 3;
           string status = resource.substring(ind, ind2 - ind);
           log.loginformation("reason for build: " + reason);
            log.loginformation("build status: " + status);
      }


    }
}
