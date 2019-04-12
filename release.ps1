[string]$csproj = [System.Uri]::new([System.IO.Path]::Combine($PSScriptRoot, 'source', 'Alias', 'Alias.csproj'))
try {
	[System.Xml.XmlReader]$xmlReader = [System.Xml.XmlReader]::Create($csproj)
	$xmlReader.ReadToDescendant('Version')
	$version = $xmlReader.ReadElementContentAsString()
} finally {
	$xmlReader.Dispose()
}
dotnet publish -c Release -o release ./source/Alias
Compress-Archive -Path ./release/* -DestinationPath alias.zip
Remove-Item -Path ./release/ -Recurse
wsl ~$env:USERNAME/.local/bin/hub release create -a alias.zip -m $version v$version
