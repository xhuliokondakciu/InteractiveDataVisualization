using KGTMachineLearningWeb.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Models.Workspace
{
    public class Category : IEquatable<Category>
    {
        protected Category()
        {

        }

        public Category(string title)
        {
            Title = title;
        }

        public Category(string title, string description) : this(title)
        {
            Description = description;
        }
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }

        public bool IsRoot { get; set; }

        public bool IsEveryones { get; set; }

        public int? ParentCategoryId { get; set; }

        [JsonIgnore]
        public virtual Category ParentCategory { get; set; }

        public string OwnerId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser Owner { get; set; }

        [JsonIgnore]
        public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();

        [JsonIgnore]
        public virtual ICollection<ChartObject> ChartObjects { get; set; } = new List<ChartObject>();

        [NotMapped]
        public bool HasParent
        {
            get
            {
                return ParentCategoryId.HasValue;
            }
        }

        public Category Copy(Category newParent, bool copyCharts = false)
        {
            var other = new Category
            {
                Title = Title,
                Description = Description,
                OwnerId = newParent.OwnerId,
                ParentCategoryId = newParent.Id,
                ParentCategory = newParent,
                IsEveryones = newParent.IsEveryones
            };

            other.ChildCategories = ChildCategories.Select(cc => cc.Copy(other)).ToList();

            if (copyCharts)
                other.ChartObjects = ChartObjects.Select(co => co.Copy(OwnerId)).ToList();

            return other;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Category);
        }

        public bool Equals(Category other)
        {
            return other != null &&
                   Id == other.Id &&
                   Title == other.Title &&
                   Description == other.Description &&
                   IsRoot == other.IsRoot &&
                   IsEveryones == other.IsEveryones &&
                   EqualityComparer<int?>.Default.Equals(ParentCategoryId, other.ParentCategoryId) &&
                   OwnerId == other.OwnerId;
        }

        public override int GetHashCode()
        {
            var hashCode = -1378556944;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + IsRoot.GetHashCode();
            hashCode = hashCode * -1521134295 + IsEveryones.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(ParentCategoryId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OwnerId);
            return hashCode;
        }

        public static bool operator ==(Category category1, Category category2)
        {
            return EqualityComparer<Category>.Default.Equals(category1, category2);
        }

        public static bool operator !=(Category category1, Category category2)
        {
            return !(category1 == category2);
        }
    }
}
