﻿using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.GetCategory
{
    public  class GetCategoryTestFixture : BaseFixture
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

        public Category GetValidCategory()
        {
            return new(GetValidCategoryName(), GetValidCategoryDescription());
        }
        public Mock<ICategoryRepository> GetRepositoryMock() => new();
    }

    [CollectionDefinition(nameof(GetCategoryTestFixture))]
    public class GetCategoryFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }
}
