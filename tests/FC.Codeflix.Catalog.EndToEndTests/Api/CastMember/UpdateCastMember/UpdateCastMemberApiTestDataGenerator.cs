using FC.Codeflix.Catalog.Api.Contracts.CastMembers;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

public class UpdateCastMemberApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateCastMemberApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add([
                        new UpdateCastMemberRequest(
                            fixture.GetInvalidNameTooShort(),
                            fixture.GetRandomType()
                        ),
                        "Name must be at least 3 characters long."
                    ]);
                    break;
                case 1:
                    invalidInputsList.Add([
                        new UpdateCastMemberRequest(
                            fixture.GetInvalidNameTooLong(),
                            fixture.GetRandomType()
                        ),
                        "Name must be less or equal 255 characters."
                    ]);
                    break;
                case 2:
                    invalidInputsList.Add([
                        new UpdateCastMemberRequest(
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
