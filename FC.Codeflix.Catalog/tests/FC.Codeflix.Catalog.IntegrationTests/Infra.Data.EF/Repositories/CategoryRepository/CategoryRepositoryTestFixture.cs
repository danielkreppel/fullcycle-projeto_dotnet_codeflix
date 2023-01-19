using Bogus;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository
{
    [CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
    public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoryTestFixture> { }
    public class CategoryRepositoryTestFixture : BaseFixture
    {
        public string GetValidCategoryName()
        {
            var categoryName = "";

            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];

            if (categoryName.Length > 255)
                categoryName = categoryName[..255];

            return categoryName;
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();

            if (categoryDescription.Length > 10_000)
                categoryDescription = categoryDescription[..10_000];

            return categoryDescription;
        }

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public DomainEntity.Category GetValidCategorySample()
        {
            return new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());
        }

        public List<DomainEntity.Category> GetValidCategorySampleList(int length = 10)
        {
            return Enumerable.Range(0, length)
                .Select(_ => GetValidCategorySample()).ToList();
        }

        public List<DomainEntity.Category> GetValidCategorySampleListWithNames(List<string> names)
        {
            return names.Select(name =>
            {
                var category = GetValidCategorySample();
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
