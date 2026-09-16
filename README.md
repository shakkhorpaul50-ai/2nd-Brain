# Local Code AI — USB, offline, and Node-free

A ChatGPT-style C# web app that talks **only** to a `llama.cpp` server bound to `127.0.0.1`. It has no npm packages, CDN assets, telemetry, or cloud API. It helps with C#, Razor (`.cshtml`), and CSS.

## USB layout

```
USB:\
  LocalCodeAI\              # publish output from this repository
  llama.cpp\llama-server.exe
  models\your-coder-model.gguf
  Start-Local-Code-AI.ps1
```

The app itself is small. For a 28 GB free drive, use a 7B–8B coding GGUF at `Q4_K_M` (typically 4–6 GB). This leaves comfortable space for the published .NET app and optional .NET runtime. Faster responses depend mainly on the host computer's RAM/CPU or GPU; a USB drive does not make inference as fast as a cloud GPU.

## One-time build (on a development computer)

Install the **.NET 8 SDK**, then publish a self-contained Windows build. This embeds the runtime so the target PC does not need .NET or Node.js:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o E:\LocalCodeAI
```

Download a Windows `llama.cpp` release and a GGUF instruct/coding model once, copy them to the USB layout above, then disconnect from the internet. The application and model will run entirely locally after that.

## Starting on Windows

Edit the two paths in `scripts/Start-Local-Code-AI.ps1` after copying it to the USB root, then run it. It starts `llama-server`, waits for its local OpenAI-compatible endpoint, opens the browser UI, and stops the server when you close the script.

The default UI connection is `http://127.0.0.1:8080`. Use **Local setup** in the app if you change the port, model label, response length, or temperature.

## Development

```bash
dotnet run
```

Then start a compatible local model server at `http://127.0.0.1:8080` and browse to the URL printed by ASP.NET.
