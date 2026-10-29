using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

public class UpdateGenreApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateGenreApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 2;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = new UpdateGenreInput(
                        Guid.NewGuid(),
                        fixture.GetInvalidNameTooShort(),
                        fixture.GetRandomBoolean()
                    );
                    invalidInputsList.Add(
                        new object[]
                        {
                            input1,
                            "Name must be at least 3 characters long.",
                        }
                    );
                    break;
                case 1:
                    var input2 = new UpdateGenreInput(
                        Guid.NewGuid(),
                        fixture.GetInvalidNameTooLong(),
                        fixture.GetRandomBoolean()
                    );
                    invalidInputsList.Add(
                        new object[]
                        {
                            input2,
                            "Name must be less or equal 255 characters.",
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

