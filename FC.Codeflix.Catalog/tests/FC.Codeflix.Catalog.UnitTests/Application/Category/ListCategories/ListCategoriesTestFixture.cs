using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>
    { }

    public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
    {
        public List<DomainEntity.Category> GetListGategoriesSample(int length = 10)
        {
            var list = new List<DomainEntity.Category>();
            for (int i = 0; i < length; i++)
            {
                list.Add(GetValidCategorySample());
            }

            return list;
        }

        public ListCategoriesInput GetInputSample()
        {
            var random = new Random();
            return new ListCategoriesInput(
                    page: random.Next(1, 10),
                    perPage: random.Next(15, 100),
                    search: Faker.Commerce.ProductName(),
                    sortBy: Faker.Commerce.ProductName(),
                    sortDir: random.Next(0, 10) > 5 ? SearchOrder.ASC : SearchOrder.DESC
                );
        }
    }
}
