.\build.ps1

cd .\src\Noobot.Runner\bin\Release\

Write-Host "Installing service..."
.\Noobot.Runner.exe install
.\Noobot.Runner.exe start 