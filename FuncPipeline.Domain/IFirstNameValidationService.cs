namespace FuncPipeline.Domain;

public interface IFirstNameValidationService
{
    bool IsValid(string firstName);
}