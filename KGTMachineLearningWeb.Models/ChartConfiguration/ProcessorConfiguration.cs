using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Models.ChartConfiguration
{
    public class ProcessorConfiguration
    {
        public ProcessorConfiguration()
        {

        }

        public ProcessorConfiguration(string name, string path, string extraParameters)
        {
            Name = name;
            Path = path;
            ExtraParameters = extraParameters;
        }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Processor full path")]
        public string Path { get; set; }

        [DisplayName("Extra parameters")]
        public string ExtraParameters { get; set; }
        [JsonIgnore]
        public virtual ICollection<ChartsConfiguration> ChartsConfigurations { get; set; }
    }
}
