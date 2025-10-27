# coverage-check.ps1
param(
    [int]$MinimumCoverage = 80
)

Write-Host "`n🧪 Running pre-commit coverage check..." -ForegroundColor Cyan
Write-Host "Required minimum coverage: $MinimumCoverage%" -ForegroundColor Yellow

# Navigate to test project and run tests
Set-Location "sg.com.shares.visionapi.Tests"

Write-Host "`nExecuting unit tests..." -ForegroundColor Gray
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults --verbosity quiet

# Check if tests passed
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ UNIT TESTS FAILED!" -ForegroundColor Red
    Write-Host "Fix failing tests before committing." -ForegroundColor Yellow
    Set-Location ".."
    exit 1
}

Write-Host "✅ All tests passed!" -ForegroundColor Green

# Find the most recent coverage file
$coverageFile = Get-ChildItem -Path .\TestResults -Recurse -Filter "coverage.cobertura.xml" | 
                Sort-Object LastWriteTime -Descending | 
                Select-Object -First 1

if ($coverageFile -eq $null) {
    Write-Host "❌ Coverage file not found!" -ForegroundColor Red
    Set-Location ".."
    exit 1
}

# Parse coverage percentage
[xml]$coverageXml = Get-Content $coverageFile.FullName
$lineRate = [math]::Round([double]$coverageXml.coverage.'line-rate' * 100, 2)

# Return to solution root
Set-Location ".."

# Display coverage results
Write-Host "`n📊 Coverage Analysis:" -ForegroundColor Cyan
Write-Host "   Current Coverage: $lineRate%" -ForegroundColor $(if ($lineRate -ge $MinimumCoverage) { "Green" } else { "Red" })
Write-Host "   Required Coverage: $MinimumCoverage%" -ForegroundColor Yellow

# Check if coverage meets minimum threshold
if ($lineRate -lt $MinimumCoverage) {
    Write-Host "`n❌ COMMIT BLOCKED!" -ForegroundColor Red
    Write-Host "   Coverage is below the required $MinimumCoverage% threshold" -ForegroundColor Red
    Write-Host "   Current: $lineRate% | Required: $MinimumCoverage%" -ForegroundColor Red
    Write-Host "`n💡 To fix this:" -ForegroundColor Yellow
    Write-Host "   1. Add more unit tests to increase coverage" -ForegroundColor Gray
    Write-Host "   2. Run: .\sg.com.shares.visionapi.Tests\RunTestsWithCoverage.ps1 -AllTests" -ForegroundColor Gray
    Write-Host "   3. View detailed report at: .\sg.com.shares.visionapi.Tests\coverage-report\index.html" -ForegroundColor Gray
    Write-Host "`n   Or use: git commit --no-verify (not recommended)" -ForegroundColor DarkGray
    exit 1
} else {
    Write-Host "`n✅ COVERAGE CHECK PASSED!" -ForegroundColor Green
    Write-Host "   Coverage $lineRate% meets the $MinimumCoverage% requirement" -ForegroundColor Green
    exit 0
}
