Add-Type -AssemblyName System.Drawing

$width = 1800
$height = 1150
$outputPath = Join-Path $PSScriptRoot "job-scheduler-architecture.png"

$bitmap = New-Object System.Drawing.Bitmap $width, $height
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
$graphics.Clear([System.Drawing.ColorTranslator]::FromHtml("#f8fbff"))

$dark = [System.Drawing.ColorTranslator]::FromHtml("#10243d")
$muted = [System.Drawing.ColorTranslator]::FromHtml("#5e6b7c")
$border = [System.Drawing.ColorTranslator]::FromHtml("#8aa0b8")
$titleFont = New-Object System.Drawing.Font "Segoe UI", 26, ([System.Drawing.FontStyle]::Bold)
$subtitleFont = New-Object System.Drawing.Font "Segoe UI", 14, ([System.Drawing.FontStyle]::Regular)
$clusterFont = New-Object System.Drawing.Font "Segoe UI", 16, ([System.Drawing.FontStyle]::Regular)
$cardTitleFont = New-Object System.Drawing.Font "Segoe UI", 12.5, ([System.Drawing.FontStyle]::Bold)
$cardFont = New-Object System.Drawing.Font "Segoe UI", 11.5, ([System.Drawing.FontStyle]::Regular)
$labelFont = New-Object System.Drawing.Font "Segoe UI", 10, ([System.Drawing.FontStyle]::Regular)

function New-RoundedRectanglePath($x, $y, $w, $h, $r) {
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $r * 2
    $path.AddArc($x, $y, $d, $d, 180, 90)
    $path.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
    $path.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
    $path.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    return $path
}

function Draw-RoundedBox($x, $y, $w, $h, $fillColor, $strokeColor, $strokeWidth) {
    $path = New-RoundedRectanglePath $x $y $w $h 14
    $brush = New-Object System.Drawing.SolidBrush $fillColor
    $pen = New-Object System.Drawing.Pen $strokeColor, $strokeWidth
    $graphics.FillPath($brush, $path)
    $graphics.DrawPath($pen, $path)
    $brush.Dispose()
    $pen.Dispose()
    $path.Dispose()
}

function Draw-Card($x, $y, $w, $h, $fillHex, $title, [string[]]$lines) {
    Draw-RoundedBox $x $y $w $h ([System.Drawing.ColorTranslator]::FromHtml($fillHex)) $dark 2.2
    $titleBrush = New-Object System.Drawing.SolidBrush $dark
    $textBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml("#1d2430"))
    $fmt = New-Object System.Drawing.StringFormat
    $fmt.Alignment = [System.Drawing.StringAlignment]::Center
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Near
    $titleRect = New-Object System.Drawing.RectangleF ($x + 8), ($y + 14), ($w - 16), 24
    $graphics.DrawString($title, $cardTitleFont, $titleBrush, $titleRect, $fmt)
    $bodyRect = New-Object System.Drawing.RectangleF ($x + 12), ($y + 43), ($w - 24), ($h - 52)
    $graphics.DrawString(($lines -join "`n"), $cardFont, $textBrush, $bodyRect, $fmt)
    $fmt.Dispose()
    $titleBrush.Dispose()
    $textBrush.Dispose()
}

function Draw-CenteredText($text, $font, $color, $x, $y, $w, $h) {
    $brush = New-Object System.Drawing.SolidBrush $color
    $fmt = New-Object System.Drawing.StringFormat
    $fmt.Alignment = [System.Drawing.StringAlignment]::Center
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
    $rect = New-Object System.Drawing.RectangleF $x, $y, $w, $h
    $graphics.DrawString($text, $font, $brush, $rect, $fmt)
    $fmt.Dispose()
    $brush.Dispose()
}

function Draw-ArrowLine([System.Drawing.PointF[]]$points, $label = "", $labelX = 0, $labelY = 0) {
    $pen = New-Object System.Drawing.Pen $dark, 2.2
    $cap = New-Object System.Drawing.Drawing2D.AdjustableArrowCap 5, 6
    $pen.CustomEndCap = $cap
    if ($points.Count -eq 2) {
        $graphics.DrawLine($pen, $points[0], $points[1])
    }
    else {
        $graphics.DrawLines($pen, $points)
    }
    $pen.Dispose()
    $cap.Dispose()

    if (-not [string]::IsNullOrWhiteSpace($label)) {
        $brush = New-Object System.Drawing.SolidBrush $dark
        $rect = New-Object System.Drawing.RectangleF ($labelX - 60), ($labelY - 10), 120, 22
        $fmt = New-Object System.Drawing.StringFormat
        $fmt.Alignment = [System.Drawing.StringAlignment]::Center
        $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
        $graphics.DrawString($label, $labelFont, $brush, $rect, $fmt)
        $fmt.Dispose()
        $brush.Dispose()
    }
}

function Point($x, $y) {
    return New-Object System.Drawing.PointF $x, $y
}

# Header
Draw-CenteredText "Implemented Architecture for Question 2: Local C#/.NET 10 JobScheduler" $titleFont $dark 0 28 $width 44
Draw-CenteredText "Self-contained local implementation with production-style interfaces for future database, SMTP, and durable scheduling integrations." $subtitleFont $muted 0 78 $width 28

# Execution boundary
Draw-RoundedBox 35 130 1730 980 ([System.Drawing.ColorTranslator]::FromHtml("#f8fbff")) $border 2.2
Draw-CenteredText "Local Windows / .NET 10 Execution Boundary" $clusterFont $dark 0 140 $width 28

# Top workflow
Draw-Card 80 210 230 120 "#e5f3ff" "Console Client" @("Scheduler.Client", "submit jobs", "poll status")
Draw-Card 385 210 250 120 "#d8f7df" "Scheduler API" @("ASP.NET Core controllers", "POST /api/jobs", "GET /api/jobs/{id}")
Draw-Card 695 210 230 120 "#e3f8dc" "Job Orchestrator" @("create job record", "update status", "enqueue work")
Draw-Card 985 210 260 120 "#fff2c7" "Store + Queue" @("InMemoryJobRepository", "InMemoryJobStatusRepository", "InMemoryJobQueue")
Draw-Card 1320 210 260 120 "#efeaff" "Background Worker" @("BackgroundService", "dequeue jobs", "execute tasks")

# Middle dispatcher and handlers
Draw-Card 780 420 300 120 "#ddfbe9" "Task Dispatcher" @("TaskHandlerRegistry", "TaskType -> ITaskHandler")
Draw-Card 210 610 270 120 "#ffdede" "Physical Filter" @("min/max limits", "removes impossible values")
Draw-Card 535 610 270 120 "#ffdede" "Anomaly Filter" @("z-score strategy", "removes outliers")
Draw-Card 860 610 270 120 "#ffdede" "Statistics" @("shift / daily / weekly", "count, min, max, avg", "uptime and target range")
Draw-Card 1185 610 270 120 "#e4f1ff" "PDF Report" @("FakeReportGenerator", "creates minimal", "valid PDF")

# Supporting services
Draw-Card 245 820 380 120 "#ffe8e8" "Mock Data + Static Rules" @("MockTimeSeriesRepository", "StaticFilteringRuleProvider", "TSDB/rules adapter later")
Draw-Card 785 825 270 120 "#e4f1ff" "Report Store" @("InMemoryReportStore", "GET /api/jobs/{id}", "/report/download")
Draw-Card 1115 825 250 120 "#e4f1ff" "Email Outbox" @("InMemoryEmailOutbox", "queued deliveries")
Draw-Card 1115 965 250 125 "#e4f1ff" "Email Worker" @("BackgroundService", "dequeues email")
Draw-Card 1440 965 260 125 "#e4f1ff" "Email Service" @("ConsoleEmailService", "logs send attempts", "no SMTP setup")
Draw-Card 80 940 250 120 "#edf0f5" "Tests + CI/CD" @("xUnit tests", "integration tests", "GitHub Actions")

# Main arrows
Draw-ArrowLine @((Point 310 270), (Point 385 270)) "HTTP" 348 250
Draw-ArrowLine @((Point 635 270), (Point 695 270)) "validate" 665 250
Draw-ArrowLine @((Point 925 270), (Point 985 270)) "persist" 955 250
Draw-ArrowLine @((Point 1245 270), (Point 1320 270)) "dequeue" 1282 250
Draw-ArrowLine @((Point 1450 330), (Point 1450 380), (Point 930 380), (Point 930 420)) "execute" 1190 360
Draw-ArrowLine @((Point 1320 315), (Point 1245 315)) "status" 1282 338

# Dispatch arrows
Draw-ArrowLine @((Point 780 500), (Point 345 610)) "PhysicalFilter" 548 555
Draw-ArrowLine @((Point 870 540), (Point 670 610)) "AnomalyFilter" 768 575
Draw-ArrowLine @((Point 995 540), (Point 995 610)) "Statistics" 1036 575
Draw-ArrowLine @((Point 1080 500), (Point 1320 610)) "PdfReport" 1200 555

# Shared data/rules dependency
$dashPen = New-Object System.Drawing.Pen $dark, 2
$dashPen.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash
$graphics.DrawLine($dashPen, (Point 435 820), (Point 345 730))
$graphics.DrawLine($dashPen, (Point 435 820), (Point 670 730))
$graphics.DrawLine($dashPen, (Point 435 820), (Point 995 730))
$dashPen.Dispose()
Draw-CenteredText "read data + rules" $labelFont $dark 470 760 150 25

# Report / email arrows
Draw-ArrowLine @((Point 1320 730), (Point 920 825)) "save PDF" 1115 780
Draw-ArrowLine @((Point 1320 730), (Point 1240 825)) "queue email" 1270 780
Draw-ArrowLine @((Point 1240 945), (Point 1240 965)) "" 1240 955
Draw-ArrowLine @((Point 1365 1027), (Point 1440 1027)) "send/log" 1402 1008

$bitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$bitmap.Dispose()

Write-Output $outputPath
