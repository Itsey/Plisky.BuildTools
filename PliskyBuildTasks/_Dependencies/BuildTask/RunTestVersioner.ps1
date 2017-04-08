
$ffx = (Split-Path $MyInvocation.MyCommand.Path) + "\..\..\PliskyVersioningSupport\Bin\Debug\"
Write-Host   $ffx
Copy-Item -Path $ffx -Filter *.* -Destination ".\" -Force
start-process powershell -ArgumentList "-noexit -Command .\versioner.ps1 -verbose " 
#Invoke-Expression 