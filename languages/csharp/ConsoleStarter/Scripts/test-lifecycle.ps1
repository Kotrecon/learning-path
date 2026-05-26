# Scripts/test-lifecycle.ps1
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$auditFile = "phase1/module2/audit/integration-test.txt"

Write-Host "Start: Integration test..."
dotnet run --project src/ConsoleStarter > $auditFile 2>&1

$created = (Select-String -Path $auditFile -Pattern "Создан:").Count
$disposed = (Select-String -Path $auditFile -Pattern "Освобождён:").Count

Write-Host "Result: Created=$created, Disposed=$disposed"
if ($created -eq $disposed -and $created -ge 5) {
    Write-Host "PASS: Test OK" -ForegroundColor Green
    exit 0
} else {
    Write-Host "FAIL: Count mismatch" -ForegroundColor Red
    exit 1
}