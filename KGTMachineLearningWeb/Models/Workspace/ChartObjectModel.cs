using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace KGTMachineLearningWeb.Models.Workspace
{
    public class 
        ChartObjectModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ChartType ChartType { get; set; }

        public int CategoryId { get; set; }
        public bool IsShared { get; set; }
        public bool IsEveryones { get; set; }

        public IEnumerable<string> AllowedActions { get; set; }
    }
}