# 比較エンジンの回帰テスト
# ハーネスで標準ケースを実行し、全レコードダンプをtools/baselines/の正解データと突き合わせる
#
# 使い方:
#   .\tools\run-regression.ps1                  # 照合(PASS/FAIL)
#   .\tools\run-regression.ps1 -UpdateBaseline  # 意図した挙動変更後にベースラインを更新
#
# 注意: 意図した挙動変更でFAILした場合は、fcコマンド等で差分内容を確認してから
#       -UpdateBaseline でベースラインを更新すること(無確認の更新は回帰検出を無効化する)

param([switch]$UpdateBaseline)

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent

dotnet build "$root\tools\DiffCheckerHarness\DiffCheckerHarness.csproj" -c Release --nologo -v q
if ($LASTEXITCODE -ne 0) { Write-Host "BUILD FAILED"; exit 1 }
$exe = "$root\tools\DiffCheckerHarness\bin\Release\net8.0\DiffCheckerHarness.exe"

$cases = @(
    @{ Name = '202_testdata'; Mode = '202'; A = "$root\TestData\FileA.stb";         B = "$root\TestData\FileB.stb";         Tol = '0' },
    @{ Name = '210_mini';     Mode = '210'; A = "$root\TestData\Mini210_FileA.stb"; B = "$root\TestData\Mini210_FileB.stb"; Tol = '100' }
)

$baselineDir = "$root\tools\baselines"
if (-not (Test-Path $baselineDir)) { New-Item -ItemType Directory $baselineDir | Out-Null }

$fail = $false
foreach ($case in $cases) {
    $dump = Join-Path $env:TEMP ("stbdiff_regression_" + $case.Name + ".tsv")
    & $exe $case.Mode $case.A $case.B $case.Tol $dump | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: harness failed for $($case.Name)"
        $fail = $true
        continue
    }

    $baseline = Join-Path $baselineDir ($case.Name + '.tsv')
    if ($UpdateBaseline) {
        Copy-Item $dump $baseline -Force
        Write-Host "UPDATED: $($case.Name)"
        continue
    }

    if (-not (Test-Path $baseline)) {
        Write-Host "MISSING BASELINE: $($case.Name) (-UpdateBaseline で作成)"
        $fail = $true
        continue
    }

    $actual = (Get-FileHash $dump).Hash
    $expected = (Get-FileHash $baseline).Hash
    if ($actual -eq $expected) {
        Write-Host "PASS: $($case.Name)"
    }
    else {
        Write-Host "FAIL: $($case.Name)"
        Write-Host "  差分確認: fc `"$baseline`" `"$dump`""
        $fail = $true
    }
}

if ($fail) { exit 1 } else { Write-Host "ALL PASS"; exit 0 }
