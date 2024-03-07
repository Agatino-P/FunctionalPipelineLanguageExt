using LanguageExt;
using System.Text;

namespace FuncPipeline.Domain;

public class Pipeline
{
    public PipelineOutcome Process(PipelineContext pipelineContext)
    {
        return ToEither(pipelineContext)
            .Bind(ValidateFirstName)
            .Map(CheckLastName)
            .Map(LowerFirstName)
            .Map(UpperLastName)
            .Bind(ValidateAge)
            .Map(DetermineAgeRange)
            .Match<PipelineOutcome>(CreateSuccessOutcome, CreateFailureOutcome);
    }

    public static Either<List<string>, PipelineContext> ToEither(PipelineContext context) 
        => Either<List<string>, PipelineContext>.Right(context);

    public Either<List<string>,PipelineContext> ValidateFirstName(PipelineContext context)
    {
        return string.IsNullOrEmpty(context.FirstName)
            ? Failure([.. context.History, "Invalid Firstname"])
            : Success(context);
    }

    public PipelineContext CheckLastName(PipelineContext context) =>
        context.LastName.Length <= 2
            ? context with { History = [.. context.History, "LastName was really short"] }
            : context;

    public static PipelineContext LowerFirstName(PipelineContext context) => 
        context with { FirstName = context.FirstName.ToLower() };
    public static PipelineContext UpperLastName(PipelineContext context) =>
        context with { LastName = context.LastName.ToUpper() };

    public static Either<List<string>, PipelineContext> ValidateAge(PipelineContext context)
    {
        return context.Age < 0
            ? Failure([.. context.History, "Age is less than zero"])
            : Success(context);
    }

    public static PipelineContext DetermineAgeRange(PipelineContext context) =>
        context with { AgeRange = context.Age < 50 ? AgeRange.Young : AgeRange.Old };

    private static PipelineOutcome CreateSuccessOutcome(PipelineContext context)
    {
        Dictionary<AgeRange, string> AgeFeelings = new(){
            { AgeRange.Undefined, "confused"},
            { AgeRange.Young, "still young"},
            { AgeRange.Old, "not so young"},
        };

        StringBuilder sentence = new StringBuilder(context.FirstName)
            .Append(' ')
            .Append(context.LastName)
            .Append(" is feeling ")
            .Append(AgeFeelings[context.AgeRange]);
        return new PipelineOutcome(true, sentence.ToString(), context.History);
    }
    private static PipelineOutcome CreateFailureOutcome(List<string> history)
    {
        return new PipelineOutcome(false, "", history);
    }

    private static Either<List<string>, PipelineContext> Success(PipelineContext context)
        => Either<List<string>, PipelineContext>.Right(context);
    private static Either<List<string>, PipelineContext> Failure(List<string> errors)
        => Either<List<string>, PipelineContext>.Left(errors);

}
