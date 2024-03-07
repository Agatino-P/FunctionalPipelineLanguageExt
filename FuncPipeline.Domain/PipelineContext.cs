namespace FuncPipeline.Domain;

public record PipelineContext(string FirstName, string LastName, int Age, AgeRange AgeRange, List<string> History)
{
    public PipelineContext(string FirstName, string LastName, int Age) :
        this(FirstName, LastName, Age, AgeRange.Undefined, [])
    { }

}

