using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory
{
    public class UpdateCategoryTestDataGenerator
    {
        public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
        {
            var fixture = new UpdateCategoryTestFixture();
            for (int i = 0; i < times; i++)
            {
                var categorySample = fixture.GetCategorySample();
                var inputSample = new UpdateCategoryInput(
                    categorySample.Id,
                    fixture.GetValidCategoryName(),
                    fixture.GetValidCategoryDescription(),
                    fixture.GetRandomBoolean()
                );

                yield return new object[] { categorySample, inputSample };
            }
        }
    }
}
