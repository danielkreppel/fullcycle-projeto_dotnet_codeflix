using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    [CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
    public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture> { }
    public class UpdateCategoryApiTestFixture : CategoryBaseFixture
    {
        public UpdateCategoryInput GetInputSample(Guid? id = null)
        {
            return new (
                id ?? Guid.NewGuid(),
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );
        }
    }
}
