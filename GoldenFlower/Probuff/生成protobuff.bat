rem @echo off

@rem =========================路径=========================


@rem curdir为当前目录
@set curdir=%cd%
@echo "current dir:%curdir%"

@rem rootdir获取根目录
@cd %curdir%
@cd ..
@set rootdir=%cd%
@echo "root    dir:%rootdir%"

@rem *.proto文件的目录
@set protodir=%curdir%\proto

@rem *.proto -> *.C# 的工具目录
@set protogendir=%curdir%\ProbuffStuff\ProtoGen

@rem ProbuffProtocol工程目录
@set protoprotocol=%curdir%\ProbuffProtocol

@rem MSBuild 目录
@set msbuilddir=C:\Windows\Microsoft.NET\Framework\v2.0.50727

@rem PreCompile目录
@set precompiledir=%curdir%\ProbuffStuff\Precompile

@rem Unity plugins 目录
@set unitypluginsdri=%rootdir%\GoldenFlowerClient\Assets\Plugins\Probuff

@rem =========================工具=========================
rem =============*.proto copy to to gen dir=============
copy /y "%protodir%\defaultproto.proto" %protogendir%

rem =============name space shot to:defaultproto=============
cd %protogendir%
rem =============gen dir's *.proto generate *.cs file , then put to VS project=============
protogen.exe -i:"%protogendir%\defaultproto.proto" -o:"%protoprotocol%\defaultproto.cs"

rem =============VS project's *.cs generate *.dll=============
"%msbuilddir%\MSBuild.exe" "%protoprotocol%\ProbuffProtocol.csproj" /t:Rebuild /p:Configuration=Release

rem =============*.dll generate serailize *.dll=============
"%precompiledir%\precompile.exe" "%protoprotocol%\bin\Release\ProbuffProtocol.dll" -o:"%protoprotocol%\bin\Release\ProbuffProtocolSerializer.dll" -t:ProbuffProtocolSerializer

rem =============*.dll  serailize *.dll copy to Unity Plugins=============
copy /y "%protoprotocol%\bin\Release\ProbuffProtocol.dll" %unitypluginsdri%
copy /y "%protoprotocol%\bin\Release\ProbuffProtocolSerializer.dll" %unitypluginsdri%
if not exist "%unitypluginsdri%\protobuf-net.dll" copy /y "%protoprotocol%\bin\Release\protobuf-net.dll" %unitypluginsdri%

pause