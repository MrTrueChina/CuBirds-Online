# 游戏服务器部署手册

游戏服务器是基于 PhotonServer 开发的，需要按照 Photon 的方式部署。

## 获取 PhotonServer

在 Photon 官方获取 PhotonServer https://www.photonengine.com/en-US/sdks#server-sdkserverserver，解压后放到合适的位置。

Photon 免费提供较少的连接数量，如果需要更多的连接数量请自行获取更高级的许可证。

## 编译

只要普通的编译方式就可以。

## 将编译后的文件复制到发布文件夹下

将变异出的文件放到 Photon 的 deploy 文件夹下，具体位置为：deploy/你喜欢的文件夹名称/bin（编译出的 bin 目录）

## 修改 PhotonServer 的配置文件

找到 PhotonServer.config 文件，找到服务器项目目录中的 PhotonServer.config，按照文件内的描述把需要复制的内容复制到 PhotonServer 的 PhotonServer.config 文件中。

## 启动

配置完毕后启动 Photon，此时应当可以看到刚刚配置的服务器，选择启动即可。
