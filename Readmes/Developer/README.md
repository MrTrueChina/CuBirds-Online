# 开发者手册

对于开发者，这个项目分为四个主要部分：
1. Cubirds-Online-Common，[.Net原生前后端通用内容](README_COMMON.md)
2. Cubirds-Online-Frontend，[Unity 的客户端](README_FRONTEND.md)
3. Cubirds-Online-Backend，[Photon5 的服务端](README_BACKEND.md)
4. Cubirds-Online-Hot-Update-Backend，[SpringBoot 的服务端](README_HOT_UPDATE_SERVER.md)

除了上述主要代码内容外还有数据库和 Postman 两个部分。

## 数据库部分

数据库位于 Deploy/数据库/数据库结构.sql，直接导入即可。

需要注意热更新服务器使用的是 PostgreSQL 服务器，如果使用其他服务器需要根据情况进行修改。

## Postman 部分

Postman 有一个环境和一套请求，都在 Deploy/Postman 中，分别导入即可。

Postman 在项目中起到上传热更新包的作用，因为只有这一个接口需要调用并没有额外制作前端。

热更新后台使用了 CSRF 验证，因此 Postman 在上传热更新包之前需要先调用 Check Update 接口获取 Token，之后才可以上传热更新包。

上传热更新包通过 Upload Hot Update AssetBundles 接口进行，在 Params 中填写资源包的名称，在 Body 中选择资源包文件和 manifest 文件，发出请求后即可上传热更新包。
