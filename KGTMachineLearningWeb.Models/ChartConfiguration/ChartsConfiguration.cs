using KGTMachineLearningWeb.Common.Exceptions;
using KGTMachineLearningWeb.Models.Identity;
using Newtonsoft.Json;
using System;
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
    public class ChartsConfiguration
    {
        public ChartsConfiguration() { }
        public ChartsConfiguration(string title, int processorId, string configuration, string userId, bool isSystem)
        {
            Title = title;
            ProcessorId = processorId;
            ConfigurationXml = configuration;
            UserId = userId;
            IsSystem = isSystem;
        }
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DisplayName("Processor")]
        public int ProcessorId { get; set; }

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
            // define your schema set by importing your schema from an xsd file
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
    }



}
