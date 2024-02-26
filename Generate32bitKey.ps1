$random = [System.Security.Cryptography.RandomNumberGenerator]::Create();
$buffer = New-Object byte[] 32;
$random.GetBytes($buffer);

$key = [Convert]::ToBase64String($buffer)
Write-Output $key
Set-Clipboard -Value $key

Write-Output "Copied to clipboard"
Read-Host -Prompt "Press Enter to exit"