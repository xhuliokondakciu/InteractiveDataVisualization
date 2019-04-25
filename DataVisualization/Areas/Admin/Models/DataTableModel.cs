using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataVisualization.Models
{
    public class DataTableModel
    {
        [Required]
        public int Draw { get; set; }
        [Required]
        public DataTableColumn[] Columns { get; set; }
        [Required]
        public DataTableOrder[] Order { get; set; }
        [Required]
        public int Start { get; set; }
        [Required]
        public int Length { get; set; }
        public DataTableSearch Search { get; set; }
    }

    public class DataTableColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTableSearch Search { get; set; }
    }

    public class DataTableSearch
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }

    public class DataTableOrder
    {
        [Required]
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class DataTableResponse<TModel>
    {
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public IEnumerable<TModel> Data { get; set; }
    }
}