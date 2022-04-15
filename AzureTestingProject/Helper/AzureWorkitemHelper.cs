﻿using System;
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
    class AzureWorkitemHelper
    {

        public void CreateWorkitems(string Projectname, string Description)

        {
            int Projectid = 7956;
            int OwnerId = 2;
            int Relatedto;
            string Relatedtoname = "Azure crmnext integration";


            WitModel witinfo = new WitModel();
            string projectworkitemid = null;
            string wid = null;
            List <WitModel> Witinfolist= new List<WitModel>();
            List<string> Workitemid = new List<string>();
            
            string epicid = null;
            var Url = "https://dev.azure.com/acidaes/";
            var User = "Pranshu.singh@crmnext.com";
            var Pat = "st44wdgr3y4oijgla7gvzosfkeknb36jh2slrl33v7vry5mbsjka";

            string connetionString;
            SqlConnection cnn;
            connetionString = @"data source=192.168.0.82;initial catalog=MY_April2022;password=abc123;persist security info=True;user id=crmnext;packet size=4096;MultipleActiveResultSets=true;enlist=false";
            cnn = new SqlConnection(connetionString);

            VssConnection connection = null;
            connection = new VssConnection(new Uri(Url), new VssBasicCredential(User, Pat));
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var newitemdocument = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();



            cnn.Open();
            

            try
            {
                WorkItem newworkitem = null;

                //Feature
                Relatedto = Projectid;
                WorkItem updateworkitem = null;
                ////Requirement
                
                
                DBQueries.QueryProject(Witinfolist, Projectid, OwnerId, cnn);
                DBQueries.fetchworkidEpic(Relatedto, projectworkitemid, Witinfolist, Workitemid, cnn);
                Addworkitem(Description, "Bug", witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid, cnn);
                Witinfolist.Clear();
                Workitemid.Clear();
                
                DBQueries.QueryFeature(Witinfolist, Relatedtoname, OwnerId, cnn);
                wid = DBQueries.fetchworkidProject(Relatedto, projectworkitemid, Witinfolist, Workitemid, cnn).First();
                Addworkitem(Description, "Bug", witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid, cnn);
                Witinfolist.Clear();
                Workitemid.Clear();
                
                DBQueries.QueryRequirement(Witinfolist, Projectid, OwnerId, cnn, Relatedtoname);
                DBQueries.Fetchworkidreqaz(Witinfolist, wid, Workitemid, cnn);
                Addworkitem(Description, "Bug", witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist, Workitemid, cnn);
                Witinfolist.Clear();
                Workitemid.Clear();

                DBQueries.QueryBug(Witinfolist, Projectid, OwnerId, cnn, Relatedtoname);
                DBQueries.Fetchworkidcasaz(Witinfolist, wid, Workitemid, cnn);
                Addworkitem(Description, "Bug", witClient, newitemdocument, out newworkitem, out updateworkitem, Witinfolist,Workitemid, cnn);
                Witinfolist.Clear();
               Workitemid.Clear();


            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.Message);

            }
        }


        private void Addworkitem(string description, string type, WorkItemTrackingHttpClient witClient, Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument newitemdocument, out WorkItem newworkitem, out WorkItem updateworkitem, List<WitModel> witinfolist, List<string> Workitemlist, SqlConnection cnn)
        {
            string query = null;
            newworkitem = null;
            updateworkitem = null;
            var documentaddrelepic = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
            int count = Workitemlist.Count;
           
            var List = Enumerable.Range(0, count).Select(i => (new KeyValuePair<string, WitModel>(Workitemlist[i], witinfolist[i]))).ToList();


            foreach (var Workitem in List)
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
                        newworkitem = NewWorkitem(description, "Bug", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "Bug Workitem created");
                        SqlCommand UpdateCommandreq = new SqlCommand(string.Format(@"update cas_ex6 set cas_ex6_5={0} where cas_ex6_Id={1}", updateworkitem.Id, Workitem.Value.WitId.ToString()), cnn);
                        setDatafields(witClient, cnn, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId));
                        UpdateCommandreq.ExecuteNonQuery();
                        
                        Workitem.Value.Workitemid = updateworkitem.Id.ToString();
                    }

                    else if ((Workitem.Value.WorkitemType == WorkitemType.Req)&&(Workitem.Value.iscreated==false))
                    {
                        
                        newworkitem = NewWorkitem(description, "Requirement", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "Feature Workitem created");
                        SqlCommand UpdateCommandreq = new SqlCommand(string.Format(@"update Req_ex4 set Req_ex4_1={0} where Req_ex4_Id={1}", updateworkitem.Id, Workitem.Value.WitId.ToString()), cnn);
                        UpdateCommandreq.ExecuteNonQuery();
                        setDatafields(witClient, cnn, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId));
                    }
                    else if ((Workitem.Value.WorkitemType == WorkitemType.Module)&&(Workitem.Value.iscreated==false))
                    {

                       
                        newworkitem = NewWorkitem(description, "Feature", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        updateworkitem = witClient.UpdateWorkItemAsync(documentaddrelepic, Convert.ToInt32(newworkitem.Id)).Result;
                        Console.WriteLine(updateworkitem.Id.ToString() + " " + "feature Workitem created");
                        SqlCommand UpdateCommandfeature = new SqlCommand(string.Format(@"update Prm_ex2 set Prm_ex2_54={0} where Prm_ex2_Id={1}", updateworkitem.Id, Workitem.Value.WitId.ToString()), cnn);
                        UpdateCommandfeature.ExecuteNonQuery();
                        
                        setDatafields(witClient, cnn, "Success", Convert.ToString(updateworkitem.Id), Workitem.Value.WitLastmodified, Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId));
                    }



                    else if ((Workitem.Value.WorkitemType == WorkitemType.Project) && (Workitem.Value.iscreated==false))
                    {
                       
                        newworkitem = NewWorkitem(description, "Epic", witClient, newitemdocument, Workitem.Value.Witname.ToString());
                        SqlCommand UpdatewiCommand = new SqlCommand(string.Format(@"update Prj_ex5 set Prj_ex5_8={0} where Prj_ex5_Id={1}", newworkitem.Id, Workitem.Value.WitId.ToString()), cnn);
                        UpdatewiCommand.ExecuteNonQuery();
                        
                        Console.WriteLine("Successfully new workitem created");
                        Console.WriteLine(newworkitem.Id.ToString() + " " + "New Workitem created");
                        
                        setDatafields(witClient, cnn, "Success", Convert.ToString(newworkitem.Id), Workitem.Value.WitLastmodified,Workitem.Value.WorkitemType, Convert.ToInt32(Workitem.Value.WitId));
                    }
                    newitemdocument.Clear();

                    documentaddrelepic.Clear();

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }
                if (List.Count == 0) { Console.WriteLine("Workitem already exist"); }

            }
        }
        private static WorkItem NewWorkitem(string Description, string type, WorkItemTrackingHttpClient witClient, Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument newitemdocument, string projectname)
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
                Value = Description
            });
            var newworkitem = witClient.CreateWorkItemAsync(newitemdocument, "Test", type).Result;
            
            return newworkitem;
        }

   
        private static async void setDatafields(WorkItemTrackingHttpClient witClient,SqlConnection cnn,string status ,string newworkitemId,string Lastmodified, WorkitemType workitemType, int myid)
        {
            string querycas = @"Update cas_ex6 set cas_ex6_9 ='{0}', cas_ex6_7 = '{3}' , cas_ex6_8 = '{1}' , cas_ex6_6='{4}' WHERE cas_ex6_Id = {2}";
            string queryreq = @"Update Req_ex4 set Req_ex4_5 = '{0}', Req_ex4_3 = '{3}', Req_ex4_4 = '{1}', Req_ex4_2 = '{4}' WHERE Req_ex4_Id = {2}";
            string querymod = @"Update Prm_ex2 set Prm_ex2_58 = '{0}', Prm_ex2_56 = '{3}', Prm_ex2_57 = '{1}', Prm_ex2_55 = '{4}' WHERE Prm_ex2_Id = {2}";
            string queryproj= @"Update Prj_ex5 set Prj_ex5_11= '{0}', Prj_ex5_12= '{3}', Prj_ex5_10 = '{1}', Prj_ex5_9 = '{4}' WHERE Prj_ex5_Id = {2}";
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
           
            SqlCommand UpdateCommand = new SqlCommand(string.Format(query, SyncInDirection, syncstatus, myid, status,DateTime.Now.ToString()), cnn);
            UpdateCommand.ExecuteNonQuery();

        }


    }

}
