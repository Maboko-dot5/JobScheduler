// PHASE 2: Scheduler.Application/Dtos/EmailDeliveryDto.cs
namespace Scheduler.Application.Dtos;

/// <summary>Represents an email delivery request.</summary>
public class EmailDeliveryDto
{
    /// <summary>Recipient email address.</summary>
    public string To { get; set; }

    /// <summary>Subject line.</summary>
    public string Subject { get; set; }

    /// <summary>Message body.</summary>
    public string Body { get; set; }

    /// <summary>Optional attachment.</summary>
    public ReportDocumentDto? Attachment { get; set; }

    /// <summary>Initializes a new instance of the <see cref="EmailDeliveryDto"/> class.</summary>
    public EmailDeliveryDto()
    {
        To = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="EmailDeliveryDto"/> class with required fields.</summary>
    public EmailDeliveryDto(string to, string subject, string body)
    {
        To = to;
        Subject = subject;
        Body = body;
    }
}
