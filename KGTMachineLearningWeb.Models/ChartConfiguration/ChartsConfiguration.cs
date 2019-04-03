using KGTMachineLearningWeb.Common.Exceptions;
using KGTMachineLearningWeb.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace KGTMachineLearningWeb.Models.ChartConfiguration
{
    public class ChartsConfiguration : IEquatable<ChartsConfiguration>
    {
        public ChartsConfiguration()
        {
            RequiresProcess = true;
        }
        public ChartsConfiguration(string title, int? processorId, string configuration, string userId, bool isSystem)
        {
            Title = title;
            ProcessorId = processorId;
            ConfigurationXml = configuration;
            UserId = userId;
            IsSystem = isSystem;
            RequiresProcess = true;
        }
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [DisplayName("Processor")]
        public int? ProcessorId { get; set; }

        public virtual ProcessorConfiguration Processor { get; set; }

        [Required]
        [Column(TypeName = "xml")]
        [AllowHtml]
        [DisplayName("Configuration")]
        public string ConfigurationXml { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        [DisplayName("Is system?")]
        public bool IsSystem { get; set; }

        [DisplayName("Requires process")]
        public bool RequiresProcess { get; set; }

        public ChartsConfigSchema GetConfigurationSchema()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ChartsConfigSchema));
                using (StringReader reader = new StringReader(ConfigurationXml))
                {
                   return (ChartsConfigSchema)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void SchemaIsValid(string configXml)
        {
            var schemaSet = new XmlSchemaSet();
            using (var schema = XmlReader.Create(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ChartConfigSchema"])))
            {
                schemaSet.Add(null, schema);
                schemaSet.Compile();

                var partialSchemaObject = schemaSet.GlobalElements[new XmlQualifiedName("ChartsConfigSchema")];

                var config = XDocument.Parse(configXml);
                config.Root.Validate(partialSchemaObject, schemaSet, ValidationCallBack, true);
            }
        }

        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                throw new ChartSchemaValidationException(args.Message, args.Exception);
            }

        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartsConfiguration);
        }

        public bool Equals(ChartsConfiguration other)
        {
            return other != null &&
                   Id == other.Id &&
                   Title == other.Title &&
                   EqualityComparer<int?>.Default.Equals(ProcessorId, other.ProcessorId) &&
                   ConfigurationXml == other.ConfigurationXml &&
                   UserId == other.UserId &&
                   IsSystem == other.IsSystem &&
                   RequiresProcess == other.RequiresProcess;
        }

        public override int GetHashCode()
        {
            var hashCode = -23027694;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(ProcessorId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ConfigurationXml);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserId);
            hashCode = hashCode * -1521134295 + IsSystem.GetHashCode();
            hashCode = hashCode * -1521134295 + RequiresProcess.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ChartsConfiguration left, ChartsConfiguration right)
        {
            return EqualityComparer<ChartsConfiguration>.Default.Equals(left, right);
        }

        public static bool operator !=(ChartsConfiguration left, ChartsConfiguration right)
        {
            return !(left == right);
        }
    }



}
