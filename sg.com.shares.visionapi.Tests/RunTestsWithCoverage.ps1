# =============================================================
# Code Coverage Script with Flexible Test Execution
# =============================================================

param(
    [string]$TestFilter = "",  # Optional: Filter specific tests
    [switch]$AllTests = $false, # Run all tests (default for CI/CD)
    [switch]$Quick = $false     # Quick mode: skip report generation
)

# Navigate to your test project folder if needed
# cd "D:\Shares.Visionapi\sg.com.shares.visionapi.Tests"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Code Coverage Test Runner" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Determine which tests to run
if ($TestFilter -ne "") {
    Write-Host "Running FILTERED tests: $TestFilter" -ForegroundColor Yellow
    $filterArg = "--filter `"FullyQualifiedName~$TestFilter`""
} elseif ($AllTests) {
    Write-Host "Running ALL tests (Full Suite)" -ForegroundColor Green
    $filterArg = ""
} else {
    Write-Host "Running ALL tests (Default behavior)" -ForegroundColor Green
    $filterArg = ""
}

Write-Host "`nCollecting code coverage...`n" -ForegroundColor Cyan

# Run tests with coverage collection
if ($filterArg -ne "") {
    Invoke-Expression "dotnet test --collect:`"XPlat Code Coverage`" $filterArg"
} else {
    dotnet test --collect:"XPlat Code Coverage"
}

# Check if tests passed
if ($LASTEXITCODE -ne 0) {
    Write-Host "`Tests FAILED! Fix the failing tests first." -ForegroundColor Red
    exit 1
}

Write-Host "`Tests PASSED!" -ForegroundColor Green

# Find the generated coverage file (cobertura format) recursively
Write-Host "`nSearching for coverage file..." -ForegroundColor Cyan
$coverageFile = Get-ChildItem -Path .\TestResults -Recurse -Filter "coverage.cobertura.xml" | 
                Sort-Object LastWriteTime -Descending | 
                Select-Object -First 1

if ($coverageFile -eq $null) {
    Write-Host "Coverage file not found!" -ForegroundColor Red
    exit 1
}

Write-Host "Coverage file found at: $($coverageFile.FullName)" -ForegroundColor Green

# Quick mode: skip report generation
if ($Quick) {
    Write-Host "`Quick mode enabled - skipping HTML report generation" -ForegroundColor Yellow
    Write-Host "Coverage data saved. Run without -Quick flag to generate report.`n" -ForegroundColor Yellow
    exit 0
}

# Define output folder for HTML report
$reportFolder = ".\coverage-report"

# Ensure the report folder exists
if (-Not (Test-Path $reportFolder)) {
    New-Item -ItemType Directory -Path $reportFolder | Out-Null
}

# Generate HTML report
Write-Host "`nGenerating HTML coverage report..." -ForegroundColor Cyan
reportgenerator -reports:$coverageFile.FullName `
                -targetdir:$reportFolder `
                -reporttypes:Html `
                -verbosity:Warning

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to generate report. Is reportgenerator installed?" -ForegroundColor Red
    Write-Host "Install with: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Yellow
    exit 1
}

# Display coverage summary in console
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Coverage Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Parse coverage percentage from the XML file
[xml]$coverageXml = Get-Content $coverageFile.FullName
$lineRate = [math]::Round([double]$coverageXml.coverage.'line-rate' * 100, 2)
$branchRate = [math]::Round([double]$coverageXml.coverage.'branch-rate' * 100, 2)

Write-Host "Line Coverage:   $lineRate%" -ForegroundColor $(if ($lineRate -ge 80) { "Green" } elseif ($lineRate -ge 60) { "Yellow" } else { "Red" })
Write-Host "Branch Coverage: $branchRate%" -ForegroundColor $(if ($branchRate -ge 80) { "Green" } elseif ($branchRate -ge 60) { "Yellow" } else { "Red" })

# Open the HTML report in default browser
$reportPath = Join-Path $reportFolder "index.html"
Write-Host "`HTML Report Generated Successfully!" -ForegroundColor Green
Write-Host "Opening report: $reportPath`n" -ForegroundColor Cyan

Start-Process $reportPath

Write-Host "========================================`n" -ForegroundColor Cyan

# Exit with appropriate code based on coverage threshold
#if ($lineRate -lt 80) {
#    Write-Host "Warning: Line coverage is below 80% threshold" -ForegroundColor Yellow
    # Uncomment to fail builds below threshold:
    # exit 1
#}

# Exit with appropriate code based on coverage threshold
if ($lineRate -lt 80) {
    Write-Host "❌ COVERAGE THRESHOLD NOT MET!" -ForegroundColor Red
    Write-Host "   Required: 80%" -ForegroundColor Red
    Write-Host "   Current:  $lineRate%" -ForegroundColor Red
    Write-Host "   Please add more tests to meet the coverage requirement." -ForegroundColor Yellow
    exit 1  # This will fail the commit - CRITICAL!
} else {
    Write-Host "✅ Coverage threshold met: $lineRate% >= 80%" -ForegroundColor Green
    exit 0  # Success - allow commit
}