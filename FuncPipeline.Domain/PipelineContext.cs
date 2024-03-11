namespace FuncPipeline.Domain;

internal record PipelineContext(string FirstName, string LastName, int Age, AgeRange AgeRange, List<string> History)
{
    internal PipelineContext(string FirstName, string LastName, int Age) :
        this(FirstName, LastName, Age, AgeRange.Undefined, [])
    { }

    internal PipelineContext(PipelineInput input) :
    this(input.FirstName, input.LastName, input.Age)
    { }

}
