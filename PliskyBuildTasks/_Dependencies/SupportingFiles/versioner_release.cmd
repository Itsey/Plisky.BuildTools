@echo off
set srcFolder=..\TaskPackage\buildtask\
set binFolder=..\..\PliskyVer\bin\Debug\

::set dstFolder=D:\Temp\_DelWorking\PliskyVersioner\
set dstFolder2=D:\temp\_DelWorking\Plisky_VersionerTask_TempUpload\

echo deleting %dstFolder2%
del %dstFolder2%

xcopy %srcFolder%*.* %dstFolder2%*.* /s /y
xcopy %binFolder%*.* %dstFolder2%*.* /s /y


::xcopy %srcFolder%\buildtask\*.json %dstFolder2%\*.* /s /y
::xcopy %binFolder%*.* %dstFolder2%\*.* /s /y
::xcopy %srcFolder%\scripts\*.* %dstFolder2%\*.* /s /y
pushd %dstFolder2%
cd..

tfx build tasks upload --task.path ./Plisky_VersionerTask_TempUpload

popd
