Param(
  [Parameter(Mandatory=$True,Position=1)]
  [string]$ToolboxVersion,
  
  [Parameter(Mandatory=$True,Position=2)]
  [string]$ProjectFolder
)

$spec = Join-Path $ProjectFolder "\Deployment\XenForms"
$nupkg = "$spec.$ToolboxVersion.nupkg"

$squirrel = Join-Path $ProjectFolder "packages\squirrel.windows.1.4.0\tools\Squirrel"
$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

if ((test-path $msbuild) -eq $false) {
  throw "msbuild not found: $msbuild"
} else {
  Write-Host "MSBuild 2015 Found." -foregroundcolor Green
}

$outdir = Join-Path $ProjectFolder "Artifacts"
$libs = Join-Path $ProjectFolder "Libraries"
$releasedir = Join-Path $outdir "Windows"
$bin = Join-Path $releasedir "Release"
$toolboxdir = Join-Path $ProjectFolder "Source\Toolbox"


Write-Host "Removing output folders..." -foregroundcolor Green
$release_exists = test-path $releasedir

if ($release_exists -eq $true) {
    rm -path "$releasedir\*" -recurse
}

Write-Host "Updating Toolbox version number to $ToolboxVersion" -foregroundcolor Green
.\Update-AllAssemblyInfoFiles.ps1 $ToolboxVersion -path $toolboxdir


Write-Host "Executing NuGet package restore for Toolbox" -foregroundcolor Green
Start-Process ".\NuGet.exe" -ArgumentList "restore $ProjectFolder\toolbox.windows.sln" -NoNewWindow -Wait


Write-Host "Building Toolbox" -foregroundcolor Green
Start-Process $msbuild -ArgumentList "$ProjectFolder\toolbox.windows.sln /p:UseSharedConfiguration=false /p:Configuration=Release /p:Platform=`"Mixed Platforms`"" -NoNewWindow -Wait


Write-Host "Executing NuGet package restore for Designer" -foregroundcolor Green
Start-Process ".\NuGet.exe" -ArgumentList "restore $ProjectFolder\designer.sln" -NoNewWindow -Wait


Write-Host "Cleaning Designer" -foregroundcolor Green
Start-Process $msbuild -ArgumentList "$ProjectFolder\Source\Designer\Droid\Droid.csproj /t:Clean /p:UseSharedConfiguration=false /p:Configuration=Debug" -NoNewWindow -Wait


Write-Host "Building Designer" -foregroundcolor Green
Start-Process $msbuild -ArgumentList "$ProjectFolder\Source\Designer\Droid\Droid.csproj /t:PackageForAndroid /p:UseSharedConfiguration=false /p:Configuration=Debug" -NoNewWindow -Wait


Write-Host "Cleaning up Toolbox" -foregroundcolor Green
.\Cleanup-Toolbox.ps1 -bin $bin -libs $libs


Write-Host "Cleaning up Designer" -foregroundcolor Green
.\Cleanup-Designer.ps1 -bin $bin -projectfolder $ProjectFolder


Write-Host "Packaging files" -foregroundcolor Green
Start-Process ".\nuget" -ArgumentList "pack $spec.nuspec -Version $ToolboxVersion" -NoNewWindow -Wait


Write-Host "Calling Squirrel" -foregroundcolor Green
Start-Process $squirrel -ArgumentList "--releasify $nupkg" -NoNewWindow -Wait