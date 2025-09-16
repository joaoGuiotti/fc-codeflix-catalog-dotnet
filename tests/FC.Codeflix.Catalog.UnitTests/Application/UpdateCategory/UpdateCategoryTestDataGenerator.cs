namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoryToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for (int i = 0; i < times; i++)
        {
            var category = fixture.GetValidCategory();
            var input = fixture.GetValidInput(category.Id);
            yield return new object[] { category, input };
        }
    }


    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new UpdateCategoryTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(new object[] {
                        fixture.GetInvalidInputShortName(),
                        "Name must be at least 3 characters long."
                    });
                    break;
                case 1:
                    invalidInputsList.Add(new object[] {
                        fixture.GetInvalidInputTooLongName(),
                        "Name must be less or equal 255 characters."
                    });
                    break;
                case 2:
                    invalidInputsList.Add(new object[] {
                        fixture.GetInvalidInputTooLongDescription(),
                        "Description must be less or equal 10.000 caracteres."
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputsList;
    }

}
