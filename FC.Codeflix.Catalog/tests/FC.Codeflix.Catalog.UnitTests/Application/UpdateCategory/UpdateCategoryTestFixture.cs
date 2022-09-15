using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory
{
    [CollectionDefinition(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }

    public class UpdateCategoryTestFixture : BaseFixture
    {
        public Mock<ICategoryRepository> GetRepositoryMock() => new();

        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

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

        public UpdateCategoryInput GetValidInput(Guid? id = null)
        {
            return new UpdateCategoryInput(
                   id ?? Guid.NewGuid(),
                   GetValidCategoryName(),
                   GetValidCategoryDescription(),
                   GetRandomBoolean()
               );
        }

        public UpdateCategoryInput GetInvalidInputShortName()
        {
            var invalidInputShortName = GetValidInput();
            invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);

            return invalidInputShortName;
        }

        public UpdateCategoryInput GetInvalidInputTooLongName()
        {
            var invalidInputTooLongName = GetValidInput();
            invalidInputTooLongName.Name = Faker.Lorem.Letter(256);

            return invalidInputTooLongName;
        }

        public UpdateCategoryInput GetInvalidInputDescriptionNull()
        {
            var invalidInputDescriptionNull = GetValidInput();
            invalidInputDescriptionNull.Description = null!;

            return invalidInputDescriptionNull;
        }

        public UpdateCategoryInput GetInvalidInputDescriptionTooLong()
        {
            var invalidInputDescriptionTooLong = GetValidInput();
            invalidInputDescriptionTooLong.Description = Faker.Lorem.Letter(10_001);

            return invalidInputDescriptionTooLong;
        }

    }
}
