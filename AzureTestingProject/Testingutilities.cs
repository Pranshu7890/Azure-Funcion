using System;
using System.Data.SqlClient;
using CRMnextServiceAPI;

using System.Net.Http;


namespace AzureTestingProject
{
    class Testingutilities
    {
        static void Main(string[] args)
        {
            //HttpRequestMessage req=null;
            //ILogger log=null;
            ////Dbcondatafetch();
            AzureWorkitemHelper testingAzureApi = new AzureWorkitemHelper();
            
            testingAzureApi.CreateWorkitems("ParentProject", "this is new Workitem created");
            

        }
    }
        
}
