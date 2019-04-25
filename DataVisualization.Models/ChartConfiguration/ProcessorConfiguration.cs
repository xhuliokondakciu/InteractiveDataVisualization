using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Models.ChartConfiguration
{
    public class ProcessorConfiguration : IEquatable<ProcessorConfiguration>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as ProcessorConfiguration);
        }

        public bool Equals(ProcessorConfiguration other)
        {
            return other != null &&
                   Id == other.Id &&
                   Name == other.Name &&
                   Path == other.Path &&
                   ExtraParameters == other.ExtraParameters &&
                   EqualityComparer<ICollection<ChartsConfiguration>>.Default.Equals(ChartsConfigurations, other.ChartsConfigurations);
        }

        public override int GetHashCode()
        {
            var hashCode = -1774390916;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ExtraParameters);
            hashCode = hashCode * -1521134295 + EqualityComparer<ICollection<ChartsConfiguration>>.Default.GetHashCode(ChartsConfigurations);
            return hashCode;
        }

        public static bool operator ==(ProcessorConfiguration left, ProcessorConfiguration right)
        {
            return EqualityComparer<ProcessorConfiguration>.Default.Equals(left, right);
        }

        public static bool operator !=(ProcessorConfiguration left, ProcessorConfiguration right)
        {
            return !(left == right);
        }
    }
}
