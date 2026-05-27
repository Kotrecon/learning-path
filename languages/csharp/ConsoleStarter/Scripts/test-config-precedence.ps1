[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Continue"

$projectDir = Join-Path $PSScriptRoot "..\src\ConsoleStarter"
$passed = 0
$total = 3

function Run-Test {
    param(
        [string]$Name,
        [string]$Expected,
        [string]$EnvVal,
        [string[]]$CliArgs = @()
    )

    if (Test-Path Env:\App__Timeout) { Remove-Item Env:\App__Timeout -ErrorAction SilentlyContinue }
    if ($EnvVal) { $env:App__Timeout = $EnvVal }

    try {
        Push-Location $projectDir
        try {
            $allArgs = @("run", "--no-build") + $CliArgs
            $output = & dotnet $allArgs 2>&1 | Out-String
        } finally {
            Pop-Location
        }

        if ($output -match "Timeout:\s*(\d+)") {
            $val = $Matches[1]
            if ($val -eq $Expected) {
                Write-Host "[PASS] $Name | Expected=$Expected Actual=$val" -ForegroundColor Green
                $script:passed++
            } else {
                Write-Host "[FAIL] $Name | Expected=$Expected Actual=$val" -ForegroundColor Red
            }
        } else {
            Write-Host "[FAIL] $Name | Pattern not found" -ForegroundColor Red
        }
    } finally {
        if (Test-Path Env:\App__Timeout) { Remove-Item Env:\App__Timeout -ErrorAction SilentlyContinue }
    }
}

Write-Host "`nBuilding project..." -ForegroundColor Gray
& dotnet build $projectDir --verbosity quiet > $null

Write-Host "`n=== Config Precedence Tests ===" -ForegroundColor Cyan

Run-Test -Name "1. JSON only"              -Expected "30"
Run-Test -Name "2. ENV over JSON"          -Expected "120" -EnvVal "120"
Run-Test -Name "3. CLI over ENV over JSON" -Expected "300" -EnvVal "120" -CliArgs @("--App:Timeout=300")

Write-Host "`n=== Results: $passed / $total ===" -ForegroundColor Cyan

if ($passed -eq $total) {
    Write-Host "SUCCESS: All precedence tests passed" -ForegroundColor Green
    exit 0
} else {
    Write-Host "FAILURE: Some tests failed" -ForegroundColor Red
    exit 1
}