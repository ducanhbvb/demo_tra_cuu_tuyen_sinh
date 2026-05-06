# Compress large images to reduce file size
# Usage: powershell -ExecutionPolicy Bypass -File tools\compress-images.ps1

$imagesFolder = "Resources\Images"
$targetFile = "campus-background.jpg"

$fullPath = Join-Path $imagesFolder $targetFile

if (-not (Test-Path $fullPath)) {
    Write-Host "[ERROR] File not found: $fullPath" -ForegroundColor Red
    exit 1
}

# Check if System.Drawing is available
try {
    Add-Type -AssemblyName System.Drawing -ErrorAction Stop
} catch {
    Write-Host "[ERROR] System.Drawing not available" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== COMPRESS IMAGE ===" -ForegroundColor Cyan
Write-Host "Input:" $fullPath
$origSize = [math]::Round((Get-Item $fullPath).Length / 1024, 2)
Write-Host "Original size:" $origSize "KB"
Write-Host ""

try {
    $img = [System.Drawing.Image]::FromFile($fullPath)

    $encoder = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.FormatID -eq [System.Drawing.Imaging.ImageFormat]::Jpeg.Guid }

    $encoderParams = New-Object System.Drawing.Imaging.EncoderParameters(1)
    $encoderParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter([System.Drawing.Imaging.Encoder]::Quality, 50)

    $outputPath = Join-Path $imagesFolder "campus-background-opt.jpg"
    $img.Save($outputPath, $encoder, $encoderParams)
    $img.Dispose()

    $newSize = [math]::Round((Get-Item $outputPath).Length / 1024, 2)
    $reduction = [math]::Round((1 - $newSize / $origSize) * 100, 1)

    Write-Host "Compressed image saved to:" $outputPath -ForegroundColor Green
    Write-Host "New size:" $newSize "KB" -ForegroundColor Green
    Write-Host "Reduction:" $reduction "%" -ForegroundColor Green
    Write-Host ""
    Write-Host "[IMPORTANT] Update Content/Client.css to use 'campus-background-opt.jpg'!" -ForegroundColor Yellow
} catch {
    Write-Host "[ERROR] $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Compression completed." -ForegroundColor Cyan
