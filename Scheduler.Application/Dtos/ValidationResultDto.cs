// PHASE 6: Scheduler.Application/Dtos/ValidationResultDto.cs
using System.Collections.Generic;

namespace Scheduler.Application.Dtos;

/// <summary>Represents validation results.</summary>
public class ValidationResultDto
{
    /// <summary>Indicates whether the request is valid.</summary>
    public bool IsValid { get; set; }

    /// <summary>Validation error messages.</summary>
    public List<string> Errors { get; set; }

    /// <summary>Initializes a new instance of the <see cref="ValidationResultDto"/> class.</summary>
    public ValidationResultDto()
    {
        Errors = new List<string>();
    }

    /// <summary>Initializes a new instance of the <see cref="ValidationResultDto"/> class with required fields.</summary>
    public ValidationResultDto(bool isValid)
    {
        IsValid = isValid;
        Errors = new List<string>();
    }
}
