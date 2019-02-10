using KGTMachineLearningWeb.Models.Chart;
using KGTMachineLearningWeb.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace KGTMachineLearningWeb.Models.Workspace
{
    public class ChartObject
    {
        protected ChartObject()
        {
        }

        public ChartObject(string title)
        {
            Title = title;
            Thumbnail = new Thumbnail();
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
        public virtual Thumbnail Thumbnail { get; set; }

        public ChartObject Copy(string ownerId)
        {
            return new ChartObject
            {
                Title = Title,
                Description = Description,
                OwnerId = ownerId,
                Thumbnail = Thumbnail?.Copy(),
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
                Thumbnail = Thumbnail?.Copy(),
                ChartDataSource = ChartDataSource?.Copy()
            };
        }
    }

    public class Thumbnail : IEquatable<Thumbnail>
    {
        public Thumbnail() { }

        public Thumbnail(string path)
        {
            SetThumbnailImage(path);
        }

        [Key,ForeignKey("ChartObject")]
        public int Id { get; set; }

        public virtual ChartObject ChartObject { get; set; }

        public byte[] Image { get; set; }


        public Thumbnail Copy()
        {
            return new Thumbnail
            {
                Image = (byte[])Image.Clone()
            };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Thumbnail);
        }

        public bool Equals(Thumbnail other)
        {
            return other != null &&
                   Id == other.Id &&
                   EqualityComparer<ChartObject>.Default.Equals(ChartObject, other.ChartObject) &&
                   EqualityComparer<byte[]>.Default.Equals(Image, other.Image);
        }

        public override int GetHashCode()
        {
            var hashCode = 1011406607;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ChartObject>.Default.GetHashCode(ChartObject);
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Image);
            return hashCode;
        }

        public Image GetThumbnailImage()
        {
            using (MemoryStream ms = new MemoryStream(Image))
            {
                Image thumbnailImage = System.Drawing.Image.FromStream(ms);
                return thumbnailImage;
            }

        }

        public void SetThumbnailImage(Image thumbnailImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                thumbnailImage.Save(ms, ImageFormat.Png);
                Image = ms.ToArray();
            }
        }

        public void SetThumbnailImage(string imagePath)
        {
            var thumbnailImage = System.Drawing.Image.FromFile(imagePath);
            using (MemoryStream ms = new MemoryStream())
            {
                thumbnailImage.Save(ms, ImageFormat.Png);
                Image = ms.ToArray();
            }

            thumbnailImage.Dispose();
        }

        public static bool operator ==(Thumbnail thumbnail1, Thumbnail thumbnail2)
        {
            return EqualityComparer<Thumbnail>.Default.Equals(thumbnail1, thumbnail2);
        }

        public static bool operator !=(Thumbnail thumbnail1, Thumbnail thumbnail2)
        {
            return !(thumbnail1 == thumbnail2);
        }
    }
}
