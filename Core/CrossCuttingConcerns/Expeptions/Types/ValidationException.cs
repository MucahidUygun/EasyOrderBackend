

using Core.Constants;

namespace Core.CrossCuttingConcerns.Expeptions.Types;
public class ValidationException : Exception
{
    public IEnumerable<ValidationExceptionModel> Errors { get; }

    public ValidationException(IEnumerable<ValidationExceptionModel> errors) : base(CoreMessages.ValidationsError)
    {
        Errors = errors;
    }
}

public class ValidationExceptionModel
{
    public string Property { get; set; }
    public string Error { get; set; }

    public ValidationExceptionModel(string property, string error)
    {
        Property = property;
        Error = error;
    }
}