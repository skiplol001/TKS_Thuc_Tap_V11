# TKS Auto Setup 
$ErrorActionPreference = 'Stop'
function Ask($m) { Write-Host $m -ForegroundColor Yellow; (Read-Host) -match '^[Yy]' }
function OK($m) { Write-Host "  OK: $m" -ForegroundColor Green }
function Warn($m) { Write-Host "  !!: $m" -ForegroundColor Yellow }
function Err($m) { Write-Host "  XX: $m" -ForegroundColor Red }

function Refresh-Path {
    $env:Path = [System.Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [System.Environment]::GetEnvironmentVariable('Path','User')
}

function Install-Pkg {
    param([string]$Pkg, [string]$Name)
    if (-not (Ask "Cai $Name bay gio? (Y/n)")) {
        Warn "Bo qua $Name"
        return $false
    }
    Write-Host "  Dang cai $Name (co the mat 2-5 phut)..." -ForegroundColor Cyan
    winget install $Pkg --accept-source-agreements --accept-package-agreements
    if ($LASTEXITCODE -eq 0) { OK "$Name da cai"; return $true }
    Err "$Name cai that bai (exit $LASTEXITCODE)"
    return $false
}

$Sln = 'TKS_Thuc_Tap_V11.sln'
$Web = 'TKS_Thuc_Tap_V11_Web'
$Db = 'TKS_Thuc_Tap_V11'
$Key = 'ConnectionStrings:TKS_Thuc_Tap_V11_Conn_String'

Write-Host ''
Write-Host '=== TKS_Thuc_Tap_V11 Auto Setup ===' -ForegroundColor Cyan
Write-Host ''

$R = (Get-Location).Path
if (-not (Test-Path "$R\$Sln")) {
    Warn "Khong tim thay $Sln trong: $R"
    $R = Read-Host 'Nhap duong dan project'
    if (-not (Test-Path "$R\$Sln")) { Err 'Van khong tim thay. Dung.'; return }
    Set-Location $R
}
OK "Project: $R"

# --- 1/8: .NET SDK >= 8 ---
$needRestart = $false
if (-not (Get-Command dotnet -EA 0)) {
    Err '.NET SDK chua cai'
    if (Install-Pkg 'Microsoft.DotNet.SDK.8' '.NET 8 SDK') {
        Refresh-Path
        $needRestart = $true
    }
}
if (Get-Command dotnet -EA 0) {
    $sdks = dotnet --list-sdks 2>$null
    $hasSdk = $false
    $foundVer = ''
    foreach ($line in $sdks) {
        if ($line -match '^(\d+)\.') {
            if ([int]$matches[1] -ge 8) { $hasSdk = $true; $foundVer = $line; break }
        }
    }
    if (-not $hasSdk) {
        Err '.NET SDK >= 8 chua cai'
        if (Install-Pkg 'Microsoft.DotNet.SDK.8' '.NET 8 SDK') {
            Refresh-Path
            $needRestart = $true
        }
    } else { OK ".NET SDK $($foundVer -replace '\s.*','')" }
}
if ($needRestart) {
    Write-Host ''
    Warn 'Vua cai SDK. Dong terminal nay, mo terminal moi, chay lai script.'
    return
}

# --- 2/8: .NET 6 Runtime ---
if (Get-Command dotnet -EA 0) {
    $rt6 = (dotnet --list-runtimes 2>$null) | Where-Object { $_ -match 'Microsoft\.NETCore\.App 6\.' }
    if (-not $rt6) {
        Warn '.NET 6 Runtime thieu (can cho Data_Access + Report)'
        if (Install-Pkg 'Microsoft.DotNet.Runtime.6' '.NET 6 Runtime') {
            Refresh-Path
        }
    } else { OK '.NET 6 Runtime' }
}

# --- 3/8: SQL Server ---
$sqlOk = $false; $srv = ''
if (Get-Command sqllocaldb -EA 0) {
    $ll = & sqllocaldb info 2>$null
    if ($ll) { $sqlOk = $true; $srv = '(localdb)\MSSQLLocalDB'; OK "LocalDB: $ll" }
}
if (-not $sqlOk) {
    $svc = @(Get-Service 'MSSQL*' -EA 0)
    if ($svc) { $sqlOk = $true; $srv = 'localhost'; OK "SQL Service: $($svc[0].Name)" }
}
if (-not $sqlOk) {
    Warn 'SQL Server chua cai'
    if (Ask 'Cai SQL Server LocalDB + Express bay gio? (Y/n)') {
        winget install Microsoft.SQLServer.2022.LocalDB --accept-source-agreements --accept-package-agreements
        winget install Microsoft.SQLServer.2022.Express --accept-source-agreements --accept-package-agreements
        Refresh-Path
        if (Get-Command sqllocaldb -EA 0) {
            $ll = & sqllocaldb info 2>$null
            if ($ll) { $sqlOk = $true; $srv = '(localdb)\MSSQLLocalDB'; OK "LocalDB: $ll" }
        }
        if (-not $sqlOk) { Warn 'SQL cai xong nhung chua nhan. Co the can restart may.' }
    } else { Warn 'Bo qua SQL Server - ban se phai cau hinh tay sau' }
}

# --- 4/8: Telerik NuGet source ---
$srcs = dotnet nuget list source 2>$null
if (-not ($srcs -match 'nuget\.telerik\.com')) {
    if (Ask 'Them Telerik NuGet source? (Y/n)') {
        dotnet nuget add source 'https://nuget.telerik.com/v3/index.json' --name Telerik 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) { OK 'Telerik NuGet' } else { Warn 'Them fail - chay PowerShell Admin' }
    }
} else { OK 'Telerik NuGet da co' }

# --- 5/8: Restore + Build ---
Write-Host ''
Write-Host 'Restore...' -ForegroundColor Gray
dotnet restore $Sln
if ($LASTEXITCODE -ne 0) { Err 'Restore fail - check Telerik credentials / .NET 6 SDK'; return }
OK 'Restore'

Write-Host 'Build...' -ForegroundColor Gray
dotnet build $Sln --no-restore -c Debug
if ($LASTEXITCODE -ne 0) { Err 'Build fail - xem log ben tren'; return }
OK 'Build'

# --- 6/8: User Secrets ---
Push-Location (Join-Path $R $Web)
dotnet user-secrets init 2>&1 | Out-Null
if ($srv) { $defConn = "Server=$srv;Database=$Db;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;" }
else { $defConn = "Server=localhost;Database=$Db;User Id=sa;Password=YOUR_PASSWORD;Encrypt=False;" }
Write-Host ''
Write-Host "Default conn: $defConn" -ForegroundColor Gray
$conn = Read-Host 'Connection string (Enter=dung default)'
if (-not $conn) { $conn = $defConn }
dotnet user-secrets set $Key $conn 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) { OK 'User Secrets' } else { Warn 'Set User Secrets fail' }
Pop-Location

# --- 7/8: Telerik License ---
Write-Host ''
Write-Host 'Telerik license key (Enter=skip):' -ForegroundColor Yellow
$tk = Read-Host 'Key'
if ($tk) {
    [Environment]::SetEnvironmentVariable('TELERIK_LICENSE', $tk, 'User')
    $env:TELERIK_LICENSE = $tk
    OK 'TELERIK_LICENSE da luu (mo terminal moi de apply)'
} else { Warn 'Skip license - set sau bang: $env:TELERIK_LICENSE = "..."' }

# --- 8/8: Database ---
if ($sqlOk) {
    try {
        Add-Type -AssemblyName System.Data
        $mc = "Server=$srv;Database=master;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
        $c = New-Object System.Data.SqlClient.SqlConnection $mc
        $c.Open()
        $cm = $c.CreateCommand()
        $cm.CommandText = "IF NOT EXISTS(SELECT name FROM sys.databases WHERE name=N'$Db') CREATE DATABASE [$Db];"
        $cm.ExecuteNonQuery() | Out-Null
        $c.Close()
        OK "Database $Db san sang"
    } catch { Warn "Tao DB fail: $($_.Exception.Message)" }
} else { Warn 'Skip tao DB - SQL Server chua co' }

# --- Done ---
Write-Host ''
Write-Host '=== HOAN TAT SETUP ===' -ForegroundColor Green
Write-Host ''
Write-Host 'Chay app (chon 1):' -ForegroundColor Yellow
Write-Host "  dotnet run --project $Web"
Write-Host '  Hoac: mo VS -> chuot phai TKS_Thuc_Tap_V11_Web -> Set as Startup -> F5'
Write-Host ''
Write-Host 'Luu y: neu vua set TELERIK_LICENSE, dong terminal hien tai, mo terminal moi truoc khi chay app.' -ForegroundColor DarkYellow