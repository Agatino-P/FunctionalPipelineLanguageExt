using LanguageExt;
using System.Text;

namespace FuncPipeline.Domain;

public class Pipeline
{
    private readonly IFirstNameValidationService firstNameValidationService;

    public Pipeline(IFirstNameValidationService firstNameValidationService)
    {
        this.firstNameValidationService = firstNameValidationService;
    }
    public PipelineOutput Process(PipelineInput input)
    {
        return ToEither(input)
            .Bind((c)=>ValidateFirstName(c, firstNameValidationService))
            .Map(CheckLastName)
            .Map(LowerFirstName)
            .Map(UpperLastName)
            .Bind(ValidateAge)
            .Map(DetermineAgeRange)
            .Match<PipelineOutput>(CreateSuccessOutcome, CreateFailureOutcome);
    }

    internal static Either<List<string>, PipelineContext> ToEither(PipelineInput input)
        => Either<List<string>, PipelineContext>.Right(new PipelineContext(input));

    internal Either<List<string>,PipelineContext> ValidateFirstName(        
        PipelineContext context, IFirstNameValidationService firstNameValidationService)
    {
        return firstNameValidationService.IsValid(context.FirstName)
            ? Success(context)
            : Failure([.. context.History, "Invalid Firstname"]);
            
    }

    internal PipelineContext CheckLastName(PipelineContext context) =>
        context.LastName.Length <= 2
            ? context with { History = [.. context.History, "LastName was really short"] }
            : context;

    internal static PipelineContext LowerFirstName(PipelineContext context) => 
        context with { FirstName = context.FirstName.ToLower() };
    internal static PipelineContext UpperLastName(PipelineContext context) =>
        context with { LastName = context.LastName.ToUpper() };

    internal static Either<List<string>, PipelineContext> ValidateAge(PipelineContext context)
    {
        return context.Age < 0
            ? Failure([.. context.History, "Age is less than zero"])
            : Success(context);
    }

    internal static PipelineContext DetermineAgeRange(PipelineContext context) =>
        context with { AgeRange = context.Age < 50 ? AgeRange.Young : AgeRange.Old };

    private static PipelineOutput CreateSuccessOutcome(PipelineContext context)
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
        return new PipelineOutput(true, sentence.ToString(), context.History);
    }
    private static PipelineOutput CreateFailureOutcome(List<string> history)
    {
        return new PipelineOutput(false, "", history);
    }

    private static Either<List<string>, PipelineContext> Success(PipelineContext context)
        => Either<List<string>, PipelineContext>.Right(context);
    private static Either<List<string>, PipelineContext> Failure(List<string> errors)
        => Either<List<string>, PipelineContext>.Left(errors);

}
