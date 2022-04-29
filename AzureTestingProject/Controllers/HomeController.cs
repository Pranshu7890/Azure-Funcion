using AzureTestingProject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public  string Index(int id,int ownid,string project )
        {
          AzureWorkitemHelper azureWorkitemHelper = new AzureWorkitemHelper();
           azureWorkitemHelper.CreateWorkitems(id, ownid, project);


            return "success";
        }
        public string patex()
        {
            return "pat";
        }

            public IActionResult Privacy()
        {
            return View();
        }

       
        
    }
}