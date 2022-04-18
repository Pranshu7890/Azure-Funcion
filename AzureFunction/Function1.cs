using System;
using System.Configuration;
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
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace AzureFunction
{
    public static class Function1
    {
        

        [FunctionName("Function1")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            SqlConnection cnn = new SqlConnection(@"data source=192.168.0.82;initial catalog=MY_April2022;password=abc123;persist security info=True;user id=crmnext;packet size=4096;enlist=false");
            
            List<WitModel> Witlist= AzureWorkitemHelper.wittotalinfo(7964, 2, "Azure CRMnext Sync", cnn);
            log.LogInformation($"Webhook was triggered!");
            HttpContent requestContent = req.Content;
           
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            dynamic data = JObject.Parse(jsonContent);
           string wid= Convert.ToString(data["resource"]["workItemId"]);
           string fields=null;
           if (data["resource"]?["fields"]?["System.State"]?["newValue"]==null)
           {
               fields = "Active";
            }
            else { fields = Convert.ToString(data["resource"]["fields"]["System.State"]["newValue"]); }

           string BoardColumn =null;
            if (String.IsNullOrEmpty(AzureWorkitemHelper.getBoradinfo(Convert.ToInt32(wid)))&& fields.Equals("Proposed"))
            {
                BoardColumn = "Backlog";
            }
            else if (String.IsNullOrEmpty(AzureWorkitemHelper.getBoradinfo(Convert.ToInt32(wid))) && fields.Equals("Closed"))
            {
                BoardColumn = "Closed";
            }
            else
            {
                BoardColumn = AzureWorkitemHelper.getBoradinfo(Convert.ToInt32(wid));
            }
            string BoardColumnDone = null;
            int statusid=0;
            if (data["resource"]?["fields"]?["System.BoardColumnDone"]?["newValue"]==null)
            {
                BoardColumnDone = "False";
            }
            else { BoardColumnDone = Convert.ToString(data["resource"]["fields"]["System.BoardColumnDone"]["newValue"]); }

            foreach (var wit in Witlist)
            {
                if ((wit.iscreated == true) && (wit.Workitemid == wid) && (wit.WorkitemType == WorkitemType.Req))
                {
                    if (BoardColumn == "Backlog")
                    {
                        statusid = 46;
                    }
                    else if ((BoardColumn == "BA") && (BoardColumnDone == "False"))
                    {
                        statusid = 271;
                    }
                    else if ((BoardColumn == "BA") && (BoardColumnDone == "True"))
                    {
                        statusid = 79;
                    }
                    else if ((BoardColumn == "Dev") && (BoardColumnDone == "False"))
                    {
                        statusid = 77;
                    }
                    else if ((BoardColumn == "Dev") && (BoardColumnDone == "True"))
                    {
                        statusid = 78;
                    }
                    else if ((BoardColumn == "QA") && (BoardColumnDone == "True"))
                    {
                        statusid = 73;
                    }
                    else if ((BoardColumn == "QA") && (BoardColumnDone == "True"))
                    {
                        statusid = 129;
                    }
                    else if (BoardColumn == "Closed") { statusid = 31; }


                    SqlCommand command = new SqlCommand(string.Format(@"update IssueRequirementMaster set statusCodeID= '{0}' where ItemId={1}", statusid, wit.WitId), cnn);
                    command.ExecuteNonQuery();



                }



            }






        }
    }
}
