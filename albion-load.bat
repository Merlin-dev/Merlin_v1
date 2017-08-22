@echo off

SET AssemblyPath=Albion\Release
SET AssemblyName=Merlin

SET Target=Albion-Online.exe

SET LoadingAssembly=%AssemblyPath%\%AssemblyName%.dll

injector -dll %LoadingAssembly% -target %Target% -namespace %AssemblyName% -class Core -method Load

