param(
    [string]$OutputFileName = "JobScheduler-System-Design.docx"
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.IO.Compression.FileSystem

$docsRoot = (Resolve-Path $PSScriptRoot).Path
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$diagramPath = Join-Path $docsRoot "job-scheduler-architecture.png"
$outputPath = Join-Path $docsRoot $OutputFileName
$buildRoot = Join-Path $docsRoot ".docx-build"

if (-not (Test-Path $diagramPath)) {
    throw "Diagram not found: $diagramPath"
}

function Assert-InDocsRoot($path) {
    $fullPath = [System.IO.Path]::GetFullPath($path)
    if (-not $fullPath.StartsWith($docsRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to write outside docs directory: $fullPath"
    }
}

Assert-InDocsRoot $buildRoot
Assert-InDocsRoot $outputPath

if (Test-Path $buildRoot) {
    Remove-Item -LiteralPath $buildRoot -Recurse -Force
}

if (Test-Path $outputPath) {
    Remove-Item -LiteralPath $outputPath -Force
}

New-Item -ItemType Directory -Force -Path $buildRoot | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $buildRoot "_rels") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $buildRoot "docProps") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $buildRoot "word") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $buildRoot "word\_rels") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $buildRoot "word\media") | Out-Null

$utf8NoBom = New-Object System.Text.UTF8Encoding $false

function Write-XmlFile($relativePath, $content) {
    $path = Join-Path $buildRoot $relativePath
    [System.IO.File]::WriteAllText($path, $content, $utf8NoBom)
}

function Escape-Xml($value) {
    if ($null -eq $value) {
        return ""
    }

    return [System.Security.SecurityElement]::Escape([string]$value)
}

function New-Run($text, $bold = $false, $size = 22, $font = "Aptos") {
    $escaped = Escape-Xml $text
    $boldXml = if ($bold) { "<w:b/>" } else { "" }
    return "<w:r><w:rPr>$boldXml<w:rFonts w:ascii=`"$font`" w:hAnsi=`"$font`"/><w:sz w:val=`"$size`"/></w:rPr><w:t xml:space=`"preserve`">$escaped</w:t></w:r>"
}

function New-Paragraph($text, $style = "Body", $bold = $false) {
    $size = switch ($style) {
        "Title" { 36 }
        "Subtitle" { 24 }
        "Heading1" { 28 }
        "Heading2" { 24 }
        default { 22 }
    }

    $spacing = switch ($style) {
        "Title" { "<w:spacing w:before=`"0`" w:after=`"120`"/>" }
        "Subtitle" { "<w:spacing w:before=`"0`" w:after=`"260`"/>" }
        "Heading1" { "<w:spacing w:before=`"280`" w:after=`"120`"/>" }
        "Heading2" { "<w:spacing w:before=`"180`" w:after=`"80`"/>" }
        default { "<w:spacing w:before=`"0`" w:after=`"100`"/>" }
    }

    return "<w:p><w:pPr>$spacing</w:pPr>$(New-Run $text ($bold -or $style -in @("Title", "Heading1", "Heading2")) $size)</w:p>"
}

function New-Bullet($text) {
    return "<w:p><w:pPr><w:spacing w:after=`"70`"/><w:ind w:left=`"360`" w:hanging=`"180`"/></w:pPr>$(New-Run "- $text" $false 22)</w:p>"
}

function New-Numbered($number, $text) {
    return "<w:p><w:pPr><w:spacing w:after=`"70`"/><w:ind w:left=`"360`" w:hanging=`"260`"/></w:pPr>$(New-Run "$number. $text" $false 22)</w:p>"
}

function New-Cell($text, $width, $isHeader = $false) {
    $shading = if ($isHeader) { "<w:shd w:fill=`"D9EAF7`"/>" } else { "" }
    $bold = $isHeader
    $lines = ([string]$text) -split "`n"
    $paragraphs = foreach ($line in $lines) {
        "<w:p><w:pPr><w:spacing w:after=`"40`"/></w:pPr>$(New-Run $line $bold 18)</w:p>"
    }

    $joinedParagraphs = $paragraphs -join ''
    return "<w:tc><w:tcPr><w:tcW w:w=`"$width`" w:type=`"dxa`"/>$shading</w:tcPr>$joinedParagraphs</w:tc>"
}

function New-Table($headers, $rows, $widths) {
    $borderXml = @"
<w:tblPr>
  <w:tblW w:w="0" w:type="auto"/>
  <w:tblBorders>
    <w:top w:val="single" w:sz="6" w:space="0" w:color="7A8EA5"/>
    <w:left w:val="single" w:sz="6" w:space="0" w:color="7A8EA5"/>
    <w:bottom w:val="single" w:sz="6" w:space="0" w:color="7A8EA5"/>
    <w:right w:val="single" w:sz="6" w:space="0" w:color="7A8EA5"/>
    <w:insideH w:val="single" w:sz="4" w:space="0" w:color="B7C4D2"/>
    <w:insideV w:val="single" w:sz="4" w:space="0" w:color="B7C4D2"/>
  </w:tblBorders>
</w:tblPr>
"@

    $headerCells = for ($i = 0; $i -lt $headers.Count; $i++) {
        New-Cell $headers[$i] $widths[$i] $true
    }

    $joinedHeaderCells = $headerCells -join ''
    $rowXml = @("<w:tr>$joinedHeaderCells</w:tr>")

    foreach ($row in $rows) {
        $cells = for ($i = 0; $i -lt $row.Count; $i++) {
            New-Cell $row[$i] $widths[$i] $false
        }

        $joinedCells = $cells -join ''
        $rowXml += "<w:tr>$joinedCells</w:tr>"
    }

    $joinedRows = $rowXml -join ''
    return "<w:tbl>$borderXml$joinedRows</w:tbl>"
}

function New-ImageParagraph($relationshipId, $cx, $cy) {
    return @"
<w:p>
  <w:pPr><w:jc w:val="center"/><w:spacing w:after="160"/></w:pPr>
  <w:r>
    <w:drawing>
      <wp:inline distT="0" distB="0" distL="0" distR="0">
        <wp:extent cx="$cx" cy="$cy"/>
        <wp:effectExtent l="0" t="0" r="0" b="0"/>
        <wp:docPr id="1" name="JobScheduler Architecture Diagram"/>
        <wp:cNvGraphicFramePr>
          <a:graphicFrameLocks noChangeAspect="1"/>
        </wp:cNvGraphicFramePr>
        <a:graphic>
          <a:graphicData uri="http://schemas.openxmlformats.org/drawingml/2006/picture">
            <pic:pic>
              <pic:nvPicPr>
                <pic:cNvPr id="1" name="job-scheduler-architecture.png"/>
                <pic:cNvPicPr/>
              </pic:nvPicPr>
              <pic:blipFill>
                <a:blip r:embed="$relationshipId"/>
                <a:stretch><a:fillRect/></a:stretch>
              </pic:blipFill>
              <pic:spPr>
                <a:xfrm>
                  <a:off x="0" y="0"/>
                  <a:ext cx="$cx" cy="$cy"/>
                </a:xfrm>
                <a:prstGeom prst="rect"><a:avLst/></a:prstGeom>
              </pic:spPr>
            </pic:pic>
          </a:graphicData>
        </a:graphic>
      </wp:inline>
    </w:drawing>
  </w:r>
</w:p>
"@
}

$componentRows = @(
    @("Console Client", "Submits jobs and polls status.", "Scheduler.Client sends HTTP requests to the API and displays job results.", "Demonstrates network-based job arrival from a client application."),
    @("Scheduler API", "Receives, validates, accepts, lists, and queries jobs.", "Scheduler.Api exposes the job, health, and report download endpoints.", "Keeps the network boundary separate from scheduling and processing."),
    @("Job Orchestrator", "Coordinates job creation and retrieval.", "Creates job records, updates status, and queues work.", "Centralizes application workflow without placing business logic in controllers."),
    @("Store + Queue", "Stores jobs, statuses, and queued work.", "Uses in-memory repositories and queue implementations.", "Keeps the test self-contained while preserving replaceable interfaces."),
    @("Background Worker", "Processes queued jobs asynchronously.", "Dequeues jobs, resolves handlers, runs tasks, and records results.", "Prevents API calls from blocking while jobs execute."),
    @("Task Dispatcher", "Routes each task to the correct handler.", "TaskHandlerRegistry maps TaskType values to ITaskHandler implementations.", "New task types can be added without changing the scheduler core."),
    @("Mock Data + Static Rules", "Provides generated historical data and filtering rules.", "MockTimeSeriesRepository and StaticFilteringRuleProvider stand in for external services.", "Avoids external database setup while keeping realistic abstractions."),
    @("Physical Filter", "Removes physically impossible values.", "Applies static min/max limits to time-series points.", "Demonstrates variable-specific physical filtering."),
    @("Anomaly Filter", "Removes outliers.", "Uses a z-score based anomaly filter.", "Provides a simple, explainable anomaly strategy."),
    @("Statistics", "Calculates shift, daily, and weekly metrics.", "Calculates count, min, max, average, uptime, and target-range percentage.", "Demonstrates the required statistics task without real plant data."),
    @("PDF Report", "Generates report output.", "FakeReportGenerator creates a minimal valid PDF document.", "Avoids PDF library setup while still producing downloadable PDF output."),
    @("Report Store", "Stores generated reports.", "InMemoryReportStore stores PDF bytes by job ID.", "Supports GET /api/jobs/{id}/report/download."),
    @("Email Outbox + Worker", "Queues and processes report emails.", "InMemoryEmailOutbox and EmailOutboxWorker handle asynchronous delivery requests.", "Separates report generation from email delivery."),
    @("Email Service", "Simulates email delivery.", "ConsoleEmailService logs send attempts and attachment presence.", "Avoids SMTP credentials while preserving IEmailService."),
    @("Tests + CI/CD", "Verifies behavior automatically.", "xUnit tests and GitHub Actions restore, build, test, and publish.", "Shows development process discipline.")
)

$apiRows = @(
    @("POST /api/jobs", "Submits a new job and returns 202 Accepted with jobId, status, and statusUrl."),
    @("GET /api/jobs", "Lists current jobs and their statuses."),
    @("GET /api/jobs/{id}", "Returns status and result details for a job."),
    @("POST /api/jobs/{id}/cancel", "Returns a cancellation acknowledgement."),
    @("GET /api/jobs/{id}/report/download", "Downloads the generated PDF report for a report job."),
    @("GET /health", "Confirms API health.")
)

$assumptionRows = @(
    @("Jobs are submitted over HTTP to a local ASP.NET Core API.", "This satisfies the requirement that jobs arrive over the network from a client application."),
    @("Persistence is in-memory.", "The solution is self-contained for assessment; repository interfaces allow durable storage later."),
    @("The time-series database is simulated.", "No external TSDB setup is required; MockTimeSeriesRepository acts as a future adapter seam."),
    @("Physical filtering rules are static.", "This demonstrates per-variable limits without requiring a rules database."),
    @("Anomaly detection uses z-score filtering.", "It is simple, explainable, and suitable for a prototype."),
    @("PDF generation uses an internal minimal PDF generator.", "This avoids extra package setup while producing a real downloadable PDF file."),
    @("Email delivery is simulated through logging.", "No SMTP credentials are required, but the IEmailService contract can be replaced with SMTP later."),
    @("Recurring report schedules are represented architecturally, not fully implemented.", "The current implementation supports queued report jobs and email outbox processing; cron/calendar scheduling is a future extension."),
    @("CI/CD uses GitHub Actions.", "This matches the repository submission process and demonstrates automated restore/build/test.")
)

$jobFlowRows = @(
    @("1", "Client submits a job over HTTP to POST /api/jobs.", "Client: Scheduler.Client/Program.cs`nAPI entry: Scheduler.Api/Controllers/JobsController.cs`nOrchestrator interface: Scheduler.Application/Orchestration/IJobOrchestrator.cs"),
    @("2", "API validates the incoming JobSubmissionRequestDto.", "Validator: Scheduler.Application/Validation/JobRequestValidator.cs`nDTOs: Scheduler.Application/Dtos/JobSubmissionRequestDto.cs and JobTaskDto.cs"),
    @("3", "JobOrchestrator creates the Job entity, copies variables and task definitions, and marks the job Queued.", "Interface: Scheduler.Application/Orchestration/IJobOrchestrator.cs`nImplementation: Scheduler.Application/Orchestration/JobOrchestrator.cs`nDomain model: Scheduler.Domain/Entities/Job.cs and JobTask.cs"),
    @("4", "Job metadata and current status are persisted in memory.", "Interfaces: Scheduler.Application/Interfaces/Repositories/IJobRepository.cs and IJobStatusRepository.cs`nImplementations: Scheduler.Infrastructure/Repositories/InMemoryJobRepository.cs and InMemoryJobStatusRepository.cs"),
    @("5", "The job is placed onto the background queue so the HTTP request can return quickly.", "Queue interface: Scheduler.Application/Interfaces/Queue/IJobQueue.cs`nImplementation: Scheduler.Application/Queue/InMemoryJobQueue.cs"),
    @("6", "API returns 202 Accepted with jobId, status, and statusUrl.", "API entry: Scheduler.Api/Controllers/JobsController.cs`nResponse DTO: Scheduler.Application/Dtos/JobSubmissionResponseDto.cs"),
    @("7", "BackgroundJobWorker dequeues the job, marks it Running, and prepares each task context.", "Worker: Scheduler.Application/Background/BackgroundJobWorker.cs`nQueue interface: Scheduler.Application/Interfaces/Queue/IJobQueue.cs`nExecution DTO: Scheduler.Application/Dtos/JobContextDto.cs"),
    @("8", "TaskHandlerRegistry resolves the correct handler for the task type.", "Registry interface: Scheduler.Application/Interfaces/Handlers/ITaskHandlerRegistry.cs`nHandler interface: Scheduler.Application/Interfaces/Handlers/ITaskHandler.cs`nRegistry implementation: Scheduler.Application/Handlers/TaskHandlerRegistry.cs"),
    @("9", "Physical filtering reads generated historical data, gets variable min/max rules, and removes impossible values.", "Handler: Scheduler.Application/Handlers/PhysicalFilterHandler.cs`nData interface: Scheduler.Application/Interfaces/Repositories/ITimeSeriesRepository.cs`nRules interface: Scheduler.Application/Interfaces/Services/IFilteringRuleProvider.cs`nService interface: Scheduler.Application/Interfaces/Services/IPhysicalFilterService.cs"),
    @("10", "Anomaly filtering reads generated historical data and applies the anomaly strategy.", "Handler: Scheduler.Application/Handlers/AnomalyFilterHandler.cs`nData interface: Scheduler.Application/Interfaces/Repositories/ITimeSeriesRepository.cs`nService interface: Scheduler.Application/Interfaces/Services/IAnomalyFilterService.cs`nImplementation: Scheduler.Infrastructure/Services/FakeAnomalyFilterService.cs"),
    @("11", "Statistics tasks calculate the requested Shift, Daily, or Weekly window and return readable numeric summaries in the job payload.", "Handler: Scheduler.Application/Handlers/StatisticsHandler.cs`nStatistics interface: Scheduler.Application/Interfaces/Services/IStatisticsService.cs`nImplementation: Scheduler.Infrastructure/Services/FakeStatisticsService.cs`nResult DTO: Scheduler.Application/Dtos/StatisticsResultDto.cs"),
    @("12", "PDF report jobs read generated time-series data for the submitted plant and variables, calculate Shift, Daily, and Weekly summaries, then pass those summaries into the PDF request.", "Handler: Scheduler.Application/Handlers/PdfReportHandler.cs`nData interface: Scheduler.Application/Interfaces/Repositories/ITimeSeriesRepository.cs`nStatistics interface: Scheduler.Application/Interfaces/Services/IStatisticsService.cs`nReport DTOs: Scheduler.Application/Dtos/PdfReportRequestDto.cs and ReportStatisticsSummaryDto.cs"),
    @("13", "FakeReportGenerator creates a valid PDF containing the submitted variables and the three statistics summaries.", "Report interface: Scheduler.Application/Interfaces/Services/IReportGenerator.cs`nImplementation: Scheduler.Infrastructure/Services/FakeReportGenerator.cs`nDocument DTO: Scheduler.Application/Dtos/ReportDocumentDto.cs"),
    @("14", "The generated report is stored by job ID for later download.", "Store interface: Scheduler.Application/Interfaces/Services/IReportStore.cs`nImplementation: Scheduler.Infrastructure/Services/InMemoryReportStore.cs`nDownload endpoint: Scheduler.Api/Controllers/JobsController.cs"),
    @("15", "The report email is queued, then the email worker logs the send attempt through the current email service.", "Outbox interface: Scheduler.Application/Interfaces/Services/IEmailOutbox.cs`nEmail interface: Scheduler.Application/Interfaces/Services/IEmailService.cs`nWorker: Scheduler.Application/Background/EmailOutboxWorker.cs`nImplementations: Scheduler.Infrastructure/Services/InMemoryEmailOutbox.cs and ConsoleEmailService.cs"),
    @("16", "The worker writes task summaries, sets the job Completed or Failed, and the client can poll status or download the PDF.", "Worker: Scheduler.Application/Background/BackgroundJobWorker.cs`nStatus DTO: Scheduler.Application/Dtos/JobStatusResponseDto.cs`nResult DTO: Scheduler.Application/Dtos/JobResultDto.cs`nDownload endpoint: GET /api/jobs/{id}/report/download")
)

$interfaceRows = @(
    @("Job submission and status", "Scheduler.Application/Orchestration/IJobOrchestrator.cs", "Scheduler.Application/Orchestration/JobOrchestrator.cs"),
    @("Job persistence", "Scheduler.Application/Interfaces/Repositories/IJobRepository.cs", "Scheduler.Infrastructure/Repositories/InMemoryJobRepository.cs"),
    @("Job status persistence", "Scheduler.Application/Interfaces/Repositories/IJobStatusRepository.cs", "Scheduler.Infrastructure/Repositories/InMemoryJobStatusRepository.cs"),
    @("Background queue", "Scheduler.Application/Interfaces/Queue/IJobQueue.cs", "Scheduler.Application/Queue/InMemoryJobQueue.cs"),
    @("Task dispatch", "Scheduler.Application/Interfaces/Handlers/ITaskHandlerRegistry.cs", "Scheduler.Application/Handlers/TaskHandlerRegistry.cs"),
    @("Task execution contract", "Scheduler.Application/Interfaces/Handlers/ITaskHandler.cs", "Scheduler.Application/Handlers/*Handler.cs"),
    @("Time-series data access", "Scheduler.Application/Interfaces/Repositories/ITimeSeriesRepository.cs", "Scheduler.Infrastructure/Repositories/MockTimeSeriesRepository.cs"),
    @("Physical filtering rules", "Scheduler.Application/Interfaces/Services/IFilteringRuleProvider.cs", "Scheduler.Infrastructure/Services/StaticFilteringRuleProvider.cs"),
    @("Physical filtering service", "Scheduler.Application/Interfaces/Services/IPhysicalFilterService.cs", "Scheduler.Infrastructure/Services/FakePhysicalFilterService.cs"),
    @("Anomaly filtering service", "Scheduler.Application/Interfaces/Services/IAnomalyFilterService.cs", "Scheduler.Infrastructure/Services/FakeAnomalyFilterService.cs"),
    @("Statistics calculation", "Scheduler.Application/Interfaces/Services/IStatisticsService.cs", "Scheduler.Infrastructure/Services/FakeStatisticsService.cs"),
    @("PDF generation", "Scheduler.Application/Interfaces/Services/IReportGenerator.cs", "Scheduler.Infrastructure/Services/FakeReportGenerator.cs"),
    @("Report storage/download", "Scheduler.Application/Interfaces/Services/IReportStore.cs", "Scheduler.Infrastructure/Services/InMemoryReportStore.cs"),
    @("Email outbox", "Scheduler.Application/Interfaces/Services/IEmailOutbox.cs", "Scheduler.Infrastructure/Services/InMemoryEmailOutbox.cs"),
    @("Email delivery", "Scheduler.Application/Interfaces/Services/IEmailService.cs", "Scheduler.Infrastructure/Services/ConsoleEmailService.cs"),
    @("Processing mode configuration", "Scheduler.Application/Interfaces/Configuration/ISchedulerProcessingModeProvider.cs", "Scheduler.Infrastructure/Configuration/SchedulerProcessingModeProvider.cs")
)

$image = [System.Drawing.Image]::FromFile($diagramPath)
$maxWidthInches = 9.6
$widthEmu = [int64]($maxWidthInches * 914400)
$heightEmu = [int64]($widthEmu * ($image.Height / $image.Width))
$image.Dispose()

$body = New-Object System.Collections.Generic.List[string]
$body.Add((New-Paragraph "JobScheduler System Design Architecture" "Title"))
$body.Add((New-Paragraph "Question 2: Local C#/.NET 10 implementation" "Subtitle"))
$body.Add((New-Paragraph "1. Overview" "Heading1"))
$body.Add((New-Paragraph "The implemented JobScheduler application uses a local C#/.NET 10 architecture with an ASP.NET Core API, a console client, background workers, in-memory persistence, task handlers, generated time-series data, generated PDF reports, and console-based email delivery. The design keeps the implementation self-contained for the practical test while preserving production-style interfaces that can later be connected to a real time-series database, durable job store, SMTP provider, and recurring schedule engine."))
$body.Add((New-Paragraph "2. Architecture Diagram" "Heading1"))
$body.Add((New-ImageParagraph "rIdDiagram" $widthEmu $heightEmu))
$body.Add((New-Paragraph "Figure 1: Implemented local .NET 10 JobScheduler architecture." "Body" $true))
$body.Add((New-Paragraph "3. Component Responsibilities" "Heading1"))
$body.Add((New-Table @("Component", "What It Does", "How It Works", "Reason") $componentRows @(2100, 3000, 4300, 4300)))
$body.Add((New-Paragraph "4. Job Lifecycle, Interfaces, and File Navigation" "Heading1"))
$body.Add((New-Table @("Step", "Flow", "Where to Find It") $jobFlowRows @(700, 4300, 8200)))
$body.Add((New-Paragraph "5. Interface Folder Map" "Heading1"))
$body.Add((New-Table @("Concern", "Interface Location", "Current Implementation") $interfaceRows @(3400, 5200, 4600)))
$body.Add((New-Paragraph "6. Implemented API Contract" "Heading1"))
$body.Add((New-Table @("Endpoint", "Purpose") $apiRows @(3600, 9600)))
$body.Add((New-Paragraph "7. Key Interfaces and Extension Points" "Heading1"))
$body.Add((New-Bullet "IJobOrchestrator coordinates job submission and status retrieval."))
$body.Add((New-Bullet "IJobRepository and IJobStatusRepository isolate persistence from application logic."))
$body.Add((New-Bullet "IJobQueue decouples API submission from background execution."))
$body.Add((New-Bullet "ITaskHandler and TaskHandlerRegistry route each TaskType to its implementation."))
$body.Add((New-Bullet "ITimeSeriesRepository abstracts the time-series database."))
$body.Add((New-Bullet "IFilteringRuleProvider abstracts physical limits and anomaly rule lookup."))
$body.Add((New-Bullet "IReportGenerator and IReportStore separate report creation from report retrieval."))
$body.Add((New-Bullet "IEmailOutbox and IEmailService separate report generation from email delivery."))
$body.Add((New-Paragraph "8. Assumptions for This Implementation" "Heading1"))
$body.Add((New-Table @("Assumption", "Reason") $assumptionRows @(5600, 7600)))
$body.Add((New-Paragraph "9. Summary" "Heading1"))
$body.Add((New-Paragraph "The implemented system supports network job submission, background execution, task dispatching, generated historical time-series data, physical filtering, anomaly filtering, statistics, PDF report generation, report download, email outbox processing, status polling, unit tests, integration tests, and CI/CD. External infrastructure such as a real time-series database, SMTP server, durable job store, and recurring schedule engine is intentionally abstracted behind interfaces so the solution remains easy to build and run for assessment."))
$body.Add((New-Paragraph "10. AI Declaration" "Heading1"))
$body.Add((New-Paragraph "AI coding assistance was used during development of this submission. The generated output was reviewed, adapted, tested, and accepted by the author."))

$documentXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document
  xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
  xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
  xmlns:wp="http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"
  xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
  xmlns:pic="http://schemas.openxmlformats.org/drawingml/2006/picture">
  <w:body>
    $($body -join "`n")
    <w:sectPr>
      <w:pgSz w:w="15840" w:h="12240" w:orient="landscape"/>
      <w:pgMar w:top="720" w:right="720" w:bottom="720" w:left="720" w:header="360" w:footer="360" w:gutter="0"/>
    </w:sectPr>
  </w:body>
</w:document>
"@

$contentTypesXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Default Extension="png" ContentType="image/png"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
  <Override PartName="/docProps/core.xml" ContentType="application/vnd.openxmlformats-package.core-properties+xml"/>
  <Override PartName="/docProps/app.xml" ContentType="application/vnd.openxmlformats-officedocument.extended-properties+xml"/>
</Types>
"@

$rootRelsXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties" Target="docProps/core.xml"/>
  <Relationship Id="rId3" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties" Target="docProps/app.xml"/>
</Relationships>
"@

$documentRelsXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rIdDiagram" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/image" Target="media/job-scheduler-architecture.png"/>
</Relationships>
"@

$created = [DateTime]::UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
$coreXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<cp:coreProperties
  xmlns:cp="http://schemas.openxmlformats.org/package/2006/metadata/core-properties"
  xmlns:dc="http://purl.org/dc/elements/1.1/"
  xmlns:dcterms="http://purl.org/dc/terms/"
  xmlns:dcmitype="http://purl.org/dc/dcmitype/"
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <dc:title>JobScheduler System Design Architecture</dc:title>
  <dc:creator>JobScheduler submission</dc:creator>
  <cp:lastModifiedBy>JobScheduler submission</cp:lastModifiedBy>
  <dcterms:created xsi:type="dcterms:W3CDTF">$created</dcterms:created>
  <dcterms:modified xsi:type="dcterms:W3CDTF">$created</dcterms:modified>
</cp:coreProperties>
"@

$appXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/extended-properties"
  xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes">
  <Application>JobScheduler</Application>
  <DocSecurity>0</DocSecurity>
  <ScaleCrop>false</ScaleCrop>
  <Company></Company>
  <LinksUpToDate>false</LinksUpToDate>
  <SharedDoc>false</SharedDoc>
  <HyperlinksChanged>false</HyperlinksChanged>
  <AppVersion>1.0</AppVersion>
</Properties>
"@

Write-XmlFile "[Content_Types].xml" $contentTypesXml
Write-XmlFile "_rels\.rels" $rootRelsXml
Write-XmlFile "docProps\core.xml" $coreXml
Write-XmlFile "docProps\app.xml" $appXml
Write-XmlFile "word\document.xml" $documentXml
Write-XmlFile "word\_rels\document.xml.rels" $documentRelsXml
Copy-Item -LiteralPath $diagramPath -Destination (Join-Path $buildRoot "word\media\job-scheduler-architecture.png")

[System.IO.Compression.ZipFile]::CreateFromDirectory($buildRoot, $outputPath)
Remove-Item -LiteralPath $buildRoot -Recurse -Force

Write-Output $outputPath
