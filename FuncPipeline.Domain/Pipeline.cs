using LanguageExt;
using System.Text;

namespace FuncPipeline.Domain;

public class Pipeline
{
    public PipelineOutcome Process(PipelineContext pipelineContext)
    {
        return ToEither(pipelineContext)
            .Bind(ValidateFirstName)
            .Bind(CheckLastName)
            .Map(LowerFirstName)
            .Map(UpperLastName)
            .Bind(ValidateAge)
            .Map(DetermineAgeRange)
            .Match<PipelineOutcome>(CreateSuccessOutcome, CreateFailureOutcome);
    }

    public Either<List<string>, PipelineContext> ToEither(PipelineContext context) 
        => Either<List<string>, PipelineContext>.Right(context);

    public Either<List<string>,PipelineContext> ValidateFirstName(PipelineContext context)
    {
        return string.IsNullOrEmpty(context.FirstName)
            ? Either<List<string>, PipelineContext>.Left([.. context.History, "Invalid Firstname"])
            : Either<List<string>, PipelineContext>.Right(context);
    }

    public Either<List<string>, PipelineContext> CheckLastName(PipelineContext context)
    {
        List<string> newHistory = new(context.History);
        if (context.LastName.Length <= 2)
        {
            newHistory.Add("LastName was really short");
        }
        return context with { History = newHistory };

    }

    public static PipelineContext LowerFirstName(PipelineContext context) => context with { FirstName = context.FirstName.ToLower() };
    public static PipelineContext UpperLastName(PipelineContext context) => context with { LastName = context.LastName.ToUpper() };

    public Either<List<string>, PipelineContext> ValidateAge(PipelineContext context)
    {
        return context.Age < 0
            ? Either<List<string>, PipelineContext>.Left([.. context.History, "Age lkess than zero"])
            : Either<List<string>, PipelineContext>.Right(context);
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

        StringBuilder sentence = new StringBuilder(context.FirstName);
        sentence.Append(' ')
            .Append(context.LastName)
            .Append(" is feeling ")
            .Append(AgeFeelings[context.AgeRange]);
        return new PipelineOutcome(true, sentence.ToString(), context.History);
    }
    private static PipelineOutcome CreateFailureOutcome(List<string> history)
    {
        return new PipelineOutcome(false, "", history);
    }

}
