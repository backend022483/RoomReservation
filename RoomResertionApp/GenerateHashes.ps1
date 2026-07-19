$passwords = @("guest123", "rec123", "agent123", "manager123", "M0ig3l0L@g1n2009")

foreach ($password in $passwords) {
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($password)
    $hash = $sha256.ComputeHash($bytes)
    $base64 = [System.Convert]::ToBase64String($hash)
    Write-Host "'$password' => '$base64'"
}
