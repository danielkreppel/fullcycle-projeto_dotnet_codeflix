using FC.Codeflix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common
{
    public class CategoryBaseFixture : BaseFixture
    {
        public CategoryPersistence Persistence;

        public CategoryBaseFixture()
        {
            Persistence = new CategoryPersistence(CreateDbContextSample());
        }

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

        public string GetInvalidNameTooShort() => Faker.Commerce.ProductName().Substring(0, 2);

        public string GetInvalidNameTooLong() => Faker.Lorem.Letter(256);

        public string GetInvalidDescriptionTooLong() => Faker.Lorem.Letter(10_001);

        public DomainEntity.Category GetCategorySample() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

        public List<DomainEntity.Category> GetCategoriesListSample(int listLength = 15)
            => Enumerable.Range(1, listLength).Select(_ => new DomainEntity.Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean())).ToList();
    }
}
