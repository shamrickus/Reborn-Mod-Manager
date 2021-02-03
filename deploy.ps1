
$contains = @("true", "false")
$projs = @("RebornModManager-GUI", "RMM")
for($i = 0; $i -lt $contains.length; $i++) {
    $contain=$contains[$i]
    for($j = 0; $j -lt $projs.length; $j++) {
        $proj=$projs[$j]
        $str=""
        if ($contain -eq "true") {
            $str="-Big"
        } 
        dotnet publish -r win-x64 -c Release -o "bin/windows/$proj$str" --self-contained $contain "./$proj/$proj.csproj"
        Remove-Item "bin/windows/$proj$str/VPK/linux" -Recurse
        Write-Host("Built, Compressing...")
        Compress-Archive -LiteralPath "bin/windows/$proj$str" -DestinationPath "bin/windows/$proj$str.zip" -Force
        Write-Host((Get-Item "bin/windows/$proj$str.zip").length/1MB)
    }
}
