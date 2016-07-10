Param(
  [Parameter(Mandatory=$True,Position=1)]
  [string]$bin,

  [Parameter(Mandatory=$True,Position=2)]
  [string]$libs
)

Write-Host "Removing *.xml, *.pdb, etc..."
rm -Path "$bin\*" -recurse -Include *.xml,*.pdb
rm -Path "$bin\*" -Include *vshost*
 
Write-Host "Removing Mac folder..."

$mac_exists = test-path "$bin\Toolbox.Desktop.app"
if ($mac_exists -eq $true) {
    rm -Path "$bin\Toolbox.Desktop.app" -Recurse
} else {
    write-host "Mac folder doesn't exists."
}

Write-Host "Copying libraries..."
copy-item "$libs\*" $bin

Write-Host "Removing xf.db..."
rm -Path "$bin\*.db"


Write-Host "Cleaning up plugins folder..."
$plugin_exists = test-path "$bin\Plugins"

if ($plugin_exists -eq $false) {
    throw Exception("Plugins folder not found.")
}

rm -Path "$bin\Plugins\*" -Exclude "XenForms.Plugin.*.dll"