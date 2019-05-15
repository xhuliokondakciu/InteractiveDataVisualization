using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Models.Chart
{
    public class ChartData : IEquatable<ChartData>
    {
        public ChartData(string chartName, IEnumerable<ChartSeries> series)
        {
            ChartName = chartName;
            Series = series;
        }
        public string ChartName { get; set; }
        public IEnumerable<ChartSeries> Series { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartData);
        }

        public bool Equals(ChartData other)
        {
            return other != null &&
                   ChartName == other.ChartName &&
                   EqualityComparer<IEnumerable<ChartSeries>>.Default.Equals(Series, other.Series);
        }

        public override int GetHashCode()
        {
            var hashCode = 711225152;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChartName);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<ChartSeries>>.Default.GetHashCode(Series);
            return hashCode;
        }

        public static bool operator ==(ChartData data1, ChartData data2)
        {
            return EqualityComparer<ChartData>.Default.Equals(data1, data2);
        }

        public static bool operator !=(ChartData data1, ChartData data2)
        {
            return !(data1 == data2);
        }
    }
    public class ChartSeries : IEquatable<ChartSeries>
    {
        public ChartSeries(string name, ChartPoint[] points)
        {
            Name = name;
            Points = points;
        }
        public string Name { get; set; }
        public ChartPoint[] Points { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartSeries);
        }

        public bool Equals(ChartSeries other)
        {
            return other != null &&
                   Name == other.Name &&
                   EqualityComparer<ChartPoint[]>.Default.Equals(Points, other.Points);
        }

        public override int GetHashCode()
        {
            var hashCode = -904786212;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<ChartPoint[]>.Default.GetHashCode(Points);
            return hashCode;
        }

        public static bool operator ==(ChartSeries serie1, ChartSeries serie2)
        {
            return EqualityComparer<ChartSeries>.Default.Equals(serie1, serie2);
        }

        public static bool operator !=(ChartSeries serie1, ChartSeries serie2)
        {
            return !(serie1 == serie2);
        }
    }

    public class ChartPoint : IEquatable<ChartPoint>
    {
        public ChartPoint(DateTimeOffset? x, double? y)
        {
            X = x;
            Y = y;
        }
        public DateTimeOffset? X { get; set; }
        public double? Y { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartPoint);
        }

        public bool Equals(ChartPoint other)
        {
            return other != null &&
                   EqualityComparer<DateTimeOffset?>.Default.Equals(X, other.X) &&
                   EqualityComparer<double?>.Default.Equals(Y, other.Y);
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset?>.Default.GetHashCode(X);
            hashCode = hashCode * -1521134295 + EqualityComparer<double?>.Default.GetHashCode(Y);
            return hashCode;
        }

        public static bool operator ==(ChartPoint point1, ChartPoint point2)
        {
            return EqualityComparer<ChartPoint>.Default.Equals(point1, point2);
        }

        public static bool operator !=(ChartPoint point1, ChartPoint point2)
        {
            return !(point1 == point2);
        }
    }
}
