using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture> { }
    public class ListCategoriesApiTestFixture : CategoryBaseFixture
    {
    }
}
