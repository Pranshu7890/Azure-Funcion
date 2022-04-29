using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AzureTestingProject
{
    static class DBQueries
    {
        public static void QueryProject(List<WitModel> witModels, int projectid, int ownerId)
        {
            string Query = string.Format(@"Select project.ProjectName,project.Detail ,project.ProjectID,project.LastModifiedOn,Prj_ex5.Prj_ex5_8 from project INNER JOIN Prj_ex5 on Prj_ex5.Prj_ex5_ID=project.ProjectID where project.ProjectID={0} and project.OwnerId={1}", projectid, ownerId);
            using (DataTableReader reader = QueryExecuter.ReadQuery(Query))
            {
                while (reader.Read())
                {
                   
                        WitModel witModel = new WitModel();
                        witModel.Witname = String.Format("{0}", reader["ProjectName"]);
                        witModel.WitId = string.Format("{0}", reader["ProjectID"]);
                        witModel.WitLastmodified = String.Format("{0}", reader["LastModifiedOn"]);
                        witModel.WorkitemType = WorkitemType.Project;
                    witModel.Description = string.Format("{0}", reader["Detail"]);
                    if (String.IsNullOrEmpty(String.Format("{0}", reader["Prj_ex5_8"])))
                    { witModel.iscreated = false; }
                    else { witModel.iscreated = true;
                        witModel.Workitemid = String.Format("{0}", reader["Prj_ex5_8"]);
                    }
                    
                    witModels.Add(witModel);

                    

                }
            }
        }
        public static List<string> fetchworkidEpic(int relatedto, List<WitModel> witModels, List<String> Wid)
        {
            foreach (var witmodel in witModels)
            {
                if (witmodel.WorkitemType == WorkitemType.Project)
                {
                    String Commnad = string.Format(@"select Prj_ex5_8 from Prj_ex5 where Prj_ex5_Id={0}", relatedto);
                    using (DataTableReader reader = QueryExecuter.ReadQuery(Commnad))
                    {
                        while (reader.Read())
                        {
                            Wid.Add(String.Format("{0}", reader["Prj_ex5_8"]));
                        }
                    }
                }

            }
            return Wid;
        }


        public static void QueryBug(List<WitModel> witModels, int projectid, int ownerId, string relatedtoname)
        {
            String Command = string.Format(@"select cases.Subject,cases.CaseId,cases.Details,cases.LastModifiedOn,cas_ex6.cas_ex6_5, cas_ex2.Cas_ex2_2 from cases INNER JOIN cas_ex6 On cases.CaseId=cas_ex6.cas_ex6_Id INNER JOIN cas_ex2 on cas_ex2.cas_ex2_Id=cas_ex6.cas_ex6_Id   where cas_ex2_2={2}", relatedtoname, ownerId, projectid);
            using (DataTableReader reader = QueryExecuter.ReadQuery(Command))
            {
                while (reader.Read())
                {
                    
                        WitModel witModel = new WitModel();
                    witModel.Witname = String.Format("{0}", reader["Subject"]);
                    witModel.WitId = string.Format("{0}", reader["CaseId"]);
                    witModel.WitLastmodified = String.Format("{0}", reader["LastModifiedOn"]);
                    witModel.Description = string.Format("{0}", reader["Details"]);
                    witModel.WorkitemType = WorkitemType.Bug;
                        witModels.Add(witModel);

                    if (String.IsNullOrEmpty(String.Format("{0}", reader["cas_ex6_5"])))
                    { witModel.iscreated = false; }
                    else { witModel.iscreated = true;
                        witModel.Workitemid = String.Format("{0}", reader["cas_ex6_5"]);
                    }
                    
                    

                }
            }

        }
        public static List<string> fetchworkidProject(int relatedto, List<WitModel> witModels, List<String> Wid)
        {
            foreach (var witmodel in witModels)
            {
                if (witmodel.WorkitemType == WorkitemType.Module)
                {
                    String command = string.Format(@"select Prj_ex5_8 from Prj_ex5 where Prj_ex5_Id={0}", relatedto);
                    using (DataTableReader reader = QueryExecuter.ReadQuery(command))
                    {
                        while (reader.Read())
                        {
                            Wid.Add(String.Format("{0}", reader["Prj_ex5_8"]));
                        }
                    }
                }

            }
            return Wid;
        }
        public static void QueryFeature(List<WitModel> witinfolists, String RelatedTo, int OwnerId)
        {


            string Command = string.Format(@"select ProjectModule.Subject,ProjectModule.Detail, ProjectModule.CustomObjectId,ProjectModule.LastModifiedOn ,Prm_ex2.Prm_ex2_54 from ProjectModule INNER JOIN Prm_ex2 On ProjectModule.CustomObjectId=Prm_ex2.Prm_ex2_Id  where ProjectModule.RelatedToName='{0}' and ProjectModule.OwnerId={1}", RelatedTo, OwnerId);
            using (DataTableReader reader = QueryExecuter.ReadQuery(Command))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        
                            WitModel witModel = new WitModel();
                        witModel.Witname = string.Format("{0}", reader["Subject"]);
                        witModel.WitId = string.Format("{0}", reader["CustomObjectId"]);
                        witModel.WitLastmodified = string.Format("{0}", reader["LastModifiedOn"]);
                        witModel.WorkitemType = WorkitemType.Module;
                        witModel.Description= string.Format("{0}", reader["Detail"]);

                        witinfolists.Add(witModel);
                        if (string.IsNullOrEmpty(string.Format("{0}", reader["Prm_ex2_54"])))
                        { witModel.iscreated = false; }
                        else { witModel.iscreated = true;
                            witModel.Workitemid = string.Format("{0}", reader["Prm_ex2_54"]);
                        }
                        

                        
                    }
                }
            }

        }

        public static void QueryRequirement(List<WitModel> witModels, int RelatedTo, int OwnerId, string relatedtoname)
        {
string Command=string.Format(@"select IssueRequirementMaster.Subject,IssueRequirementMaster.Details ,IssueRequirementMaster.ItemId , IssueRequirementMaster.LastModifiedOn , Req_ex4.Req_ex4_1, Req_ex1.Req_ex1_84 from IssueRequirementMaster INNER JOIN Req_ex4 ON IssueRequirementMaster.ItemId=Req_ex4.Req_ex4_Id INNER JOIN Req_ex1 ON Req_ex4.Req_ex4_Id=Req_ex1.Req_ex1_Id where Req_ex1.Req_ex1_84={2} ", relatedtoname, OwnerId, RelatedTo);
            using (DataTableReader reader = QueryExecuter.ReadQuery(Command))
            {
                while (reader.Read())
                {

                        WitModel witModel = new WitModel();
                    witModel.Witname = String.Format("{0}", reader["Subject"]);
                    witModel.WitId = string.Format("{0}", reader["ItemId"]);
                    witModel.WitLastmodified = String.Format("{0}", reader["LastModifiedOn"]);
                    witModel.WorkitemType = WorkitemType.Req;
                    witModel.Description = string.Format("{0}", reader["Details"]);



                    if (String.IsNullOrEmpty(String.Format("{0}", reader["Req_ex4_1"])))
                    {
                        witModel.iscreated = false;
                    }
                    else
                    {
                        witModel.iscreated = true;
                        witModel.Workitemid = String.Format("{0}", reader["Req_ex4_1"]);
                    }
                    witModels.Add(witModel);



                }
            }
        }



        public static List<String> Fetchworkidcasaz(List<WitModel> witModels, string v, List<string> workitemscas)
        {
            List<string> module = new List<string>();

            foreach (var cases in witModels)
            {
                if (cases.WorkitemType == WorkitemType.Bug)
                {
                    String Command = String.Format(@"select Cas_ex4_89 from Cas_ex4 where Cas_ex4_Id={0}", cases.WitId);
                    using (DataTableReader reader = QueryExecuter.ReadQuery(Command))
                    {
                        while (reader.Read())
                        {
                            module.Add(String.Format("{0}", reader["Cas_ex4_89"]));
                        }
                    }
                }
            }

            for (int i = 0; i < module.Count; i++)
            {
                if (String.IsNullOrEmpty(module[i]))
                {
                    module[i] = "NULL";
                }

                string Command = String.Format(@"select Prm_ex2_54 from Prm_ex2 where Prm_ex2_Id= {0}", String.Format("{0}", module[i]));
                using (DataTableReader reader = QueryExecuter.ReadQuery(Command))
                {
                    while (reader.Read())
                    {
                        workitemscas.Add(String.Format("{0}", reader["Prm_ex2_54"]));
                    }
                    if (module[i] == "NULL") { workitemscas.Insert(i, v); }
                }
            }

            return workitemscas;
        }
        public static List<string> Fetchworkidreqaz(List<WitModel> witModels, string v, List<string> workitemsreq)
        {
            List<string> reqid = new List<string>();




            foreach (var requirement in witModels)
            {
                if (requirement.WorkitemType == WorkitemType.Req) 
                {
                    string commandouter = String.Format(@"select Req_ex2_82  from Req_ex2 where Req_ex2_Id= {0} ", requirement.WitId);
                    using (DataTableReader reader = QueryExecuter.ReadQuery(commandouter))
                    {
                        while (reader.Read())
                        { 
                            reqid.Add(String.Format("{0}", reader["Req_ex2_82"]));

                        }
                    }
                }
            }


            for (int i = 0; i < reqid.Count; i++)
            {
                var data = Convert.ToString(reqid[i]);

                if (String.IsNullOrEmpty(data))
                {
                    reqid[i] = "NULL";
                }

                string command = String.Format(@"select Prm_ex2_54 from Prm_ex2 where Prm_ex2_Id= {0}", String.Format("{0}", reqid[i]));
                using (DataTableReader reader = QueryExecuter.ReadQuery(command))
                {
                    while (reader.Read())
                    {
                        workitemsreq.Add(String.Format("{0}", reader["Prm_ex2_54"]));
                    }
                    if (reqid[i] == "NULL") { workitemsreq.Insert(i, v); }
                }
            }
            return workitemsreq;
        }
    }
    


}
