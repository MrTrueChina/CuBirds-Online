Photon.SocketServer.dll 来自 Photon文件夹\lib\PhotonSocketServer\net461\Photon.SocketServer.dll
ExitGames 的三个 dll 来自复粘，可能在部分情况下可以在 NuGet 包管理器中搜索到

此外需要从 NuGet 包管理器下载，需要注意可能有些显示已安装但其实没安装，要看引用列表里是不是真的有：

Photon5 需要的引用：
Microsoft.Extensions.Caching.Abstractions 6.0.0 版
Microsoft.Extensions.Caching.Memory 6.0.0 版
Microsoft.Extensions.Configuration，6.0.0 版
Microsoft.Extensions.Configuration.Abstractions，6.0.0 版
Microsoft.Extensions.Configuration.Binder，6.0.0 版
Microsoft.Extensions.Configuration.FileExtensions 6.0.0 版
Microsoft.Extensions.Configuration.Xml，6.0.0 版
Microsoft.Extensions.FileProviders.Abstractions 6.0.0 版
Microsoft.Extensions.FileProviders.Physical 6.0.0 版
Microsoft.Extensions.FileSystemGlobbing 6.0.0 版
Microsoft.Extensions.Primitives，6.0.0 版

当然需要的通用内容：
log4net，这里用的是 2.0.12.0 版