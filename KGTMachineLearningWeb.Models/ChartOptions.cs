using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KGTMachineLearningWeb.Models
{
    public class ChartOption<xDataType,yDataType> : IEquatable<ChartOption<xDataType, yDataType>>
    {
        [JsonProperty(propertyName:"title")]
        public string Title { get; set; }

        [JsonProperty(propertyName: "series")]
        public IEnumerable<Series<xDataType,yDataType>> Series { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartOption<xDataType, yDataType>);
        }

        public bool Equals(ChartOption<xDataType, yDataType> other)
        {
            return other != null &&
                   Title == other.Title &&
                   EqualityComparer<IEnumerable<Series<xDataType, yDataType>>>.Default.Equals(Series, other.Series);
        }

        public override int GetHashCode()
        {
            var hashCode = 594275041;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<Series<xDataType, yDataType>>>.Default.GetHashCode(Series);
            return hashCode;
        }

        public static bool operator ==(ChartOption<xDataType, yDataType> option1, ChartOption<xDataType, yDataType> option2)
        {
            return EqualityComparer<ChartOption<xDataType, yDataType>>.Default.Equals(option1, option2);
        }

        public static bool operator !=(ChartOption<xDataType, yDataType> option1, ChartOption<xDataType, yDataType> option2)
        {
            return !(option1 == option2);
        }
    }

    public class Series<xDataType,yDataType> : IEquatable<Series<xDataType, yDataType>>
    {
        [JsonProperty(propertyName:"name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "data")]
        public IEnumerable<HighChartPoint<xDataType, yDataType>> Data { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Series<xDataType, yDataType>);
        }

        public bool Equals(Series<xDataType, yDataType> other)
        {
            return other != null &&
                   Name == other.Name &&
                   EqualityComparer<IEnumerable<HighChartPoint<xDataType, yDataType>>>.Default.Equals(Data, other.Data);
        }

        public override int GetHashCode()
        {
            var hashCode = 1601430459;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<HighChartPoint<xDataType, yDataType>>>.Default.GetHashCode(Data);
            return hashCode;
        }

        public static bool operator ==(Series<xDataType, yDataType> series1, Series<xDataType, yDataType> series2)
        {
            return EqualityComparer<Series<xDataType, yDataType>>.Default.Equals(series1, series2);
        }

        public static bool operator !=(Series<xDataType, yDataType> series1, Series<xDataType, yDataType> series2)
        {
            return !(series1 == series2);
        }
    }

    public class HighChartPoint<xDataType,yDataType> : IEquatable<HighChartPoint<xDataType, yDataType>>
    {
        [JsonProperty(propertyName : "x")]
        public xDataType X { get; set; }
        [JsonProperty(propertyName : "y")]
        public yDataType Y { get; set; }

        [JsonProperty(propertyName : "name")]
        public string Name { get; set; }

        [JsonProperty(propertyName : "color")]
        public string Color { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as HighChartPoint<xDataType, yDataType>);
        }

        public bool Equals(HighChartPoint<xDataType, yDataType> other)
        {
            return other != null &&
                   EqualityComparer<xDataType>.Default.Equals(X, other.X) &&
                   EqualityComparer<yDataType>.Default.Equals(Y, other.Y) &&
                   Name == other.Name &&
                   Color == other.Color;
        }

        public override int GetHashCode()
        {
            var hashCode = -295210639;
            hashCode = hashCode * -1521134295 + EqualityComparer<xDataType>.Default.GetHashCode(X);
            hashCode = hashCode * -1521134295 + EqualityComparer<yDataType>.Default.GetHashCode(Y);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Color);
            return hashCode;
        }

        public static bool operator ==(HighChartPoint<xDataType, yDataType> point1, HighChartPoint<xDataType, yDataType> point2)
        {
            return EqualityComparer<HighChartPoint<xDataType, yDataType>>.Default.Equals(point1, point2);
        }

        public static bool operator !=(HighChartPoint<xDataType, yDataType> point1, HighChartPoint<xDataType, yDataType> point2)
        {
            return !(point1 == point2);
        }
    }
}