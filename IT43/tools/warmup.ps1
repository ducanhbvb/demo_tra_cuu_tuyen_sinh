param(
    [string]$Url = "http://localhost:63601",
    [int]$Timeout = 30
)

$endpoints = @(
    "/",
    "/Client/ChiTietTruong.aspx?id=1",
    "/Client/index.aspx",
    "/Admin/QuanLyTruong.aspx",
    "/Client/ChiTietTinTuyenSinh.aspx?id=1"
)

Write-Host "`n=== WARM-UP APPLICATION ===" -ForegroundColor Cyan
Write-Host "Target: $Url" -ForegroundColor Cyan
Write-Host "Time: $(Get-Date)`n" -ForegroundColor Cyan

$success = 0
$failed = 0

foreach ($endpoint in $endpoints) {
    $fullUrl = $Url + $endpoint
    try {
        $timer = Measure-Command {
            $response = Invoke-WebRequest -Uri $fullUrl -UseBasicParsing -TimeoutSec $Timeout
        }
        if ($response.StatusCode -eq 200) {
            Write-Host "[OK] $endpoint ($($timer.TotalSeconds)s)" -ForegroundColor Green
            $success++
        } else {
            Write-Host "[FAIL] $endpoint (Status: $($response.StatusCode))" -ForegroundColor Red
            $failed++
        }
    } catch [System.Net.WebException] {
        Write-Host "[FAIL] $endpoint (Connection failed)" -ForegroundColor Red
        $failed++
    } catch {
        Write-Host "[FAIL] $endpoint ($($_.Exception.Message))" -ForegroundColor Red
        $failed++
    }
}

Write-Host "`n=== SUMMARY ===" -ForegroundColor Cyan
Write-Host "Successful: $success" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Red
Write-Host "Total: $($endpoints.Count)" -ForegroundColor Yellow
Write-Host "`nWarm-up completed at $(Get-Date)`n" -ForegroundColor Cyan
