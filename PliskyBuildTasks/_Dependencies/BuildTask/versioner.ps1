[CmdletBinding(DefaultParameterSetName = 'None')]
param
(
    [String] #[Parameter(Mandatory = $true)]
    $VersionNumber,

    [String] #[Parameter(Mandatory = $true)]
    $AssemblyVersionFiles,

    [String] #[Parameter(Mandatory = $true)]
    $FileVersionFiles,

    [String] #[Parameter(Mandatory = $true)]
    $WixVersionFiles
)




$scriptName = "Plisky Versioner :"

$scriptpath = Split-Path $MyInvocation.MyCommand.Path
Write-Host ($scriptpath)
Write-Host ($scriptName + " execution starts.")
Write-Verbose ($scriptName + "VersionNumber = $VersionNumber")
Write-Verbose ($scriptName + "AssemblyVersionFiles = $AssemblyVersionFiles")
Write-Verbose ($scriptName + "FileVersionFiles = $FileVersionFiles")
Write-Verbose ($scriptName + "WixVersionFiles = $WixVersionFiles") 

# Import the Task.Internal dll that has all the cmdlets we need for Build
#import-module "Microsoft.TeamFoundation.DistributedTask.Task.Internal"


Write-Verbose ($scriptName + "Script Completes")
[System.Reflection.Assembly]::LoadFrom( ($scriptpath + "\pliskyversioningsupport.dll"))
$versioner = New-Object Plisky.Infrastructure.Build.BuildVersioner
$versioner.RootPath = $Build.SourcesDirectory
$versioner.AssemblyFilesToUpdate= $AssemblyVersionFiles
$versioner.FileVersionFilesToUpdate=$FileVersionFiles
$versioner.WixVersionFilesToUpdate = $WixVersionFiles
$versioner.VersionNumberKey = $VersionNumber
$versioner.IncrementAndApplyVersion()

