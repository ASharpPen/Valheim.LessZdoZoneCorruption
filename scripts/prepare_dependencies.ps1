# Quick'n'dirty NStrip script for preparing dependencies. Expects NStrip.exe placed in same folder. See https://github.com/bbepis/NStrip

# Output directory to place libraries in. 
# Eg., "C:\YOUR-REPO-PATH\src\Libs"
$outputDir = 

# Path to bepinex profile from which to pick up unity files and potentially mods.
# Eg., C:\Users\YOUR-USER\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\SOME-PROFILE
$profilePath = 

# Path to Valheim main folder for getting the game dlls.
$valheimPath = "D:\Games\Steam\steamapps\common\Valheim\valheim_Data\Managed"

function Strip
{
  [CmdletBinding()]
  param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]
    $Source,

    [Parameter(Mandatory=$true, Position=1)]
    [string]
    $TargetDir,

    [Parameter(Mandatory=$false, Position=2)]
    [switch]
    $Publicize
  )

  $out = "$outputDir\$TargetDir"

  if (-not(Test-Path $out))
  {
    New-Item -ItemType Directory -Path $out
  }

  if (Test-Path -Path $Source -PathType Leaf)
  {
    $file = Split-Path $Source -Leaf
    $out = Join-Path $out $file
  }

  if($Publicize) { 
    & .\Nstrip.exe -p -cg -d $valheimPath $Source $out
  }
  else {
    & .\Nstrip.exe -cg -d $valheimPath $Source $out
  }
  
  Write-Host "-Source $Source -TargetDir $out"
}

function CopyFile
{
  [Cmdletbinding()]
  param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]
    $Source,

    [Parameter(Mandatory=$true, Position=1)]
    [string]
    $TargetDir
  )

  $out = "$outputDir\$TargetDir"

  if (-not(Test-Path $out))
  {
    New-Item -ItemType Directory -Path $out
  }

  if (Test-Path -Path $Source -PathType Leaf)
  {
    Copy-Item $Source -Destination $out
  }

  Write-Host "-Source $Source -TargetDir $out"
}

# Valheim
Strip "$valheimPath\assembly_valheim.dll" "Valheim" -Publicize
Strip "$valheimPath\assembly_utils.dll" "Valheim" -Publicize

# Unity
CopyFile "$valheimPath\UnityEngine.dll" "Unity"
CopyFile "$valheimPath\UnityEngine.CoreModule.dll" "Unity"
CopyFile "$valheimPath\UnityEngine.PhysicsModule.dll" "Unity"
CopyFile "$valheimPath\UnityEngine.ImageConversionModule.dll" "Unity"
CopyFile "$valheimPath\UnityEngine.UI.dll" "Unity"

Write-Host "Done"