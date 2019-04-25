using DataVisualization.Context;
using DataVisualization.Models.Workspace;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Services
{
    public class CategoryRepository : BaseRepository<int, Category>, ICategoryRepository
    {
        public CategoryRepository(VisContext context) : base(context) { }

        public IEnumerable<Category> SearchByTitleAndDescription(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            var retVal = _entities.Where(ca => ca.Title.ToLower().Contains(lowerSearchTerm)
            || ca.Description.ToLower().Contains(lowerSearchTerm));
            return retVal;

        }

        public Category GetRootByOwnerId(string ownerId)
        {
            return context.Set<Category>().SingleOrDefault(c => c.IsRoot && c.OwnerId == ownerId);

        }

        public override bool Delete(int id)
        {
            var categoryToDelete = GetById(id);

            RecursiveDelete(categoryToDelete);

            var retVal = Save();
            return retVal == 1;

        }

        public override bool Delete(Category entityToDelete)
        {
            return Delete(entityToDelete.Id);

        }

        private void RecursiveDelete(Category parent)
        {
            if (parent.ChildCategories != null)
            {
                var childCategories = parent.ChildCategories.ToList();

                foreach (var child in childCategories)
                {
                    RecursiveDelete(child);
                }
            }

            _entities.Remove(parent);

        }
    }
}
