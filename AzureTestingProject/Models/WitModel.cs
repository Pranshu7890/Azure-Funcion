using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTestingProject
{
    public class WitModel
    {
        public string WitId { get; set; }
        public string Witname { get; set; }
        public string WitLastmodified { get; set; }
        public string Workitemid { get; set; }
        public WorkitemType WorkitemType { get; set; }
        public string Description { get; set; }
        public bool iscreated {get; set;}
        public int state;


    }
}
