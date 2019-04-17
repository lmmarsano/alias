# Alias
Custom commands: make an executable for any command.

# Description
The alias application maintains an assignment of alias names to commands in a configuration file `alias.conf`.
When alias runs, it looks up its file name and runs the assigned command if any.
Otherwise, if no assignment to that name exists, it runs in configuration mode, providing an alternative interface to edit the configuration file.
The user may also directly edit `alias.conf` by following the [JSON reference](#json-reference).

# Why?
Some Windows applications are, to put kindly, *limited*.
You might need to fool them into thinking that script you want it to run is actually an `exe` file.
Or maybe you want that Linux executable under [WSL][wsl] to look like a native application.
Do we really want to write another `exe` to run a single command?
Do we really want to track down these developers, tell them to write better code, and hope they will?
No?
Here’s a workaround.

# Requirements
[.NET Core 3.0][dotnet] runtime

# Installation
Save the application to a directory in your `PATH`.
```PowerShell
New-Item -Type Directory -Path ~\path | Set-Location
Start-BitsTransfer -Source https://github.com/lmmarsano/alias/releases/latest/download/alias-win-x64.zip
Expand-Archive -Path alias-win-x64.zip
Remove-Item -Path alias-win-x64.zip
[System.Environment]::SetEnvironmentVariable(
	'PATH',
	(('%USERPROFILE%\path', [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User) -split ';' | Select-Object -Unique -join ';'),
	[System.EnvironmentVariableTarget]::User
)
```

x86-64 binaries are also released for [Linux][release-linux] and [MacOS][release-osx].

# Example
In a directory containing the `alias` executable
```PowerShell
New-Item -Type HardLink -Name test.exe -Value alias.exe
alias set test cmd /c echo
.\test hello world
# hello world
```

# Usage
By convention, this guide assumes the alias application has the unassigned name `alias`, though the user can rename the application to any unassigned name.

## Run External Command
In the same directory as the the alias executable, link or copy the executable to *name* (*name*`.exe` on Windows), configure *name*,
> `alias set` name command initial-argument*

and run
> name argument*

If *name* is configured, then this runs the configured command and initial arguments with the additional arguments.
> command initial-argument* argument*

## Add/Set Command
> `alias set` name command argument*

Add or change named alias’ configuration.
To pass `-` prefixed arguments, first include `--` to disable option processing.
`--` will be excluded from the saved command.
To save `--`, too, repeat it.

## Remove Command
> `alias unset` name

Remove the alias.

## Reset
> `alias reset`

Remove all configured aliases.

# JSON Reference
Located in the same directory as the `alias` executable, `alias.conf` is a plain [JSON][json] file with a root object containing the `"binding"` property.
```JavaScript
{"binding": bindings}
```

## `bindings`
```JavaScript
{(alias: commandEntry)*}
```
An object with any number of mappings between *alias* keys *commandEntry* values.
Each alias must be unique.

### `alias`
```JavaScript
unique string
```
A plain string naming an alias.
The name must include executable file name extension (`exe` on Windows) if the operating system requires it.

### `commandEntry`
```JavaScript
{"command": command, "arguments"?: arguments}
```
An object with `"command"` and optional `"arguments"` properties.

#### `command`
```JavaScript
string
```
A string naming an executable.
The full path is only required as your system requires (eg, directory not in the environment `PATH` variable).

#### `arguments`
```JavaScript
string
```
A single string containing arguments as you would enter them in the console.
Escape `"` as `\"` and `\` as `\\`.
The command will be called with these arguments and any additional arguments passed through invocation.

# Development
Install at least [.NET Core 3.0][dotnet], clone the repository, change into the directory, and build.
```PowerShell
git clone https://github.com/lmmarsano/alias.git
Set-Location -Path alias
dotnet build
```
Edit sources in `source\Alias`.
For [Visual Studio Code][vscode]
```PowerShell
code source\Alias
```
An executable is built under `source\Alias\bin\Debug\netcoreapp3.0`.
```PowerShell
Set-Location -Path source\Alias\bin\Debug\netcoreapp3.0
.\Alias
```

# Demo
Try it out by running the demo script
```PowerShell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
.\Demo.ps1
.\Alias list
# alias-set
# alias-unset
# mklink
```
and running the generated executables.
- `mklink.exe` calls the link creation command from the `cmd.exe` environment
- `alias-set.exe` calls `alias set` and hardlinks an executable alias
- `alias-unset.exe` calls `alias unset` and removes the executable alias

# To Do
Include file system operations to link/copy/remove the alias executable as configuration mode edits `alias.conf`.

[release-linux]: //github.com/lmmarsano/alias/releases/latest/download/alias-linux-x64.txz
[release-osx]: //github.com/lmmarsano/alias/releases/latest/download/alias-osx-x64.txz
[dotnet]: https://dotnet.microsoft.com/download/dotnet-core/3.0
[wsl]: https://docs.microsoft.com/en-us/windows/wsl/about
[json]: https://www.json.org/
[vscode]: https://code.visualstudio.com/
