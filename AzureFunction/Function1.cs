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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Azurefunction")] HttpRequestMessage req,
            ILogger log)
        {
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
            log.LogInformation(BoardColumn+" "+BoardColumnDone);
            
        if (BoardColumn == "Backlog")
                    {
                        statusid = 66;
                    }
                    else if ((BoardColumn == "BA") && (BoardColumnDone == "False"))
                    {
                        statusid = 62;
                    }
                    else if ((BoardColumn == "BA") && (BoardColumnDone == "True"))
                    {
                        statusid = 100003;
                    }
                    else if ((BoardColumn == "Dev") && (BoardColumnDone == "False")&&(AzureWorkitemHelper.getQAfailure(Convert.ToInt32(wid))==false))
                    {
                        statusid = 77;
                    }
                    else if ((BoardColumn == "Dev") && (BoardColumnDone == "True"))
                    {
                        statusid = 78;
                    }
                    else if ((BoardColumn == "QA") && (BoardColumnDone == "False"))
                    {
                        statusid = 129;
                    }
                    else if ((BoardColumn == "QA") && (BoardColumnDone == "True"))
                    {
                        statusid = 73;
                    }
                    else if (AzureWorkitemHelper.getQAfailure(Convert.ToInt32(wid)))
                    {
                        statusid = 420;
                    }
                   
                    else if (BoardColumn == "Closed") { statusid = 31; }
                    else if (AzureWorkitemHelper.getQAfailure(Convert.ToInt32(wid))==false) { statusid = 78; }
                    log.LogInformation(Convert.ToString(statusid));
            string widquery = string.Format(@"select Req_ex4_Id from req_ex4 where req_ex4_1='{0}')", wid);
            string myid;
            using (System.Data.DataTableReader reader = QueryExecuter.ReadQuery(widquery))
            {
                while (reader.Read())
                {
                    if (reader.HasRows) {
                        myid=String.Format("{0}", reader["Req_ex4_Id"]);
                        string command = string.Format(@"update IssueRequirementMaster set statusCodeID= '{0}' where ItemId={1}", statusid, myid);
                        QueryExecuter.updateQuery(command);
                    }
                    else
                    {
                        log.LogInformation("item is not created");
                    }
                    
                }
            }
                    
                    
        }
    }
}
