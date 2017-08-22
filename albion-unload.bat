@echo off

SET AssemblyPath=Albion\Release
SET AssemblyName=Merlin

SET Target=Albion-Online.exe

SET UnloadAssembly=%AssemblyPath%\%AssemblyName%-unload.dll

echo Unloading
if exist %UnloadAssembly% injector -dll %UnloadAssembly% -target %Target% -namespace %AssemblyName% -class Core -method Unload
if exist %UnloadAssembly% del %UnloadAssembly%