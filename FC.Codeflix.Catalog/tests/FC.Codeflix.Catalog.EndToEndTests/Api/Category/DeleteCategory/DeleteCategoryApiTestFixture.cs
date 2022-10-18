using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory
{
    [CollectionDefinition(nameof(DeleteCategoryApiTestFixture))]
    public class DeleteCategoryApiTestFixtureCollection : ICollectionFixture<DeleteCategoryApiTestFixture> { }
    public class DeleteCategoryApiTestFixture : CategoryBaseFixture
    {
    }
}
