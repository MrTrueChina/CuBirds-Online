# 游戏服务器开发者手册

游戏服务器部分位于 Cubirds-Online-Backend 文件夹，使用 PhotonServer 开发，对游戏主要功能提供服务。

## 需要引用的 DLL：
1. Photon.SocketServer.dll，来自 Photon文件夹\lib\PhotonSocketServer\net461\Photon.SocketServer.dll
2. ExitGames.Logging.dll，来自从 Photon5 的可运行服务端的编译后文件里复粘，官方提供了 API 文档那就应该是可以用的
3. ExitGames.Logging.Log4Net.dll，同上一个 ExitGames 的 dll
4. ExitGamesLibs.dll，同上一个 ExitGames 的 dll

## 编译时不需要运行需要的、可以通过 NuGet 包管理器添加的引用：<br/>（这些 dll 可能在 NuGet 包管理器里显示已安装但在引用列表里没有显示，此时需要卸载这些包重新安装）
1. Microsoft.Extensions.Caching.Abstractions 6.0.0 版
2. Microsoft.Extensions.Caching.Memory 6.0.0 版
3. Microsoft.Extensions.Configuration，6.0.0 版
4. Microsoft.Extensions.Configuration.Abstractions，6.0.0 版
5. Microsoft.Extensions.Configuration.Binder，6.0.0 版
6. Microsoft.Extensions.Configuration.FileExtensions 6.0.0 版
7. Microsoft.Extensions.Configuration.Xml，6.0.0 版
8. Microsoft.Extensions.FileProviders.Abstractions 6.0.0 版
9. Microsoft.Extensions.FileProviders.Physical 6.0.0 版
10. Microsoft.Extensions.FileSystemGlobbing 6.0.0 版
11. Microsoft.Extensions.Primitives，6.0.0 版

## 编译时需要的、可以通过 NuGet 包管理器添加的引用：
1. log4net，2.0.12.0 版（用新版开机会报异常，可能是不同版本的配置方式不同）

## 需要依赖的项目：
1. Cubirds-Online-Common
