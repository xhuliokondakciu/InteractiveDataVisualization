using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KGTMachineLearningWeb.Models.Workspace.Workspace
{
    public class WorkspaceModel
    {
        public IEnumerable<TreeNodeModel> TreeNodes { get; set; }
        public CategoryContentModel CategoryContents { get; set; }
    }
}