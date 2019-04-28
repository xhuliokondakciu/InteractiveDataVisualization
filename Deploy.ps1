Import-Module WebAdministration
$iisAppPoolName = "DefaultAppPool"
$iisAppPoolDotNetVersion = "v4.0"
$iisAppName = "ThesisXhulio"
$directoryPath = "C:\ThesisXhulio"

$currentPath = Get-Location

#navigate to the app pools root
Set-Location IIS:\AppPools\

#check if the app pool exists
if (!(Test-Path $iisAppPoolName -pathType container)) {
    #create the app pool
    $appPool = New-Item $iisAppPoolName
    $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value $iisAppPoolDotNetVersion
}

#navigate to the sites root
Set-Location IIS:\Sites\

#check if the site exists
if (!(Test-Path $iisAppName -pathType container)) {
    #create the site
    $iisApp = New-Item $iisAppName -bindings @{protocol = "http"; bindingInformation = "*:52262:" } -physicalPath $directoryPath
    $iisApp | Set-ItemProperty -Name "applicationPool" -Value $iisAppPoolName
}

Set-Location $currentPath

MSBuild.exe /Target:Clean

MSBuild.exe /p:DeployOnBuild=true /p:PublishProfile=WebDeploy

Start-Process http://localhost:52262/