# Delegator
Make any command appear as an executable file.

# Requirements
.NET Core runtime

# Installation
Save the application to a directory in your PATH.
```powershell
New-Item -Type Directory -Path $env:USERPROFILE\path
Set-Location -Path $env:USERPROFILE\path
Start-BitsTransfer -Source $url
[System.Environment]::SetEnvironmentVariable(
	'PATH',
	(('%USERPROFILE%\path', [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User) -split ';' | Select-Object -Unique -join ';'),
	[System.EnvironmentVariableTarget]::User
)
```

# Usage
The application maintains an assignment of names to commands in a configuration file `delegator.conf`.
When the application runs, it looks up its basename in the configuration.
If the name is assigned, it runs the assigned command.
Otherwise, it runs in configuration mode, which accepts options to configure commands.
By convention, this guide assumes the application has the unassigned name `delegate`, though the user can rename the application to any unassigned name.

## Add/Set Command
delegate --set name --command command
Add or change name's configuration.

## Remove Command
delegate --unset name
Remove name's configuration.

## Reset
delegate --reset
Remove all configured names.

## Restore
delegate --restore
Recreate links for all configured names.