Import-Module -Name ".\build\Invoke-MsBuild.psm1"

nuget restore

Write-Host "Building..."
$buildSucceeded = Invoke-MsBuild -Path ".\Noobot.sln" -MsBuildParameters "/target:Clean;Build /property:Configuration=Release;Platform=""Any CPU"" /verbosity:Minimal"
if ($buildSucceeded)
{ 
	Write-Host "Build completed successfully." 
}
else
{ 
	Write-Host "Build failed. Check the build log file for errors." 
}