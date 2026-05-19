// PHASE 2: Scheduler.Application/Dtos/ReportDocumentDto.cs
namespace Scheduler.Application.Dtos;

/// <summary>Represents a generated report document.</summary>
public class ReportDocumentDto
{
    /// <summary>File name of the report.</summary>
    public string FileName { get; set; }

    /// <summary>MIME content type.</summary>
    public string ContentType { get; set; }

    /// <summary>Binary data for the report.</summary>
    public byte[] Content { get; set; }

    /// <summary>Initializes a new instance of the <see cref="ReportDocumentDto"/> class.</summary>
    public ReportDocumentDto()
    {
        FileName = string.Empty;
        ContentType = string.Empty;
        Content = System.Array.Empty<byte>();
    }

    /// <summary>Initializes a new instance of the <see cref="ReportDocumentDto"/> class with required fields.</summary>
    public ReportDocumentDto(string fileName, string contentType, byte[] content)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
    }
}
