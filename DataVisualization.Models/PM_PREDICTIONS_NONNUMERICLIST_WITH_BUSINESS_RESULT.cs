using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace DataVisualization.Models
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTS
    {

        private List<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> predictionField;

        [System.Xml.Serialization.XmlElementAttribute("Prediction")]
        public List<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> Prediction
        {
            get
            {
                return this.predictionField;
            }
            set
            {
                this.predictionField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction
    {

        private DateTimeOffset dateTimeUtcField;

        private double bidField;

        private double askField;

        private PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPredictionNode[] outputNodeValuesField;

        private string modelPredictionField;

        private string desiredPredictionsBinaryField;

        private DesiredPredictionsBinary desiredPredictionsBinaryFieldStruct;

        private double predictedBusinessYieldPpmField;

        private PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPredictionLabel[] businessResultPpmPerLabelField;

        [XmlElementAttribute(ElementName = "DateTimeUtc")]
        [JsonPropertyAttribute(PropertyName = "DateTimeUtc")]
        public string DateTimeUtcString
        {
            get
            {
                return this.dateTimeUtcField.ToString("yyyy-mm-dd H:mm:ss.FFFFFFF");
            }
            set
            {
                this.dateTimeUtcField = DateTimeOffset.Parse(value);
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public DateTimeOffset DateTimeUtcDate
        {
            get
            {
                return this.dateTimeUtcField;
            }
            set
            {
                this.dateTimeUtcField = value;
            }
        }

        public double Bid
        {
            get
            {
                return this.bidField;
            }
            set
            {
                this.bidField = value;
            }
        }

        public double Ask
        {
            get
            {
                return this.askField;
            }
            set
            {
                this.askField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("Node", IsNullable = false)]
        public PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPredictionNode[] OutputNodeValues
        {
            get
            {
                return this.outputNodeValuesField;
            }
            set
            {
                this.outputNodeValuesField = value;
            }
        }

        public string ModelPrediction
        {
            get
            {
                return this.modelPredictionField;
            }
            set
            {
                this.modelPredictionField = value;
            }
        }

        public string DesiredPredictionsBinary
        {
            get
            {
                return this.desiredPredictionsBinaryField;
            }
            set
            {
                var desiredPredictions = new DesiredPredictionsBinary();
                foreach (var desiredPredictionString in value.Split(','))
                {
                    var dPrediction = desiredPredictionString.Split('=');
                    switch (dPrediction[0].ToLowerInvariant())
                    {
                        case "buy":
                            desiredPredictions.Buy = Boolean.Parse(dPrediction[1]);
                            break;
                        case "sell":
                            desiredPredictions.Sell = Boolean.Parse(dPrediction[1]);
                            break;
                        case "none":
                            desiredPredictions.None = Boolean.Parse(dPrediction[1]);
                            break;
                    }
                }
                this.desiredPredictionsBinaryFieldStruct = desiredPredictions;
                this.desiredPredictionsBinaryField = value;
            }
        }

        public DesiredPredictionsBinary DesiredPredictionsBinaryStruct
        {
            get
            {
                return this.desiredPredictionsBinaryFieldStruct;
            }
        }


        public double PredictedBusinessYieldPpm
        {
            get
            {
                return this.predictedBusinessYieldPpmField;
            }
            set
            {
                this.predictedBusinessYieldPpmField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("Label", IsNullable = false)]
        public PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPredictionLabel[] BusinessResultPpmPerLabel
        {
            get
            {
                return this.businessResultPpmPerLabelField;
            }
            set
            {
                this.businessResultPpmPerLabelField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPredictionNode
    {

        private byte ordinalIdField;

        private double outputValueField;

        private string meaningField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte OrdinalId
        {
            get
            {
                return this.ordinalIdField;
            }
            set
            {
                this.ordinalIdField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double OutputValue
        {
            get
            {
                return this.outputValueField;
            }
            set
            {
                this.outputValueField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Meaning
        {
            get
            {
                return this.meaningField;
            }
            set
            {
                this.meaningField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPredictionLabel
    {

        private string nameField;

        private double businessResultPpmField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double BusinessResultPpm
        {
            get
            {
                return this.businessResultPpmField;
            }
            set
            {
                this.businessResultPpmField = value;
            }
        }
    }

    public partial struct DesiredPredictionsBinary
    {
        public bool Buy { get; set; }
        public bool Sell { get; set; }
        public bool None { get; set; }
    }
}
