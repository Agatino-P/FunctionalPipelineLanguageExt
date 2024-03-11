namespace FuncPipeline.Domain;

public record PipelineOutput(bool IsValid, string Sentence, List<string> Errors)
{ }

