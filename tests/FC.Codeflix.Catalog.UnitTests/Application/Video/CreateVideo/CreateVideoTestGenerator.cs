namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

public class CreateVideoTestGenerator
{
    public static IEnumerable<object[]> GenerateInvalidVideoInputs(int? length = 12)
    {
        var fixture = new CreateVideoTestFixture();
        var invalidInputList = new List<object[]>();
        const int totalInvalidCases = 6;

        for (var index = 0; index < length; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add([
                        fixture.CreateVideoInput(title: ""),
                        "Title is required"
                    ]);
                    break;
                case 1:
                    invalidInputList.Add([
                        fixture.CreateVideoInput(description: ""),
                        "Description is required"
                    ]);
                    break;
                case 2:
                    invalidInputList.Add([
                        fixture.CreateVideoInput(title: fixture.GetTooShortTitle()),
                        "Title must be between 3 and 255 characters"
                    ]);
                    break;
                case 3:
                    invalidInputList.Add([
                        fixture.CreateVideoInput(description: fixture.GetTooShortDescription()),
                        "Description must be between 3 and 4.000 characters"
                    ]);
                    break;
                case 4:
                    invalidInputList.Add([
                        fixture.CreateVideoInput(title: fixture.GetTooLongTitle()),
                        "Title must be between 3 and 255 characters"
                    ]);
                    break;
                case 5:
                    invalidInputList.Add([
                        fixture.CreateVideoInput(description: fixture.GetTooLongDescription()),
                        "Description must be between 3 and 4.000 characters"
                    ]);
                    break;
            }
        }

        return invalidInputList;
    }
}
