# 对于开发者
这个项目分为三个部分：
1. Cubirds-Online-Common，.Net原生前后端通用内容
2. Cubirds-Online-Frontend，Unity 的客户端
3. Cubirds-Online-Backend，Photon5 的服务端

## Cubirds-Online-Common
前后端的通用数据，不需要引用包或其他项目，也不需要进行修改。

## Cubirds-Online-Frontend
Unity 客户端

### 需要的资源（全部放入 Assets/Plugins）：
1. DoTween 1.2.632，包括 DoTween 的 Resources 文件夹也放入 Plugins
2. TextMeshPro 3.0.6（Unity 自带的包，不是资源商店里的。这个资源可能不是必须的，因为在一次更新中删掉了所有用到的地方，但不确定是否在其他地方有隐式使用）
3. PUN2

### 需要的 DLL：
1. Cubirds-Online-Common 编译出来的 dll，放在 Assets/Plugins/Common 文件夹里

## Cubirds-Online-Backend
Photon5 服务端

### 需要引用的 DLL：
1. Photon.SocketServer.dll，来自 Photon文件夹\lib\PhotonSocketServer\net461\Photon.SocketServer.dll
2. ExitGames.Logging.dll，来自从 Photon5 的可运行服务端的编译后文件里复粘，官方提供了 API 文档那就应该是可以用的
3. ExitGames.Logging.Log4Net.dll，同上一个 ExitGames 的 dll
4. ExitGamesLibs.dll，同上一个 ExitGames 的 dll

### 编译时不需要运行需要的、可以通过 NuGet 包管理器添加的引用：<br/>（这些 dll 可能在 NuGet 包管理器里显示已安装但在引用列表里没有显示，此时需要卸载这些包重新安装）
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

### 编译时需要的、可以通过 NuGet 包管理器添加的引用：
1. log4net，2.0.12.0 版（用新版开机会报异常，可能是不同版本的配置方式不同）

### 需要依赖的项目：
1. Cubirds-Online-Common

### 发布时需要的步骤
1. 找到 PhotonServer.config 文件，这个不是后台需要的文件，是给 Photon5 的配置，需要添加到 Photon5 的配置里
2. 把编译出来的文件放进 Photon5 的部署文件夹里