
namespace FC.Codeflix.Catalog.UnitTests.Application.Category.UpdateCategory
{
    public class UpdateCategoryTestDataGenerator
    {
        public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
        {
            var fixture = new UpdateCategoryTestFixture();
            for (int i = 0; i < times; i++)
            {
                var categorySample = fixture.GetValidCategorySample();
                var inputSample = fixture.GetValidInput(categorySample.Id);

                yield return new object[] { categorySample, inputSample };
            }
        }

        public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
        {
            var fixture = new UpdateCategoryTestFixture();
            var invalidInputs = new List<object[]>();
            var totalInvalidCases = 3;

            for (int i = 0; i < times; i++)
            {
                switch (i % totalInvalidCases)
                {
                    case 0:
                        invalidInputs.Add(new object[] { fixture.GetInvalidInputShortName(), "Name should be at least 3 characters long" });
                        break;
                    case 1:
                        invalidInputs.Add(new object[] { fixture.GetInvalidInputTooLongName(), "Name should be less or equal 255 characters long" });
                        break;
                    case 2:
                        invalidInputs.Add(new object[] { fixture.GetInvalidInputDescriptionTooLong(), "Description should be less or equal 10000 characters long" });
                        break;
                    default:
                        break;
                }
            }

            return invalidInputs;
        }
    }
}
