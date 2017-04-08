@echo off
set srcFolder=D:\Files\Git2015\Plisky.net\PliskyVersioning\_Dependencies\BuildTask\
set dstFolder=D:\Temp\PliskyVerisoningTemp\TestFolder\
set binFolder=D:\Files\Git2015\Plisky.net\PliskyVersioning\PliskyVersioningSupport\bin\Debug\

xcopy %srcFolder%*.* %dstFolder%*.* /s /y
xcopy %binFolder%*.* %dstFolder%*.* /s /y
