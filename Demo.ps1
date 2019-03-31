dotnet build source\Alias -o Demo
Set-Location -Path Demo
@'
{
	"binding": {
		"mklink.exe": {
			"command": "cmd",
			"arguments": "/c mklink"
		},
		"cowsay.exe": {
			"command": "cmd",
			"arguments": "/c \"%0\\cowsay.cmd\""
		}
	}
}
'@ -replace '%0',($PWD.Path -replace '\\','\\') | Set-Content -Path alias.conf
@'
_(~)_
 )"(
(@_@)
 ) (
'@ | Set-Content -Path cow.txt
@'
@echo off
type %~dp0cow.txt
echo.%*
'@ | Set-Content -Path cowsay.cmd
-split 'mklink cowsay' | % {
	New-Item -ItemType HardLink -Path "$_.exe" -Value Alias.exe
}
