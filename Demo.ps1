Set-StrictMode -Version Latest
dotnet build (Join-Path -Path source -ChildPath Alias) -o Demo
Set-Location -Path Demo
'@"%~dp0Alias.exe" set %* && mklink /h "%~dp0%1.exe" "%~dp0Alias.exe" 2>NUL >NUL' | Set-Content -Path alias-set.cmd
'@"%~dp0Alias.exe" unset %* && del "%~dp0%1.exe" 2>NUL >NUL' | Set-Content -Path alias-unset.cmd
@'
Set-StrictMode -Version Latest
$ErrorActionPreference=[System.Management.Automation.ActionPreference]::Stop
function Set-Alias {
	[CmdletBinding()]
	param(
		# Alias file name
		[Parameter(Mandatory = $true)]
		[string]
		$Alias,
		# Command file name or path
		[Parameter(Mandatory = $true)]
		[string]
		$Command,
		# Command arguments
		[Parameter(ValueFromRemainingArguments = $true)]
		[string[]]
		$Arguments
	)

	#region
	$Target = Join-Path -Path $PSScriptRoot -ChildPath Alias.exe
	$LinkPath = Join-Path -Path $PSScriptRoot -ChildPath $Alias
	#endregion
	#region
	& $Target set $Alias $Command @Arguments
	if ($?) {
		try {
			[void](New-Item -Verbose:$VerbosePreference -ErrorAction $ErrorActionPreference -Path "$LinkPath.exe" -ItemType HardLink -Value $Target)
			Write-Verbose -Message ('{0} created' -f $LinkPath)
		} catch {
			Write-Verbose -Message ('{0} not created' -f $LinkPath)
		}
	}
	#endregion
}
[string[]]$Arguments = $args
Set-Alias @Arguments
'@ | Set-Content -Path alias-set.ps1
@'
Set-StrictMode -Version Latest
$ErrorActionPreference=[System.Management.Automation.ActionPreference]::Stop
function Remove-Alias {
	[CmdletBinding()]
	param(
		# Alias file name
		[Parameter(Mandatory = $true)]
		[string]
		$Alias
	)

	#region
	$Target = Join-Path -Path $PSScriptRoot -ChildPath Alias.exe
	$LinkPath = Join-Path -Path $PSScriptRoot -ChildPath $Alias
	#endregion
	#region
	& $Target unset $Alias
	if ($?) {
		try {
			[void](Remove-Item -Verbose:$VerbosePreference -ErrorAction $ErrorActionPreference -Path "$LinkPath.exe")
			Write-Verbose -Message ('{0} removed' -f $LinkPath)
		} catch {
			Write-Verbose -Message ('{0} not removed' -f $LinkPath)
		}
	}
	#endregion
}
[string[]]$Arguments = $args
Remove-Alias @Arguments
'@ | Set-Content -Path alias-unset.ps1
$Alias = Resolve-Path -Path Alias.exe
& $Alias set alias-set powershell -- -NoProfile -NoLogo -File (Resolve-Path -Path alias-set.ps1)
$AliasSet = New-Item -ItemType HardLink -Path alias-set.exe -Value Alias.exe
& $AliasSet alias-unset powershell -- -NoProfile -NoLogo -File (Resolve-Path -Path alias-unset.ps1)
& $AliasSet mklink cmd /c mklink
