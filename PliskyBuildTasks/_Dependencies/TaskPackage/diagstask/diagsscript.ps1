[CmdletBinding(DefaultParameterSetName = 'None')]
param
(
    [String] $pbv
)


Write-Host "Versioner Starts Execution"

$scriptName = "Plisky Versioner :"
$scriptpath = Split-Path $MyInvocation.MyCommand.Path

Write-Host ($scriptpath)
Write-Host ($scriptName + " execution starts.")


Write-Host "##vso[task.logissue type=warning;] PowerShell Warning Test"

Write-Host No problem reading $env:DynamicVariable

Write-Host ("##vso[task.setvariable variable=pliskyBuildVerNumber;]Bannana")

Write-Host "output done"
write-host ("HERE 0>> pliskyBuildVerNumber VALUE: {0}" -f $env:pliskyBuildVerNumber)
Write-Host $versioner.VersionString
Write-Host ("SOME TEXT $($(DynamicVariable)")
Write-Host ("##vso[task.setvariable variable=pliskyBuildVerNumber;]Bannana")
write-host ("HERE 0>> pliskyBuildVerNumber VALUE: {0}" -f $pbv)
Write-Host "output done 1"
Write-Host ("##vso[task.setvariable variable=pliskyBuildVerNumber;]$($versioner.VersionString)")
Write-Host "output done 1.5"
write-host ("HERE 1>> pliskyBuildVerNumber VALUE: {0}" -f $env:pliskyBuildVerNumber)
Write-Host "output done 2"

Write-Host "output done 3 "
write-host ("HERE 2 >> pliskyBuildVerNumber VALUE: {0}" -f $env:pliskyBuildVerNumber)

Get-ChildItem Env:. | out-string



