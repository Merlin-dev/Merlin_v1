@echo off

echo Unloading
injector -dll Albion\Release\Merlin-unload.dll -target Albion-Online.exe -namespace Merlin -class Core -method Unload
del Albion\Release\Merlin-unload.dll
