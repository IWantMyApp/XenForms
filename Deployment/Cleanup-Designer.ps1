Param(
  [Parameter(Mandatory=$True,Position=1)]
  [string]$bin,

  [Parameter(Mandatory=$True,Position=1)]
  [string]$ProjectFolder
)

Write-Host "Copying designer apk to desktop folder..." -ForegroundColor Green
Copy-Item "$ProjectFolder\Source\Designer\Droid\bin\Debug\com.xenforms.designer.apk" "$bin\com.xenforms.designer.unsigned.apk"


Write-Host "Executing JarSigner" -foregroundcolor Green
Start-Process 'C:\Program Files\Java\jdk1.8.0_91\bin\jarsigner.exe' -ArgumentList "-verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore $ProjectFolder\Build\xenforms.keystore -signedjar $bin\com.xenforms.designer.unsigned.apk $bin\com.xenforms.designer.unsigned.apk xenforms" -NoNewWindow -Wait


Write-Host "Executing ZipAlign" -foregroundcolor Green
Start-Process 'C:\Users\rmanager\AppData\Local\Android\sdk\build-tools\23.0.3\zipalign.exe' -ArgumentList "-f -v 4 $bin\com.xenforms.designer.unsigned.apk $bin\com.xenforms.designer.apk" -NoNewWindow -Wait


Write-Host "Removing unsigned apk" -foregroundcolor Green
rm -Path "$bin\com.xenforms.designer.unsigned.apk"
