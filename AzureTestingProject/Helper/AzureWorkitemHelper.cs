using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Data.SqlClient;

namespace AzureTestingProject
{
   public  class AzureWorkitemHelper
    {

        
        public void CreateWorkitems(int Projectid, int OwnerId, string Relatedtoname)

        {
             int Relatedto;
            string projectworkitemid = null;
            string wid = null;
            List <WitModel> Witinfolist= new List<WitModel>();
            List<string> Workitemid = new List<string>();
            SqlConnection cnn;
             cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        
            
            WorkItemTrackingHttpClient witClient = Projectinfo.connection.GetClient<WorkItemTrackingHttpClient>();
            var newitemdocument = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
            cnn.Open();
            try { 
            
            WorkItem newworkitem = null;

                //Feature
                Relatedto = Projectid;
                WorkItem updateworkitem = null;
                ////Requirement
               
                DBQueries.QueryProject(Witinfolist, Projectid, OwnerId, cnn);
                DBQueries.fetchworkidEpic(Relatedto, projectworkitemid, Witinfolist, Workitemid, cnn);
                Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid, cnn);
                Witinfolist.Clear();
                Workitemid.Clear();

                DBQueries.QueryFeature(Witinfolist, Relatedtoname, OwnerId, cnn);
                wid = DBQueries.fetchworkidProject(Relatedto, projectworkitemid, Witinfolist, Workitemid, cnn).First();
                Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid, cnn);
                Witinfolist.Clear();
                Workitemid.Clear();

                DBQueries.QueryRequirement(Witinfolist, Projectid, OwnerId, cnn, Relatedtoname);
                DBQueries.Fetchworkidreqaz(Witinfolist, wid, Workitemid, cnn);
                Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid, cnn);
                Witinfolist.Clear();
                Workitemid.Clear();

                DBQueries.QueryBug(Witinfolist, Projectid, OwnerId, cnn, Relatedtoname);
                DBQueries.Fetchworkidcasaz(Witinfolist, wid, Workitemid, cnn);
                Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid, cnn);
                Witinfolist.Clear();
                Workitemid.Clear();


            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.Message);

            }
        }


        private void Addworkitem( WorkItemTrackingHttpClient witClient, Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument newitemdocument, out WorkItem newworkitem, out WorkItem updateworkitem, List<WitModel> witinfolist, List<string> Workitemlist, SqlConnection cnn)
        {
            string query = null;
            newworkitem = null;
            updateworkitem = null;
            var documentaddrelepic = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
            int count = Workitemlist.Count;
           
            var Heirerchylist = Enumerable.Range(0, count).Select(i => (new KeyValuePair<string, WitModel>(Workitemlist[i], witinfolist[i]))).ToList();


            foreach (var Workitem in Heirerchylist)
            {
                try
                {
                    var documentaddrelmodule = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
                    documentaddrelepic.Add(
                   new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
                   {
                       Path = "/relations/-",
                       Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                       Value = new
                       {
                           rel = "System.LinkTypes.Hierarchy-Reverse",
                           url = "https://dev.azure.com/acidaes/Test" + "/_apis/wit/workItems/" + Workitem.Key,
                           attributes = new
                           {
                               description = "Parent child relation established"
                           } 
                       }
                   });

                    if ((Workitem.Value.WorkitemType == WorkitemType.Bug)&&(Workitem.Value.iscreated==false))
                    {
                        newworkitem = NewWorkitem( "Bug", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "Bug Workitem created");
                        setDatafields(witClient, cnn, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), updateworkitem.Id);
                        Workitem.Value.Workitemid = updateworkitem.Id.ToString();
                    }

                    else if ((Workitem.Value.WorkitemType == WorkitemType.Req)&&(Workitem.Value.iscreated==false))
                    {
                        newworkitem = NewWorkitem("Requirement", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "Feature Workitem created");
                        setDatafields(witClient, cnn, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), updateworkitem.Id);
                    }
                    else if ((Workitem.Value.WorkitemType == WorkitemType.Module)&&(Workitem.Value.iscreated==false))
                    {
                        newworkitem = NewWorkitem( "Feature", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "feature Workitem created");
                        setDatafields(witClient, cnn, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified, Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), updateworkitem.Id);
                    }



                    else if ((Workitem.Value.WorkitemType == WorkitemType.Project) && (Workitem.Value.iscreated==false))
                    {
                        newworkitem = NewWorkitem( "Epic", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        Console.WriteLine(newworkitem.Id.ToString() + " " + "New Workitem created");
                        setDatafields(witClient, cnn, "Success", Convert.ToString(newworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), newworkitem.Id);
                    }
                    newitemdocument.Clear();

                    documentaddrelepic.Clear();

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);   
                }
                if (Heirerchylist.Count == 0) { Console.WriteLine("Workitem already exist"); }

            }
        }
        private static WorkItem NewWorkitem(string type, WorkItemTrackingHttpClient witClient, Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument newitemdocument, string projectname)
        {
            newitemdocument.Add(
                            new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
                            {
                                Path = "/fields/System.Title",

                                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                                Value = projectname
                            }
                            );
            newitemdocument.Add(new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
            {
                Path = "/fields/System.Description",
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                Value = "this is new workitemcreated"
            });
            var newworkitem = witClient.CreateWorkItemAsync(newitemdocument, "Test", type).Result;
            
            return newworkitem;
        }
          public static string getBoradinfo(int wid)
        {
            string board = null;
            Wiql wiqlQuery = new Wiql

            {
                Query = string.Format(@"SELECT [System.Id] ,[System.BoardColumn] FROM workitems WHERE [System.TeamProject] = 'Test' and   [System.Id] = '{0}'", wid)
            };
            WorkItemTrackingHttpClient witClient = Projectinfo.connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemQueryResult tasks = witClient.QueryByWiqlAsync(wiqlQuery).Result;
            IEnumerable<WorkItemReference> workItemReference = tasks.WorkItems.OrderBy(item => item.Id);
            List<WorkItem> tasksList = witClient.GetWorkItemsAsync(workItemReference.Select(itemID => itemID.Id)).Result;
            foreach (var task in tasksList)
            {


                 board= task.Fields["System.BoardColumn"].ToString();


            }
            return board;

        }
         
        private static async void setDatafields(WorkItemTrackingHttpClient witClient,SqlConnection cnn,string status ,string newworkitemId,string Lastmodified, WorkitemType workitemType, int myid, int? id)
        {
            string querycas = @"Update cas_ex6 set cas_ex6_9 ='{0}', cas_ex6_7 = '{3}' , cas_ex6_8 = '{1}' , cas_ex6_6='{4}' ,cas_ex6_5= '{5}' WHERE cas_ex6_Id = {2}";
            string queryreq = @"Update Req_ex4 set Req_ex4_5 = '{0}', Req_ex4_3 = '{3}', Req_ex4_4 = '{1}', Req_ex4_2 = '{4}' , Req_ex4_1= '{5}' WHERE Req_ex4_Id = {2}";
            string querymod = @"Update Prm_ex2 set Prm_ex2_58 = '{0}', Prm_ex2_56 = '{3}', Prm_ex2_57 = '{1}', Prm_ex2_55 = '{4}', Prm_ex2_54= '{5}' WHERE Prm_ex2_Id = {2}";
            string queryproj= @"Update Prj_ex5 set Prj_ex5_11= '{0}', Prj_ex5_12= '{3}', Prj_ex5_10 = '{1}', Prj_ex5_9 = '{4}' ,Prj_ex5_8= '{5}' WHERE Prj_ex5_Id = {2}";
            string query = null;
            if (workitemType == WorkitemType.Bug) { query = querycas; }
            else if (workitemType == WorkitemType.Req) { query = queryreq; }
            else if (workitemType == WorkitemType.Module) { query = querymod; }
            else if(workitemType == WorkitemType.Project){ query = queryproj; }

            string Lastsyncin = null;
            string SyncInDirection = null;
            string syncstatus = null;

            if (String.IsNullOrEmpty(newworkitemId))
            {
                Lastsyncin = "Failed";
                SyncInDirection = "Failed";
                syncstatus = "Failure";

            }
            else {
                Wiql wiqlQuery = new Wiql

                {
                    Query = string.Format(@"SELECT [System.Id] ,[System.ChangedDate], [System.State] FROM workitems WHERE [System.TeamProject] = 'Test' and   [System.Id] = '{0}'", newworkitemId)
                };

                WorkItemQueryResult tasks = witClient.QueryByWiqlAsync(wiqlQuery).Result;
                IEnumerable<WorkItemReference> workItemReference = tasks.WorkItems.OrderBy(item => item.Id);
                List<WorkItem> tasksList = witClient.GetWorkItemsAsync(workItemReference.Select(itemID => itemID.Id)).Result;
                foreach (var task in tasksList)
                {


                    SyncInDirection = task.Fields["System.ChangedDate"].ToString();


                }
                DateTime Azdt = DateTime.ParseExact(SyncInDirection, "M/dd/yyyy h:mm:ss tt", null);
                DateTime Mycrdt = DateTime.ParseExact(Lastmodified, "M/dd/yyyy h:mm:ss tt", null);
                int result = DateTime.Compare(Azdt, Mycrdt);
                if (result > 0)
                {
                    syncstatus = "Syncin Direction forward(Azure is Latest)";
                }
                else if (result < 0) { syncstatus = "Syncin Direction backword(CRmnext is Latest)"; }
                else { syncstatus = "Both are in exact sync"; }
            }
           
            SqlCommand UpdateCommand = new SqlCommand(string.Format(query, SyncInDirection, syncstatus, myid, status,DateTime.Now.ToString(),id), cnn);
            UpdateCommand.ExecuteNonQuery();

        }
        public static List<WitModel> wittotalinfo(int Projectid, int OwnerId, string Relatedtoname, SqlConnection cnn)
        {
            int Relatedto;
            string projectworkitemid = null;
            string wid = null;
            List<WitModel> Witinfolist = new List<WitModel>();
            List<string> Workitemid = new List<string>();
            
             var newitemdocument = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
            cnn.Open();
            try
            {
                DBQueries.QueryProject(Witinfolist, Projectid, OwnerId, cnn);
                DBQueries.QueryFeature(Witinfolist, Relatedtoname, OwnerId, cnn);
                DBQueries.QueryRequirement(Witinfolist, Projectid, OwnerId, cnn, Relatedtoname);
                DBQueries.QueryBug(Witinfolist, Projectid, OwnerId, cnn, Relatedtoname);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Witinfolist;
        }

    }

}
