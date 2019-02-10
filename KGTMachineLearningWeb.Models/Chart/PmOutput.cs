using KGTMachineLearningWeb.Common.Attributes;

namespace KGTMachineLearningWeb.Models.Chart
{
    public class PmOutput
    {
        [Order]
        public double Buy { get; set; }
        [Order]
        public double None { get; set; }
        [Order]
        public double Sell { get; set; }
        [Order]
        public string StringRepresentation { get; set; }
    }
}
