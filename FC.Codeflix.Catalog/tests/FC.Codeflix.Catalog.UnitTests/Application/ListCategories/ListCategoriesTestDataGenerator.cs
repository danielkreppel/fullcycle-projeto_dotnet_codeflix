using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories
{
    public class ListCategoriesTestDataGenerator
    {
        public static IEnumerable<object[]> GetInputsWithoutAllParameters(int times = 14)
        {
            var fixture = new ListCategoriesTestFixture();
            var inputSample = fixture.GetInputSample();

            for (int i=0; i<times; i++)
            {
                switch(i % 7)
                {
                    case 0:
                        yield return new object[] { new ListCategoriesInput() };
                        break;
                    case 1:
                        yield return new object[] { new ListCategoriesInput(inputSample.Page) };
                        break;
                    case 2:
                        yield return new object[] { new ListCategoriesInput(inputSample.Page, inputSample.PerPage) };
                        break;
                    case 3:
                        yield return new object[] { new ListCategoriesInput(inputSample.Page, inputSample.PerPage, inputSample.Search) };
                        break;
                    case 4:
                        yield return new object[] { new ListCategoriesInput(inputSample.Page, inputSample.PerPage, inputSample.Search, inputSample.SortBy) };
                        break;
                    case 5:
                        yield return new object[] { new ListCategoriesInput(inputSample.Page, inputSample.PerPage, inputSample.Search, inputSample.SortBy, inputSample.SortDir) };
                        break;
                    default:
                        yield return new object[] { new ListCategoriesInput() };
                        break;
                }
            }
        }
    }
}
