using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.CreateCastMember;

public class CreateCastMemberApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCastMemberApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add([
                        new CreateCastMemberInput(
                            fixture.GetInvalidNameTooShort(),
                            fixture.GetRandomType()
                        ),
                        "Name must be at least 3 characters long."
                    ]);
                    break;
                case 1:
                    invalidInputsList.Add([
                        new CreateCastMemberInput(
                            fixture.GetInvalidNameTooLong(),
                            fixture.GetRandomType()
                        ),
                        "Name must be less or equal 255 characters."
                    ]);
                    break;
                case 2:
                    invalidInputsList.Add([
                        new CreateCastMemberInput(
                            fixture.GetValidName(),
                            (ECastMemberType)99
                        ),
                        "Type must be a valid CastMember type."
                    ]);
                    break;
            }
        }

        return invalidInputsList;
    }
}
