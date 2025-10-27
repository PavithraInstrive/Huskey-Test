# coverage-check.ps1
param(
    [int]$MinimumCoverage = 80
)

Write-Host "`n🧪 Running pre-commit coverage check..." -ForegroundColor Cyan
Write-Host "Required minimum coverage: $MinimumCoverage%" -ForegroundColor Yellow

# Run tests with coverage collection
Write-Host "`nExecuting unit tests..." -ForegroundColor Gray
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ TESTS FAILED!" -ForegroundColor Red
    Write-Host "Fix failing tests before committing." -ForegroundColor Yellow
    exit 1
}

# Find the most recent coverage file
$coverageFile = Get-ChildItem -Path .\TestResults -Recurse -Filter "coverage.cobertura.xml" | 
                Sort-Object LastWriteTime -Descending | 
                Select-Object -First 1

if ($coverageFile -eq $null) {
    Write-Host "❌ Coverage file not found!" -ForegroundColor Red
    exit 1
}

# Parse coverage percentage
[xml]$coverageXml = Get-Content $coverageFile.FullName
$lineRate = [math]::Round([double]$coverageXml.coverage.'line-rate' * 100, 2)

Write-Host "Current coverage: $lineRate%" -ForegroundColor $(if ($lineRate -ge $MinimumCoverage) { "Green" } else { "Red" })

# Check if coverage meets minimum threshold
if ($lineRate -lt $MinimumCoverage) {
    Write-Host "`n❌ COMMIT BLOCKED: Coverage below $MinimumCoverage%" -ForegroundColor Red
    Write-Host "   Required: $MinimumCoverage%" -ForegroundColor Red
    Write-Host "   Current:  $lineRate%" -ForegroundColor Red
    Write-Host "`n💡 To fix this:" -ForegroundColor Yellow
    Write-Host "   1. Add more unit tests to increase coverage" -ForegroundColor Gray
    Write-Host "   2. Run: .\RunTestsWithCoverage.ps1 -AllTests" -ForegroundColor Gray
    Write-Host "   3. View detailed report at: .\coverage-report\index.html" -ForegroundColor Gray
    Write-Host "`n   Or use 'git commit --no-verify' to bypass (not recommended)" -ForegroundColor DarkGray
    exit 1
} else {
    Write-Host "✅ Coverage check passed!" -ForegroundColor Green
    exit 0
}
