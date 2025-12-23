$ModName = "ED-LaserDrill"
$RimworldPath = "D:\SteamLibrary\steamapps\common\RimWorld"

$ExePath = $RimworldPath + "\RimWorldWin64.exe"
$ModSource = "D:\~Git\Jaxxa-Rimworld\~SubModules\" + $ModName +"\" + $ModName
$ModDestination = $RimworldPath + "\Mods\" + $ModName

Remove-Item -Path $ModDestination -Recurse

Copy-Item -Path $ModSource -Destination $ModDestination -Recurse

start -FilePath $ExePath

#start -FilePath $ExePath -ArgumentList "-savedatafolder=C:\~Git\RimworldSaves_1.3\ModTest"