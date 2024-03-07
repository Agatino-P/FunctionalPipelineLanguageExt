using FluentAssertions;
using FuncPipeline.Domain;
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

    private readonly Pipeline sut=new();
    
    [Fact]
    public void AcceptValidContext()
    {
        PipelineOutcome actual = sut.Process(aValidContext);
        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RejectEmptyFirstName()
    {
        PipelineOutcome actual = sut.Process(aValidContext with { FirstName=""});
        actual.IsValid.Should().BeFalse();
    }

    [Fact]
    public void TrackLastNameLessThanTwoCharacters()
    {
        PipelineOutcome actual = sut.Process(aValidContext with { LastName = "P" });
        actual.IsValid.Should().BeTrue();
        actual.Errors.Exists(e => e.Contains("short")).Should().BeTrue();
    }

    [Fact]
    public void ContainLowercaseFirstNameInSentence()
    {
        PipelineOutcome actual = sut.Process(aValidContext);
        actual.Sentence.Contains(aValidContext.FirstName.ToLower()).Should().BeTrue();
    }

    [Fact]
    public void ContainUppercaseLastNameInSentence()
    {
        PipelineOutcome actual = sut.Process(aValidContext);
        actual.Sentence.Contains(aValidContext.LastName.ToUpper()).Should().BeTrue();
    }

    [Fact]
    public void RejectNegativeAge()
    {
        PipelineOutcome actual = sut.Process(aValidContext with { Age=-12});
        actual.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CommentOnYoungAge()
    {
        PipelineOutcome actual = sut.Process(aValidContext with { Age=25});
        actual.Sentence.Contains("still young").Should().BeTrue();
    }
    [Fact]
    public void CommentOnOldAge()
    {
        PipelineOutcome actual = sut.Process(aValidContext with { Age = 90 });
        actual.Sentence.Contains("not so young").Should().BeTrue();
    }
    
    [Fact]
    public void ReportAllErrors()
    {
        PipelineOutcome actual = sut.Process(aValidContext with { LastName="p", Age = -11 });
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(2);
    }


    private static readonly PipelineContext aValidContext = new("Agatino", "Pesce", 52);
}