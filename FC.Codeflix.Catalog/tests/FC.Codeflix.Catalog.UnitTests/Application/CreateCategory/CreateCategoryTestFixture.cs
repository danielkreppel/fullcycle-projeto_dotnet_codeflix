using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory
{
    [CollectionDefinition(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>
    {}

    public class CreateCategoryTestFixture : BaseFixture
    {
        public CreateCategoryTestFixture() : base() { }

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

        public CreateCategoryInput GetInput()
        {
            return new(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
                );
        }

        public CreateCategoryInput GetInvalidInputShortName()
        {
            var invalidInputShortName = GetInput();
            invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);

            return invalidInputShortName;
        }

        public CreateCategoryInput GetInvalidInputTooLongName()
        {
            var invalidInputTooLongName = GetInput();
            invalidInputTooLongName.Name = Faker.Lorem.Letter(256);

            return invalidInputTooLongName;
        }

        public CreateCategoryInput GetInvalidInputDescriptionNull()
        {
            var invalidInputDescriptionNull = GetInput();
            invalidInputDescriptionNull.Description = null!;

            return invalidInputDescriptionNull;
        }

        public CreateCategoryInput GetInvalidInputDescriptionTooLong()
        {
            var invalidInputDescriptionTooLong = GetInput();
            invalidInputDescriptionTooLong.Description = Faker.Lorem.Letter(10_001);

            return invalidInputDescriptionTooLong;
        }

        public Mock<ICategoryRepository> GetRepositoryMock() => new ();

        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new ();
    }
}
