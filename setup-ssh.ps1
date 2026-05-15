# 管理者 PowerShell で実行してください
# > .\setup-ssh.ps1

Write-Host "=== OpenSSH サーバー セットアップ ===" -ForegroundColor Cyan

# 1. インストール確認・インストール
$cap = Get-WindowsCapability -Online -Name OpenSSH.Server~~~~0.0.1.0
if ($cap.State -ne 'Installed') {
    Write-Host "OpenSSH Server をインストールしています..." -ForegroundColor Yellow
    Add-WindowsCapability -Online -Name OpenSSH.Server~~~~0.0.1.0 | Out-Null
    Write-Host "インストール完了" -ForegroundColor Green
} else {
    Write-Host "OpenSSH Server はインストール済みです" -ForegroundColor Green
}

# 2. サービス起動・自動起動設定
Start-Service sshd
Set-Service -Name sshd -StartupType Automatic
Write-Host "sshd サービスを起動しました（自動起動 ON）" -ForegroundColor Green

# 3. ファイアウォール（既存ルールがなければ追加）
if (-not (Get-NetFirewallRule -Name 'sshd' -ErrorAction SilentlyContinue)) {
    New-NetFirewallRule -Name sshd -DisplayName 'OpenSSH Server (sshd)' `
        -Enabled True -Direction Inbound -Protocol TCP -Action Allow -LocalPort 22 | Out-Null
    Write-Host "ファイアウォール ポート 22 を開放しました" -ForegroundColor Green
} else {
    Write-Host "ファイアウォールルールは既に存在します" -ForegroundColor Green
}

# 4. 接続情報を表示
$ip = (Get-NetIPAddress -AddressFamily IPv4 |
    Where-Object { $_.InterfaceAlias -notlike '*Loopback*' -and $_.IPAddress -notlike '169.*' } |
    Select-Object -First 1).IPAddress

Write-Host ""
Write-Host "=== スマホからの接続情報 ===" -ForegroundColor Cyan
Write-Host "ホスト     : $ip" -ForegroundColor White
Write-Host "ポート     : 22" -ForegroundColor White
Write-Host "ユーザー名 : $env:USERNAME" -ForegroundColor White
Write-Host "パスワード : Windows のログインパスワード" -ForegroundColor White
Write-Host ""
Write-Host "推奨SSHアプリ: Termius（iOS/Android、無料）" -ForegroundColor Yellow
Write-Host "接続後に 'claude' と入力するとセッション開始できます" -ForegroundColor Yellow
