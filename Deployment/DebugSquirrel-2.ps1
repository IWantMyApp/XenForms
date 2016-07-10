Param(
  [Parameter(Mandatory=$True,Position=1)]
  [string]$ToolboxVersion,
  
  [Parameter(Mandatory=$True,Position=2)]
  [string]$ProjectFolder
)

$spec = Join-Path $ProjectFolder "\build\XenForms"
$nupkg = "$spec.$ToolboxVersion.nupkg"

$squirrel = Join-Path $ProjectFolder "packages\squirrel.windows.1.4.0\tools\Squirrel"
$outdir = Join-Path $ProjectFolder "Output"
$libs = Join-Path $ProjectFolder "Libraries"
$releasedir = Join-Path $outdir "Debug"
$bin = Join-Path $releasedir "Desktop"
$toolboxdir = Join-Path $ProjectFolder "Source\Toolbox"


Write-Host "Updating Toolbox version number to $ToolboxVersion" -foregroundcolor Green
.\Update-AllAssemblyInfoFiles.ps1 $ToolboxVersion -path $toolboxdir


Write-Host "Cleaning up Toolbox" -foregroundcolor Green
.\Cleanup-Toolbox.ps1 -bin $bin -libs $libs


Write-Host "Cleaning up Designer" -foregroundcolor Green
.\Cleanup-Designer.ps1 -bin $bin -projectfolder $ProjectFolder


Write-Host "Packaging files" -foregroundcolor Green
Start-Process ".\nuget" -ArgumentList "pack $spec.nuspec -Version $ToolboxVersion" -NoNewWindow -Wait


Write-Host "Calling Squirrel" -foregroundcolor Green
Start-Process $squirrel -ArgumentList "--releasify $nupkg" -NoNewWindow -Wait