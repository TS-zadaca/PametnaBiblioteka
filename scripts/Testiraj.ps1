$ErrorActionPreference = 'Stop'
$korijen = Split-Path -Parent $PSScriptRoot
Push-Location $korijen
try {
    $vrijeme = Get-Date -Format 'yyyyMMdd-HHmmss-fff'
    $rezultati = Join-Path $korijen "TestResults/$vrijeme"
    & dotnet test PametnaBiblioteka.sln --configuration Release -m:1 --disable-build-servers --collect:'XPlat Code Coverage' --results-directory $rezultati --logger 'trx;LogFileName=rezultati.trx'
    if ($LASTEXITCODE -ne 0) { throw 'Build ili testovi nisu prosli.' }

    $datoteke = @(Get-ChildItem $rezultati -Filter coverage.cobertura.xml -Recurse)
    if ($datoteke.Count -eq 0) { throw 'Nije pronadjen izvjestaj o pokrivenosti.' }
    [xml]$xml = Get-Content $datoteke[0].FullName -Raw
    $kultura = [System.Globalization.CultureInfo]::InvariantCulture
    $linije = [double]::Parse([string]$xml.coverage.'line-rate', $kultura) * 100
    $grane = [double]::Parse([string]$xml.coverage.'branch-rate', $kultura) * 100
    Write-Host ('Pokrivenost linija: {0:F2}%; grana: {1:F2}%' -f $linije, $grane)
    Write-Host "Rezultati: $rezultati"
    if ($linije -lt 80 -or $grane -lt 80) { throw 'Pokrivenost mora biti najmanje 80% i za linije i za grane.' }
    Write-Host 'Svi testovi i prag pokrivenosti su zadovoljeni.'
}
finally { Pop-Location }
