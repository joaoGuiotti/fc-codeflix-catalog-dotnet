using System.Runtime.CompilerServices;
using Bogus;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.ValueObject;

public class ImageTest
{
    private Faker _faker => new Faker();

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Image - ValueObjects")]
    public void Instantiate()
    {
        var urlImage = _faker.Internet.Url();
        var image = new Image(urlImage);

        image.Path.Should().Be(urlImage);
    }

    [Fact(DisplayName = nameof(EqualsByPath))]
    [Trait("Domain", "Image - ValueObjects")]
    public void EqualsByPath()
    {
        var path = _faker.Internet.Url();
        var image = new Image(path);
        var image2 = new Image(path);

        var isEquals = image == image2;

        isEquals.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(DifferentByPath))]
    [Trait("Domain", "Image - ValueObjects")]
    public void DifferentByPath()
    {
        var path = _faker.Internet.Url();
        var differentPath = _faker.Internet.Url();
        var image = new Image(path);
        var image2 = new Image(differentPath);

        var isDifferent = !image.Equals(image2);

        isDifferent.Should().BeTrue();
    }
}
