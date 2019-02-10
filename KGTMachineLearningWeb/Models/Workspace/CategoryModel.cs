using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KGTMachineLearningWeb.Models.Workspace
{
    public class CategoryModel
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public LinkedList<int> Hierarchy { get; set; }

        public bool IsShared { get; set; }

        public bool IsEveryones { get; set; }

        public bool IsRoot { get; set; }

        public IEnumerable<string> AllowedActions { get; set; }

        public string Icon
        {
            get
            {
                if (IsEveryones && IsRoot)
                {
                    return "everyone-icon";
                }
                else if (IsShared && IsRoot)
                {
                    return "user-icon";
                }
                else
                {
                    return "folder-icon";
                }
            }
        }
    }
}