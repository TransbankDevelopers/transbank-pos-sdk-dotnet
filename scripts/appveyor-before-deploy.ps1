Write-Host "Before Deploy Script"
Set-Location TransbankPosSDK
if ($env:APPVEYOR_REPO_TAG_NAME -Match "^v[0-9]+\.[0-9]+\.[0-9]+" ){
    Write-Host "Version Tag Found Deploy new Nuget Release"
    $VERSION_NUMBER=$env:APPVEYOR_REPO_TAG_NAME.substring(1)
    dotnet.exe pack TransbankPosSDK.csproj -c release  -p:Version=$VERSION_NUMBER --output nupkgs
    Push-AppveyorArtifact '.\nupkgs\*.nupkg'
    Write-Host "Done"
}
elseif ($env:APPVEYOR_FORCED_BUILD -Match "true") {
    Write-Host "New build button presed and no version Tag Found deploy prerelease to Nuget"
    dotnet.exe pack Transbank.csproj -c release --version-suffix ci-$env:APPVEYOR_BUILD_ID --output nupkgs
    Push-AppveyorArtifact '.\nupkgs\*.nupkg'
    Write-Host "Done"
}

Set-Location ..