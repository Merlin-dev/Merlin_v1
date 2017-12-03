@echo off

SET AssemblyPath=Albion\Release
SET AssemblyName=Merlin
SET AssemblyNamespace=Merlin

SET Target=Albion-Online.exe

SET LoadingAssembly=%AssemblyPath%\%AssemblyName%.dll

injector -dll %LoadingAssembly% -target %Target% -namespace %AssemblyNamespace% -class Core -method Load

