using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Common.Enum
{
    public static class Enumerators
    {
        public enum CategoryViewType
        {
            List = 0,
            Grid
        }

        public enum Permissions
        {
            View = 0,
            Copy,
            Delete,
            Edit,
            Create
        }

        public enum UserRoles
        {
            Admin = 1,
            User
        }

        public enum DataFileType
        {
            PmOutput = 1,
            PmOutputsStrRepresentation,
            RbStreamWithDesiredPredictions,
            RbStreamWithDesiredPredictionBSNLabel
        }
    }
}
