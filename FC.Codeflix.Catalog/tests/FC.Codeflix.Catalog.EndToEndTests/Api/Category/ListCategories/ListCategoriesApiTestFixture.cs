using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture> { }
    public class ListCategoriesApiTestFixture : CategoryBaseFixture
    {
        public List<DomainEntity.Category> GetValidCategorySampleListWithNames(List<string> names)
        {
            return names.Select(name =>
            {
                var category = GetCategorySample();
                category.Update(name);
                return category;
            }).ToList();
        }

        public List<DomainEntity.Category> CloneCategoryListOrdered(List<DomainEntity.Category> categoriesList, string orderBy, SearchOrder orderDir)
        {
            var listClone = new List<DomainEntity.Category>(categoriesList);

            var orderedEnumerable = (orderBy.ToLower(), orderDir) switch
            {
                ("name", SearchOrder.ASC) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
                ("name", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                ("id", SearchOrder.ASC) => listClone.OrderBy(x => x.Id),
                ("id", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.ASC) => listClone.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.DESC) => listClone.OrderByDescending(x => x.CreatedAt),
                _ => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            };

            return orderedEnumerable.ToList();            
        }
    }
}
