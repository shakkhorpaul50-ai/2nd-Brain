# Run this from the USB root after editing the model filename if needed.
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$llamaServer = Join-Path $root "llama.cpp\llama-server.exe"
$model = Join-Path $root "models\your-coder-model.gguf"
$app = Join-Path $root "LocalCodeAI\OfflineCodeAssistant.exe"

if (!(Test-Path $llamaServer)) { throw "Missing llama-server.exe at $llamaServer" }
if (!(Test-Path $model)) { throw "Set `$model to your GGUF model path. Missing: $model" }
if (!(Test-Path $app)) { throw "Missing published app at $app" }

$server = Start-Process -FilePath $llamaServer -ArgumentList "-m `"$model`" --host 127.0.0.1 --port 8080 -c 4096" -PassThru
try {
    for ($i = 0; $i -lt 40; $i++) {
        try { Invoke-WebRequest http://127.0.0.1:8080/health -UseBasicParsing -TimeoutSec 1 | Out-Null; break } catch { Start-Sleep -Seconds 1 }
    }
    Start-Process -FilePath $app
    Write-Host "Local Code AI is running. Close this window to stop the model server."
    Wait-Process -Id $server.Id
} finally { if (!$server.HasExited) { Stop-Process -Id $server.Id -Force } }
