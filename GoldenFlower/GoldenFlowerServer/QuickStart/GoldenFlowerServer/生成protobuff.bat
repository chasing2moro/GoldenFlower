rem @echo off

@rem =========================路径=========================


@rem curdir为当前目录
@set curdir=%cd%
@echo "current dir:%curdir%"

@rem dstDir获取Unity目录
@cd %curdir%
@cd ..
@cd ..
@cd ..
@set dstDir=%cd%\GoldenFlowerClient\Assets\Script\Framework\CommonShare
@echo "root    dir:%rootdir%"


rd /q /s %dstDir%
md %dstDir%
Xcopy /y "%curdir%\CommonShare" %dstDir% /s /e

pause