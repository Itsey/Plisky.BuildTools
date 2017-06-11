$ffx = (Split-Path $MyInvocation.MyCommand.Path) + "\..\..\..\PliskyVer\bin\Debug\*.*"
$tempFolder = "D:\Temp\_DelWorking\Vertests"
Write-Host   $ffx

Copy-Item -Path $ffx -Filter *.* -Destination $tempFolder -Force
Write-Host "Copying BuildTask"
Copy-Item -Path "..\..\TaskPackage\buildtask\*.*" -Filter *.* -Destination $tempFolder -Force
Copy-Item -Path "..\..\TaskPackage\scripts\*.*" -Filter *.* -Destination $tempFolder -Force
Write-Host "Copying Json"
Copy-Item -Path ".\*.json" -Filter *.* -Destination $tempFolder -Force
Write-Host "Copying Test Data"
Copy-Item -Path "..\TestFileStructure\Properties\Assemblyinfo.txt" -Destination $tempFolder"\assemblyinfo.cs" -Force
Write-Host "running Process"

$versionerScript = $tempFolder+"\versioner.ps1"
$jsonfile = $tempFolder+"\versionrules.json"

pushd $tempFolder
start-process powershell -ArgumentList "-noexit -Command .\versioner.ps1 $jsonFile **\assemblyinfo.cs!FILE $tempFolder -verbose" 
#Invoke-Expression 
popd