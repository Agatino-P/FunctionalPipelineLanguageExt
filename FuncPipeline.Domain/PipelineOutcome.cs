namespace FuncPipeline.Domain;

public record PipelineOutcome(bool IsValid, string Sentence, List<string> Errors)
{ }

