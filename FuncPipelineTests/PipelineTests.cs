using FluentAssertions;
using FuncPipeline.Domain;
using NSubstitute;
namespace FuncPipelineTests;

/*Take a Person Class
Validate FirstName is not null
Track LastName is less than 2 characters
Convert FirstName to LowerCase
Convert Lastname to UpperCase
Validate Age > 0
Fill comment based on age <50
Each step logs something
Early failure
In the end return a single outcome with Full name and Comment or list of comments
*/

public class PipelineTests
{
    readonly IFirstNameValidationService fnvs = Substitute.For<IFirstNameValidationService>();
    private readonly Pipeline sut;
    
    public PipelineTests()
    {
        sut = new(fnvs);
        fnvs.IsValid(Arg.Any<string>()).Returns(true);
    }
    [Fact]
    public void AcceptValidContext()
    {
        PipelineOutput actual = sut.Process(aValidInput);
        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RejectEmptyFirstName()
    {
        fnvs.IsValid(Arg.Any<string>()).Returns(false);
        PipelineOutput actual = sut.Process(aValidInput with { FirstName=""});
        actual.IsValid.Should().BeFalse();
    }

    [Fact]
    public void TrackLastNameLessThanTwoCharacters()
    {
        PipelineOutput actual = sut.Process(aValidInput with { LastName = "P" });
        actual.IsValid.Should().BeTrue();
        actual.Errors.Exists(e => e.Contains("short")).Should().BeTrue();
    }

    [Fact]
    public void ContainLowercaseFirstNameInSentence()
    {
        PipelineOutput actual = sut.Process(aValidInput);
        actual.Sentence.Contains(aValidInput.FirstName.ToLower()).Should().BeTrue();
    }

    [Fact]
    public void ContainUppercaseLastNameInSentence()
    {
        PipelineOutput actual = sut.Process(aValidInput);
        actual.Sentence.Contains(aValidInput.LastName.ToUpper()).Should().BeTrue();
    }

    [Fact]
    public void RejectNegativeAge()
    {
        PipelineOutput actual = sut.Process(aValidInput with { Age=-12});
        actual.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CommentOnYoungAge()
    {
        PipelineOutput actual = sut.Process(aValidInput with { Age=25});
        actual.Sentence.Contains("still young").Should().BeTrue();
    }
    [Fact]
    public void CommentOnOldAge()
    {
        PipelineOutput actual = sut.Process(aValidInput with { Age = 90 });
        actual.Sentence.Contains("not so young").Should().BeTrue();
    }
    
    [Fact]
    public void ReportAllErrors()
    {
        PipelineOutput actual = sut.Process(aValidInput with { LastName="p", Age = -11 });
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(2);
    }


    private static readonly PipelineInput aValidInput = new("Agatino", "Pesce", 52);
}