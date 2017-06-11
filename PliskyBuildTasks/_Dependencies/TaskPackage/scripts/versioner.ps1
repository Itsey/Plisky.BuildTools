[CmdletBinding(DefaultParameterSetName = 'None')]
param
(
    [String] #[Parameter(Mandatory = $true)]
    $VersionRuleSource,

    [String] #[Parameter(Mandatory = $true)]
    $VersionsToApply,

	[String] #[Parameter(Mandatory = $true)]
    $SourceRootDirectory
)

function Log ([string]$Severity, [string]$Text) {
    ## Use one of the VSTS Build environment variables to check if we execute the PowerShell script
    ## on a local machine or on the VSTS Build server
    $isLocal = !$env:SYSTEM_TEAMPROJECT
    Write-Host "IN LOG"
    if ($Severity -eq "Fatal") {
        if ($isLocal -eq $true) { Write-Error $Text } else { Write-Error "##vso[task.logissue type=error;]$Text" }
    } if ($Severity -eq "Error") {
        if ($isLocal -eq $true) { Write-Error $Text } else { Write-Error "##vso[task.logissue type=error;]$Text" }
    } elseif ($Severity -eq "Warn") {
        if ($isLocal -eq $true) { Write-Warning $Text } else { Write-Host "##vso[task.logissue type=warning;]$Text" }
    } else {
        Write-Host $Text
    }
}




Write-Host "Versioner Starts Execution"

$scriptpath = Split-Path $MyInvocation.MyCommand.Path
[System.Reflection.Assembly]::LoadFrom( ($scriptpath + "\pliskyver.dll"))

$versioner = New-Object Plisky.Build.VersioningTask

$logEventAction =  { 
      Write-Host "Log Event Action Called"
	  Write-Host $EventArgs.Text
      Log $EventArgs.Severity $EventArgs.Text
}    

Write-Host "registering event handler"
#$eh = Register-ObjectEvent -SourceIdentifier "Logger" -InputObject $versioner -EventName Logger -Action $logEventAction
$eh = Register-ObjectEvent  -InputObject $versioner -EventName Logger -Action $logEventAction
Write-Host "registered"

$scriptName = "Plisky Versioner :"
$scriptpath = Split-Path $MyInvocation.MyCommand.Path

Write-Host ($scriptpath)
Write-Host ($scriptName + " execution starts.")

Write-Verbose ($scriptName + "VersionStorage = $VersionRuleSource")
Write-Verbose ($scriptName + "VersionsToApply = $VersionsToApply")
Write-Verbose ($scriptName + "SourceRootDirectory = $SourceRootDirectory")

Write-Host ($scriptName + "VersionStorage = $VersionRuleSource")
Write-Host ($scriptName + "VersionsToApply = $VersionsToApply")
Write-Host ($scriptName + "SourceRootDirectory = $SourceRootDirectory")


Write-Verbose ($scriptName + "Script Completes")


$versioner.BaseSearchDir = $SourceRootDirectory
$versioner.PersistanceValue = $VersionRuleSource
$versioner.SetAllVersioningItems($VersionsToApply)

$versioner.IncrementAndUpdateAll()



function RegisterEventLogger {
   
 
    $logEvent = Get-EventSubscriber | Where-Object  { $_.SourceIdentifier -eq "ScipBe.EventLogger.LogEvent" } | measure
 
    if ($logEvent.Count -eq 0)
    {
        $eventLogger = [ScipBe.Demo2016.VstsBuild.BuildTasks.Core.Providers.EventLogger]
        Register-ObjectEvent -InputObject $eventLogger -SourceIdentifier "ScipBe.EventLogger.LogEvent" -EventName LogEvent -Action $logEventAction
    }
}


