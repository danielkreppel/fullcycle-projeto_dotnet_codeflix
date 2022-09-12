using FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category
{
    public class CreateCategoryTestDataGenerator
    {
        public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
        {
            var fixture = new CreateCategoryTestFixture();
            var invalidInputs = new List<object[]>();
            var totalInvalidCases = 4;

            for (int i = 0; i < times; i++)
            {
                switch(i % totalInvalidCases)
                {
                    case 0:
                        invalidInputs.Add(new object[] { fixture.GetInvalidInputShortName(), "Name should be at least 3 characters long" });
                        break;
                    case 1:
                        invalidInputs.Add(new object[] { fixture.GetInvalidInputTooLongName(), "Name should be less or equal 255 characters long" });
                        break;
                    case 2:
                        invalidInputs.Add(new object[] { fixture.GetInvalidInputDescriptionNull(), "Description should not be null" });
                        break;
                    case 3:
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
