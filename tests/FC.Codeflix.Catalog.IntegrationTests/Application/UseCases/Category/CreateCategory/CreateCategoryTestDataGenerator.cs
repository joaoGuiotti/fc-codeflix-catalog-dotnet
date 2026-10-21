namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(
                        new object[]
                        {
                            fixture.GetInvalidInputShortName(),
                            "Name must be at least 3 characters long.",
                        }
                    );
                    break;
                case 1:
                    invalidInputsList.Add(
                        new object[]
                        {
                            fixture.GetInvalidInputTooLongName(),
                            "Name must be less or equal 255 characters.",
                        }
                    );
                    break;
                case 2:
                    invalidInputsList.Add(
                        new object[]
                        {
                            fixture.GetInvalidInputCategoryNull(),
                            "Name cannot be null or empty.",
                        }
                    );
                    break;
                case 3:
                    invalidInputsList.Add(
                        new object[]
                        {
                            fixture.GetInvalidInputTooLongDescription(),
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

