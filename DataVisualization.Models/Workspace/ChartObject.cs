using DataVisualization.Models.Chart;
using DataVisualization.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DataVisualization.Models.Workspace
{
    public class ChartObject : IEquatable<ChartObject>
    {
        protected ChartObject()
        {
        }

        public ChartObject(string title)
        {
            Title = title;
        }

        public ChartObject(string title,string description) : this(title)
        {
            Description = description;
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        public string OwnerId { get; set; }

        [JsonIgnore]
        public ChartsConfigSchemaChartChartTypeValue ChartType { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser Owner { get; set; }

        [JsonIgnore]
        public virtual ChartDataSource ChartDataSource { get; set; }

        [NotMapped]
        public bool IsEveryones
        {
            get
            {
                return Category?.IsEveryones ?? false;
            }
        }

        [JsonIgnore]
        public string  Thumbnail { get; set; }

        public ChartObject Copy(string ownerId)
        {
            return new ChartObject
            {
                Title = Title,
                Description = Description,
                OwnerId = ownerId,
                Thumbnail = Thumbnail,
                ChartDataSource = ChartDataSource?.Copy()
            };
        }

        public ChartObject Copy(string ownerId,int newCategoryId)
        {
            return new ChartObject
            {
                Title = Title,
                Description = Description,
                CategoryId = newCategoryId,
                OwnerId = ownerId,
                Thumbnail = Thumbnail,
                ChartDataSource = ChartDataSource?.Copy()
            };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartObject);
        }

        public bool Equals(ChartObject other)
        {
            return other != null &&
                   Id == other.Id &&
                   Title == other.Title &&
                   Description == other.Description &&
                   CategoryId == other.CategoryId &&
                   OwnerId == other.OwnerId &&
                   EqualityComparer<ChartDataSource>.Default.Equals(ChartDataSource, other.ChartDataSource) &&
                   IsEveryones == other.IsEveryones;
        }

        public override int GetHashCode()
        {
            var hashCode = 1723322677;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + CategoryId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OwnerId);
            hashCode = hashCode * -1521134295 + EqualityComparer<ChartDataSource>.Default.GetHashCode(ChartDataSource);
            hashCode = hashCode * -1521134295 + IsEveryones.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ChartObject left, ChartObject right)
        {
            return EqualityComparer<ChartObject>.Default.Equals(left, right);
        }

        public static bool operator !=(ChartObject left, ChartObject right)
        {
            return !(left == right);
        }
    }
}
