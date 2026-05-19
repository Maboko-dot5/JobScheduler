// PHASE 1: Scheduler.Domain/ValueObjects/JobError.cs
namespace Scheduler.Domain.ValueObjects;

/// <summary>Represents an error produced during job execution.</summary>
public class JobError
{
    /// <summary>Error code.</summary>
    public string Code { get; set; }

    /// <summary>Error message.</summary>
    public string Message { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobError"/> class.</summary>
    public JobError()
    {
        Code = string.Empty;
        Message = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="JobError"/> class with required fields.</summary>
    public JobError(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
