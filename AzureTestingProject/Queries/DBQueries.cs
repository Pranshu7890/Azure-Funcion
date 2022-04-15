using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AzureTestingProject
{
    static class DBQueries
    {
        public static void QueryProject(List<WitModel> witModels, int projectid, int ownerId, SqlConnection cnn)
        {
            SqlCommand command = new SqlCommand(string.Format(@"Select project.ProjectName, project.ProjectID,project.LastModifiedOn,Prj_ex5.Prj_ex5_8 from project INNER JOIN Prj_ex5 on Prj_ex5.Prj_ex5_ID=project.ProjectID where project.ProjectID={0} and project.OwnerId={1}", projectid, ownerId), cnn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                   
                        WitModel witModel = new WitModel();
                        witModel.Witname = String.Format("{0}", reader["ProjectName"]);
                        witModel.WitId = string.Format("{0}", reader["ProjectID"]);
                        witModel.WitLastmodified = String.Format("{0}", reader["LastModifiedOn"]);
                        witModel.WorkitemType = WorkitemType.Project;
                    if (String.IsNullOrEmpty(String.Format("{0}", reader["Prj_ex5_8"])))
                    { witModel.iscreated = false; }
                    else { witModel.iscreated = true; }
                    
                    witModels.Add(witModel);

                    

                }
            }
        }
        public static List<string> fetchworkidEpic(int relatedto, string projectworkitemid, List<WitModel> witModels, List<String> Wid, SqlConnection cnn)
        {
            foreach (var witmodel in witModels)
            {
                if (witmodel.WorkitemType == WorkitemType.Project)
                {
                    SqlCommand command = new SqlCommand(string.Format(@"select Prj_ex5_8 from Prj_ex5 where Prj_ex5_Id={0}", relatedto), cnn);
                    using (SqlDataReader reader = command.ExecuteReader())
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


        public static void QueryBug(List<WitModel> witModels, int projectid, int ownerId, SqlConnection cnn, string relatedtoname)
        {
            SqlCommand command = new SqlCommand(string.Format(@"select cases.Subject,cases.CaseId,cases.LastModifiedOn,cas_ex6.cas_ex6_5  from cases INNER JOIN cas_ex6 On cases.CaseId=cas_ex6_Id where cases.RelatedToName='{0}' and cases.OwnerID={1}", relatedtoname, ownerId, projectid), cnn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    
                        WitModel witModel = new WitModel();
                    witModel.Witname = String.Format("{0}", reader["Subject"]);
                    witModel.WitId = string.Format("{0}", reader["CaseId"]);
                    witModel.WitLastmodified = String.Format("{0}", reader["LastModifiedOn"]);
                    witModel.WorkitemType = WorkitemType.Bug;
                        witModels.Add(witModel);

                    if (String.IsNullOrEmpty(String.Format("{0}", reader["cas_ex6_5"])))
                    { witModel.iscreated = false; }
                    else { witModel.iscreated = true; }
                    
                    

                }
            }

        }
        public static List<string> fetchworkidProject(int relatedto, string projectworkitemid, List<WitModel> witModels, List<String> Wid, SqlConnection cnn)
        {
            foreach (var witmodel in witModels)
            {
                if (witmodel.WorkitemType == WorkitemType.Module)
                {
                    SqlCommand command = new SqlCommand(string.Format(@"select Prj_ex5_8 from Prj_ex5 where Prj_ex5_Id={0}", relatedto), cnn);
                    using (SqlDataReader reader = command.ExecuteReader())
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
        public static void QueryFeature(List<WitModel> witinfolists, String RelatedTo, int OwnerId, SqlConnection cnn)
        {


            SqlCommand command = new SqlCommand(string.Format(@"select ProjectModule.Subject, ProjectModule.CustomObjectId,ProjectModule.LastModifiedOn ,Prm_ex2.Prm_ex2_54 from ProjectModule INNER JOIN Prm_ex2 On ProjectModule.CustomObjectId=Prm_ex2.Prm_ex2_Id  where ProjectModule.RelatedToName='{0}' and ProjectModule.OwnerId={1}", RelatedTo, OwnerId), cnn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        
                            WitModel witModel = new WitModel();
                        witModel.Witname = string.Format("{0}", reader["Subject"]);
                        witModel.WitId = string.Format("{0}", reader["CustomObjectId"]);
                        witModel.WitLastmodified = String.Format("{0}", reader["LastModifiedOn"]);
                        witModel.WorkitemType = WorkitemType.Module;
                        
                            
                            witinfolists.Add(witModel);
                        if (String.IsNullOrEmpty(String.Format("{0}", reader["Prm_ex2_54"])))
                        { witModel.iscreated = false; }
                        else { witModel.iscreated = true; }
                        

                        
                    }
                }
            }

        }

        public static void QueryRequirement(List<WitModel> witModels, int RelatedTo, int OwnerId, SqlConnection cnn, string relatedtoname)
        {

            SqlCommand command = new SqlCommand(string.Format(@"select IssueRequirementMaster.Subject, IssueRequirementMaster.ItemId , IssueRequirementMaster.LastModifiedOn , Req_ex4.Req_ex4_1 from IssueRequirementMaster INNER JOIN Req_ex4 ON IssueRequirementMaster.ItemId=Req_ex4.Req_ex4_Id where IssueRequirementMaster.RelatedToName='{0}' and IssueRequirementMaster.OwnerID={1}", relatedtoname, OwnerId, RelatedTo), cnn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {

                        WitModel witModel = new WitModel();
                    witModel.Witname = String.Format("{0}", reader["Subject"]);
                    witModel.WitId = string.Format("{0}", reader["ItemId"]);
                    witModel.WitLastmodified = String.Format("{0}", reader["LastModifiedOn"]);
                    witModel.WorkitemType = WorkitemType.Req;
                   
                        
                        

                    if (String.IsNullOrEmpty(String.Format("{0}", reader["Req_ex4_1"])))
                    {
                        witModel.iscreated = false;
                    }
                    else
                    {
                        witModel.iscreated = true;
                    }
                    witModels.Add(witModel);



                }
            }
        }



        public static List<String> Fetchworkidcasaz(List<WitModel> witModels, string v, List<string> workitemscas, SqlConnection cnn)
        {
            List<string> module = new List<string>();

            foreach (var cases in witModels)
            {
                if (cases.WorkitemType == WorkitemType.Bug)
                {
                  SqlCommand commandouter = new SqlCommand(String.Format(@"select Cas_ex4_89 from Cas_ex4 where Cas_ex4_Id={0}", cases.WitId), cnn);
                    using (SqlDataReader reader = commandouter.ExecuteReader())
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

                SqlCommand commandinner = new SqlCommand(String.Format(@"select Prm_ex2_54 from Prm_ex2 where Prm_ex2_Id= {0}", String.Format("{0}", module[i])), cnn);
                using (SqlDataReader reader = commandinner.ExecuteReader())
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
        public static List<string> Fetchworkidreqaz(List<WitModel> witModels, string v, List<string> workitemsreq, SqlConnection cnn)
        {
            List<string> reqid = new List<string>();




            foreach (var requirement in witModels)
            {
                if (requirement.WorkitemType == WorkitemType.Req) 
                {
                    SqlCommand commandouter = new SqlCommand(String.Format(@"select Req_ex2_82  from Req_ex2 where Req_ex2_Id= {0} ", requirement.WitId), cnn);
                    using (SqlDataReader reader = commandouter.ExecuteReader())
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

                SqlCommand commandinner = new SqlCommand(String.Format(@"select Prm_ex2_54 from Prm_ex2 where Prm_ex2_Id= {0}", String.Format("{0}", reqid[i])), cnn);
                using (SqlDataReader reader = commandinner.ExecuteReader())
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
    

    //    public static void populateOffsetDate(string BugOffset,String ReqOffset,SqlConnection cnn)
    //    {
    //        SqlCommand Updateoffset = new SqlCommand(String.Format(@"update  Prj_ex5 set Prj_ex5_8={0},Prj_ex5_9={1} where Prj_ex5_Id={2}",BugOffset,ReqOffset),cnn);
    //        Updateoffset.ExecuteNonQuery();
    //    }
    //}


}
