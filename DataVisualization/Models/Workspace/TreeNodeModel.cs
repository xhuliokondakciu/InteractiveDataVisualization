using Highsoft.Web.Mvc.Charts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataVisualization.Models.Workspace
{
    public class TreeNodeModel
    {
        public string Key { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public IList<TreeNodeModel> Children { get; set; }
        
        public bool Expanded { get; set; }
        
        public bool Folder { get; set; }
        
        public bool Lazy { get; set; }

        public bool IsRoot { get; set; }

        public bool IsShared { get; set; }

        public bool IsEveryones { get; set; }

        public bool IsSharedRoot { get; set; }

        public IEnumerable<string> AllowedActions { get; set; }

        public string Icon
        {
            get
            {
                if (IsSharedRoot && IsRoot && IsShared)
                {
                    return "share-icon";
                }
                else if (IsEveryones && IsRoot)
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