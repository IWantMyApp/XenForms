Param(
  [Parameter(Mandatory=$True,Position=1)]
  [string]$ToolboxVersion,
  
  [Parameter(Mandatory=$True,Position=2)]
  [string]$ProjectFolder
)

$toolboxdir = Join-Path $ProjectFolder "Source\Toolbox"


Write-Host "Updating Toolbox version number to $ToolboxVersion" -foregroundcolor Green
.\Update-AllAssemblyInfoFiles.ps1 $ToolboxVersion -path $toolboxdir