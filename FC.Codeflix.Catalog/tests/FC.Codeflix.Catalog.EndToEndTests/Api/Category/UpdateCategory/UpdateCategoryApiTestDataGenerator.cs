using FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    public class UpdateCategoryApiTestDataGenerator
    {
        public static IEnumerable<object[]> GetInvalidInputs()
        {
            var fixture = new UpdateCategoryApiTestFixture();
            var invalidInputs = new List<object[]>();
            var totalInvalidCases = 3;

            for (int i = 0; i < totalInvalidCases; i++)
            {
                switch (i % totalInvalidCases)
                {
                    case 0:
                        var input1 = fixture.GetInputSample();
                        input1.Name = fixture.GetInvalidNameTooShort();
                        invalidInputs.Add(new object[] { input1, "Name should be at least 3 characters long" });
                        break;
                    case 1:
                        var input2 = fixture.GetInputSample();
                        input2.Name = fixture.GetInvalidNameTooLong();
                        invalidInputs.Add(new object[] { input2, "Name should be less or equal 255 characters long" });
                        break;
                    case 2:
                        var input3 = fixture.GetInputSample();
                        input3.Description = fixture.GetInvalidDescriptionTooLong();
                        invalidInputs.Add(new object[] { input3, "Description should be less or equal 10000 characters long" });
                        break;
                    default:
                        break;
                }
            }

            return invalidInputs;
        }
    }
}
