@echo off

SET AssemblyPath=Albion\Release
SET AssemblyName=Merlin

SET Target=Albion-Online.exe

SET LoadingAssembly=%AssemblyPath%\%AssemblyName%.dll

echo Unloading
if exist %UnloadAssembly% injector -dll %UnloadAssembly% -target %Target% -namespace %AssemblyName% -class Core -method Unload

echo Loading
if exist %UnloadAssembly% del %UnloadAssembly%

copy /y %LoadingAssembly% %UnloadAssembly%

injector -dll %LoadingAssembly% -target %Target% -namespace %AssemblyName% -class Core -method Load

