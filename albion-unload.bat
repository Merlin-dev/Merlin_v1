@echo off

echo Unoading
mono-assembly-injector -dll Albion\Release\Merlin-unload.dll -target Albion-Online.exe -namespace Merlin -class Core -method Unload