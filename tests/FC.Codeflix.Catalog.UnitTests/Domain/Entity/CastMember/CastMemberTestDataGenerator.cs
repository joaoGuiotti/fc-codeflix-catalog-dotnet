namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

public class CastMemberTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidNames()
    {
        var fixture = new CastMemberTestFixture();
        var invalidInputsList = new List<object[]>
        {
            new object[]
            {
                null!,
                "Name cannot be null or empty."
            },
            new object[]
            {
                "",
                "Name cannot be null or empty."
            },
            new object[]
            {
                "   ",
                "Name cannot be null or empty."
            },
            new object[]
            {
                fixture.GetInvalidNameTooShort(),
                "Name must be at least 3 characters long."
            },
            new object[]
            {
                fixture.GetInvalidNameTooLong(),
                "Name must be less or equal 255 characters."
            }
        };

        return invalidInputsList;
    }
}
