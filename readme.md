# Alias
Custom commands: make an executable for any command.

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
The alias application maintains an assignment of alias names to commands in a configuration file `alias.conf`.
When alias runs, it looks up its basename in the configuration.
If the name is assigned, it runs the assigned command.
Otherwise, it runs in configuration mode, which accepts options to configure commands.
Configuration mode creates executables by simply linking/copying the alias executable and adding entries in `alias.conf` to associate link/copy filenames with commands: the user may edit `alias.conf` and name links/copies to achieve the same effect.
By convention, this guide assumes the alias application has the unassigned name `alias`, though the user can rename the application to any unassigned name.

## Add/Set Command
alias --set name --command command
Add or change name’s configuration.

## Remove Command
alias --unset name
Remove name’s configuration.

## Reset
alias --reset
Remove all configured names.

## Restore
alias --restore
Recreate links for all configured names.