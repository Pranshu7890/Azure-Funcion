using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace AzureTestingProject
{
   public  class AzureWorkitemHelper
    {
        
        static VssConnection connection;
        static WorkItemTrackingHttpClient witClient;
     

        static AzureWorkitemHelper()
        {
       connection = new VssConnection(new Uri("https://dev.azure.com/acidaes/"), new VssBasicCredential("Pranshu.singh@crmnext.com", "vslh2nhtr4t4ndsvaxj6d53fbag4aslwpeko7dhc56ugavyszsca"));
       witClient = connection.GetClient<WorkItemTrackingHttpClient>();
       


        }
        
        public void CreateWorkitems(int Projectid, int OwnerId, string Relatedtoname)

        {
           
             int Relatedto;
            int projectworkitemid=0;
            string wid = null;
            List <WitModel> Witinfolist= new List<WitModel>();
            List<string> Workitemid = new List<string>();
            
            
            var newitemdocument = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
         
            try { 
            
                WorkItem newworkitem = null;
                Relatedto = Projectid;
                WorkItem updateworkitem = null;
                              
                DBQueries.QueryProject(Witinfolist, Projectid, OwnerId);
                wid=DBQueries.fetchworkidEpic(Relatedto, Witinfolist, Workitemid).FirstOrDefault();
               Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid);
                Witinfolist.Clear();
                Workitemid.Clear();

                DBQueries.QueryFeature(Witinfolist, Relatedtoname, OwnerId);

                DBQueries.fetchworkidProject(Relatedto, Witinfolist, Workitemid);
                Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid);
                Witinfolist.Clear();
                Workitemid.Clear();

                DBQueries.QueryRequirement(Witinfolist, Projectid, OwnerId, Relatedtoname);
                DBQueries.Fetchworkidreqaz(Witinfolist, wid, Workitemid);
                Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid);
                Witinfolist.Clear();
                Workitemid.Clear();

                DBQueries.QueryBug(Witinfolist, Projectid, OwnerId, Relatedtoname);
                DBQueries.Fetchworkidcasaz(Witinfolist, wid, Workitemid);
               Addworkitem( witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid);
                Witinfolist.Clear();
                Workitemid.Clear();
                

            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.Message);
                

            }
        }


        private void Addworkitem( WorkItemTrackingHttpClient witClient, Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument newitemdocument, out WorkItem newworkitem, out WorkItem updateworkitem, List<WitModel> witinfolist, List<string> Workitemlist)
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
                        newworkitem = NewWorkitem( "Bug", witClient, newitemdocument, Workitem.Value.Witname.ToString(),Workitem.Value.Description);
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "Bug Workitem created");
                        setDatafields(witClient, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), updateworkitem.Id);
                        Workitem.Value.Workitemid = updateworkitem.Id.ToString();
                        
                    }

                    else if ((Workitem.Value.WorkitemType == WorkitemType.Req)&&(Workitem.Value.iscreated==false))
                    {
                        newworkitem = NewWorkitem("Requirement", witClient, newitemdocument, Workitem.Value.Witname.ToString(), Workitem.Value.Description);
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "Feature Workitem created");
                        setDatafields(witClient, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), updateworkitem.Id);
                        
                    }
                    else if ((Workitem.Value.WorkitemType == WorkitemType.Module)&&(Workitem.Value.iscreated==false))
                    {
                        newworkitem = NewWorkitem( "Feature", witClient, newitemdocument, Workitem.Value.Witname.ToString(), Workitem.Value.Description);
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "Bug Workitem created");
                        setDatafields(witClient, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified, Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), updateworkitem.Id);
                       
                    }



                    else if ((Workitem.Value.WorkitemType == WorkitemType.Project) && (Workitem.Value.iscreated==false))
                    {
                        newworkitem = NewWorkitem( "Epic", witClient, newitemdocument, Workitem.Value.Witname.ToString(), Workitem.Value.Description);
                        Console.WriteLine(newworkitem.Id.ToString() + " " + "Epic Workitem created");
                        setDatafields(witClient, "Success", Convert.ToString(newworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId), newworkitem.Id);
                        
                    }
                    newitemdocument.Clear();

                    documentaddrelepic.Clear();

                }
                catch(Exception ex)
                {
                    Console.WriteLine( ex.Message);   
                }
                if (Heirerchylist.Count == 0) { Console.WriteLine("Workitem already exist"); }

            }
            Console.WriteLine( "Creation function executed");

        }
        private static WorkItem NewWorkitem(string type, WorkItemTrackingHttpClient witClient, Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument newitemdocument, string projectname, string description)
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
                Value = description
            }) ;

            
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
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemQueryResult tasks = witClient.QueryByWiqlAsync(wiqlQuery).Result;
            IEnumerable<WorkItemReference> workItemReference = tasks.WorkItems.OrderBy(item => item.Id);
            List<WorkItem> tasksList = witClient.GetWorkItemsAsync(workItemReference.Select(itemID => itemID.Id)).Result;
            foreach (var task in tasksList)
            {


                 board= task.Fields["System.BoardColumn"].ToString();


            }
            return board;

        }
        public static bool  getQAfailure(int wid)
        {
            string board = null;
            Wiql wiqlQuery = new Wiql

            {
                Query = string.Format(@"SELECT [System.Id] ,[System.BoardColumn] FROM workitems WHERE [System.State] = 'Active' and  EVER [System.BoardColumn]= 'QA' and  EVER [System.BoardColumnDone]= False  and [System.BoardColumn] = 'Dev' and [System.ChangedDate]='{1}' and [System.BoardColumnDone]= False and   [System.Id] = '{0}'", wid,DateTime.Now.ToString("M/d/yyyy"))
            };
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemQueryResult tasks = witClient.QueryByWiqlAsync(wiqlQuery).Result;
            witClient.GetRecentActivityDataAsync();
            IEnumerable<WorkItemReference> workItemReference = tasks.WorkItems.OrderBy(item => item.Id);
            if (workItemReference.Count()!=0)
            {
                return true;
            }
            return false;

        }


        public static async void setDatafields(WorkItemTrackingHttpClient witClient,string status ,string newworkitemId,string Lastmodified, WorkitemType workitemType, int myid, int? id)
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
                DateTime Azdt = DateTime.ParseExact(SyncInDirection, "M/d/yyyy h:mm:ss tt", null);
                DateTime Mycrdt = DateTime.ParseExact(Lastmodified, "M/d/yyyy h:mm:ss tt", null);
                int result = DateTime.Compare(Azdt, Mycrdt);
                if (result > 0)
                {
                    syncstatus = "Syncin Direction forward(Azure is Latest)";
                }
                else if (result < 0) { syncstatus = "Syncin Direction backword(CRmnext is Latest)"; }
                else { syncstatus = "Both are in exact sync"; }
            }
            string finalquery  = string.Format(query, SyncInDirection, syncstatus, myid, status, DateTime.Now.ToString(), id);
             Console.WriteLine(QueryExecuter.updateQuery(finalquery));
        }
        public static List<WitModel> wittotalinfo(int Projectid, int OwnerId, string Relatedtoname)
        {
            int Relatedto;
            string projectworkitemid = null;
            string wid = null;
            List<WitModel> Witinfolist = new List<WitModel>();
            List<string> Workitemid = new List<string>();
            
             var newitemdocument = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
            
            try
            {
                DBQueries.QueryProject(Witinfolist, Projectid, OwnerId);
                DBQueries.QueryFeature(Witinfolist, Relatedtoname, OwnerId);
                DBQueries.QueryRequirement(Witinfolist, Projectid, OwnerId, Relatedtoname);
                DBQueries.QueryBug(Witinfolist, Projectid, OwnerId, Relatedtoname);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Witinfolist;
        }





    }

}
