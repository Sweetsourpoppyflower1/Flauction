# update-coverage.ps1

# 1. Run tests and collect coverage data
dotnet test --collect:"XPlat Code Coverage"

# 2. Generate the HTML report
reportgenerator "-reports:Flauction.Tests\TestResults\*\coverage.cobertura.xml" "-targetdir:coveragereport" "-reporttypes:Html"

# 3. Optional: Automatically open the new report
Start-Process "coveragereport\index.html"

Write-Host "Coverage report updated successfully."