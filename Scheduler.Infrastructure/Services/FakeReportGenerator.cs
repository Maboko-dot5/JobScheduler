// PHASE 7: Scheduler.Infrastructure/Services/FakeReportGenerator.cs
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>Provides fake PDF report generation.</summary>
public class FakeReportGenerator : IReportGenerator
{
    /// <inheritdoc />
    public Task<ReportDocumentDto> GenerateAsync(PdfReportRequestDto request, CancellationToken cancellationToken)
    {
        var pdfContent = CreatePdf(BuildReportLines(request));
        return Task.FromResult(new ReportDocumentDto("report.pdf", "application/pdf", pdfContent));
    }

    private static IReadOnlyList<string> BuildReportLines(PdfReportRequestDto request)
    {
        var lines = new List<string>
        {
            "Job Scheduler Report",
            $"Series: {request.SeriesId}",
            $"Plant: {request.PlantId}",
            $"Variables: {(request.Variables.Count == 0 ? "<none>" : string.Join(", ", request.Variables))}"
        };

        if (request.TimeRange is not null)
        {
            lines.Add($"Start: {request.TimeRange.StartUtc:O}");
            lines.Add($"End: {request.TimeRange.EndUtc:O}");
        }

        if (request.Statistics is not null)
        {
            lines.Add($"Statistics Series: {request.Statistics.SeriesId}");
            lines.Add($"Statistics Metrics: {request.Statistics.Metrics.Count}");
            foreach (var metric in request.Statistics.Metrics)
            {
                lines.Add($"{metric.Key}: {metric.Value:0.###}");
            }
        }

        return lines;
    }

    private static byte[] CreatePdf(IReadOnlyList<string> lines)
    {
        var contentStream = BuildContentStream(lines);
        var contentBytes = Encoding.ASCII.GetBytes(contentStream);

        var pdf = new StringBuilder();
        var offsets = new List<int> { 0 };

        pdf.AppendLine("%PDF-1.4");
        pdf.AppendLine("% Job Scheduler report");

        AppendObject(pdf, offsets, "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        AppendObject(pdf, offsets, "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
        AppendObject(
            pdf,
            offsets,
            "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>\nendobj\n");
        AppendObject(pdf, offsets, "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");
        AppendObject(
            pdf,
            offsets,
            $"5 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n{contentStream}endstream\nendobj\n");

        var xrefStart = pdf.Length;
        pdf.AppendLine("xref");
        pdf.AppendLine("0 6");
        pdf.AppendLine("0000000000 65535 f ");

        for (var index = 1; index < offsets.Count; index++)
        {
            pdf.AppendLine($"{offsets[index]:0000000000} 00000 n ");
        }

        pdf.AppendLine("trailer");
        pdf.AppendLine("<< /Size 6 /Root 1 0 R >>");
        pdf.AppendLine("startxref");
        pdf.AppendLine(xrefStart.ToString());
        pdf.AppendLine("%%EOF");

        return Encoding.ASCII.GetBytes(pdf.ToString());
    }

    private static void AppendObject(StringBuilder pdf, List<int> offsets, string objectText)
    {
        offsets.Add(pdf.Length);
        pdf.Append(objectText);
    }

    private static string BuildContentStream(IReadOnlyList<string> lines)
    {
        var stream = new StringBuilder();
        stream.AppendLine("BT");
        stream.AppendLine("/F1 12 Tf");
        stream.AppendLine("72 740 Td");

        for (var index = 0; index < lines.Count; index++)
        {
            if (index > 0)
            {
                stream.AppendLine("0 -16 Td");
            }

            stream.Append('(');
            stream.Append(EscapePdfText(lines[index]));
            stream.AppendLine(") Tj");
        }

        stream.AppendLine("ET");
        return stream.ToString();
    }

    private static string EscapePdfText(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);
    }
}
