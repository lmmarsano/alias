Set-StrictMode -Version Latest
[string]$csproj = [System.Uri]::new([System.IO.Path]::Combine($PSScriptRoot, 'source', 'Alias', 'Alias.csproj'))
try {
	[System.Xml.XmlReader]$xmlReader = [System.Xml.XmlReader]::Create($csproj)
	$xmlReader.ReadToDescendant('Version')
	$version = $xmlReader.ReadElementContentAsString()
} finally {
	$xmlReader.Dispose()
}
'win', 'linux', 'osx' | % {
	dotnet publish -c Release -r $_-x64 --self-contained false -o release/$_/alias ./source/Alias
}
Compress-Archive -Path ./release/win/alias/* -DestinationPath alias-win-x64.zip
'linux', 'osx' | % {
	wsl tar -C ./release/$_ -caf alias-$_-x64.txz .
}
Remove-Item -Path ./release/ -Recurse
wsl ~$env:USERNAME/.local/bin/hub release create -a alias-win-x64.zip\#Windows -a alias-linux-x64.txz\#Linux -a alias-osx-x64.txz\#MacOS -m $version v$version
