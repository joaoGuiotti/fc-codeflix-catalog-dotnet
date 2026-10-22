namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

public class CreateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.getExampleInput();
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
                    var input2 = fixture.getExampleInput();
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
                    var input4 = fixture.getExampleInput();
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

