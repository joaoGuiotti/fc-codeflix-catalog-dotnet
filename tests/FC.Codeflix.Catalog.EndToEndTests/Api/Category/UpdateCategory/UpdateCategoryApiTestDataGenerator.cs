namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

public class UpdateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateCategoryApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetExampleInput();
                    input1.Name = fixture.GetInvalidNameTooShort();
                    invalidInputsList.Add(
                        new object[]
                        {
                            input1,
                            "Name must be at least 3 characters long.",
                        }
                    );
                    break;
                case 1:
                    var input2 = fixture.GetExampleInput();
                    input2.Name = fixture.GetInvalidNameTooLong();
                    invalidInputsList.Add(
                        new object[]
                        {
                            input2,
                            "Name must be less or equal 255 characters.",
                        }
                    );
                    break;
                case 2:
                    var input4 = fixture.GetExampleInput();
                    input4.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputsList.Add(
                        new object[]
                        {
                            input4,
                            "Description must be less or equal 10.000 caracteres.",
                        }
                    );
                    break;
                default:
                    break;
            }
        }

        return invalidInputsList;
    }

}
