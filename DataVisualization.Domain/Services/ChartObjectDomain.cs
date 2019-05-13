using Highsoft.Web.Mvc.Charts;
using DataVisualization.Domain.Contracts;
using DataVisualization.Models.Workspace;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("DataVisualization.Config")]
namespace DataVisualization.Domain.Services
{
    public class ChartObjectDomain : BaseDomain<int, ChartObject>, IChartObjectDomain
    {

        private IChartObjectRepository ChartObjectRepository => repository as IChartObjectRepository;

        private readonly ICategoryDomain _categoryDomain;

        public ChartObjectDomain(
            IChartObjectRepository repository,
            ICategoryDomain categoryDomain
           ) : base(repository)
        {
            _categoryDomain = categoryDomain;
        }

        public override ChartObject Add(ChartObject entity)
        {
            GetUniqueChartObjectName(entity, entity.CategoryId);

            return base.Add(entity);
        }

        public ChartObject ChangeCategory(int chartObjectId, int newCategoryId)
        {

            var chartObject = GetById(chartObjectId);

            if (chartObject == null)
                throw new ArgumentException("Chart object couldn't be found", "chartObjectId");

            if (chartObject.CategoryId == newCategoryId)
                return chartObject;

            chartObject.CategoryId = newCategoryId;
            GetUniqueChartObjectName(chartObject, newCategoryId);

            return Update(chartObject);

        }

        public IEnumerable<ChartObject> GetByCategoryId(int categoryId)
        {
            return ChartObjectRepository.GetByCategoryId(categoryId);
        }

        public IEnumerable<ChartObject> SearchByTitleAndDescription(string searchTerm)
        {
            return ChartObjectRepository.SearchByTitleAndDescription(searchTerm);
        }

        public ChartObject Copy(int chartObjectToCopyId, int destinationCategoryId, string ownerId)
        {
            var chartObjectToCopy = GetById(chartObjectToCopyId);

            if (chartObjectToCopy == null)
                throw new ArgumentException("Couldn't find chart object", "chartObjectToCopyId");

            var destinationCategory = _categoryDomain.GetById(destinationCategoryId);

            if (destinationCategory == null)
                throw new ArgumentException("Couldn't find destination category", "destinationCategory");

            var copiedChartObject = chartObjectToCopy.Copy(ownerId,destinationCategoryId);
            GetUniqueChartObjectName(copiedChartObject, destinationCategoryId);
            Add(copiedChartObject);
            return copiedChartObject;

        }

        public string GetUniqueChartObjectName(ChartObject chartObject, int parentCategoryId)
        {

            //Mach all numbers at the end of the name surronded by round brackets
            string pattern = Regex.Escape(chartObject.Title) + @"\s{0,1}(\((?<number>\d+)\)){0,1}$";

            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

            bool nameIsUnique = true;
            int currentNumber = 0;
            var parentCategory = _categoryDomain.GetById(parentCategoryId);
            if (parentCategory == null)
                throw new ArgumentException("Couldn't find parent category", "parentCategoryId");

            foreach (var childChartObject in parentCategory.ChartObjects)
            {
                if (childChartObject.Id == chartObject.Id)
                    continue;

                var match = Regex.Match(childChartObject.Title, pattern, options);
                if (match.Success)
                {
                    nameIsUnique = false;
                    if (match.Groups["number"].Success)
                    {
                        int parsedNumber = 0;
                        int.TryParse(match.Groups["number"].Value, out parsedNumber);
                        if (parsedNumber > currentNumber)
                        {
                            currentNumber = parsedNumber;
                        }
                    }
                }
            }

            if (!nameIsUnique)
            {
                chartObject.Title = string.Format("{0}({1})", chartObject.Title, currentNumber + 1);
            }

            return chartObject.Title;
        }
    }
}
