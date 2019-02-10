using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KGTMachineLearningWeb.Common.Exceptions
{

    [Serializable]
    public class ChartSchemaValidationException : XmlException
    {
        public ChartSchemaValidationException() { }
        public ChartSchemaValidationException(string message) : base(message) { }
        public ChartSchemaValidationException(string message, Exception inner) : base(message, inner) { }
        protected ChartSchemaValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
