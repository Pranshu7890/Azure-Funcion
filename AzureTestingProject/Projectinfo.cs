using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AzureTestingProject
{
    public class Projectinfo
    {
       
        public static VssConnection connection = new VssConnection(new Uri("https://dev.azure.com/acidaes/"), new VssBasicCredential("Pranshu.singh@crmnext.com", "st44wdgr3y4oijgla7gvzosfkeknb36jh2slrl33v7vry5mbsjka"));
       
        

    }
}
