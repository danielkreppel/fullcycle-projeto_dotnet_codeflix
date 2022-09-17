using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>
    { }
    
    public class ListCategoriesTestFixture : BaseFixture
    {
        public Mock<ICategoryRepository> GetRepositoryMock() => new();

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

        public bool GetRandomBoolean() => (new Random()).NextDouble() < 0.5;

        public Category GetCategorySample() => new Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

        public List<Category> GetListGategoriesSample(int length = 10)
        {
            var list = new List<Category>();
            for (int i = 0; i < length; i++)
            {
                list.Add(GetCategorySample());
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
