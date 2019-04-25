using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataVisualization.Models
{
    public class ChartFileModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string ChartTitle { get; set; }
        [Required]
        public int ConfigId { get; set; }
    }
}