# 客户端开发者手册

玩家使用的客户端，位于 Cubirds-Online-Frontend 文件夹中，基于 Unity3D 开发。

## 需要的资源（全部放入 Assets/Plugins）：
1. DoTween 1.2.632，包括 DoTween 的 Resources 文件夹也放入 Plugins
2. TextMeshPro 3.0.6（Unity 自带的包，不是资源商店里的。这个资源可能不是必须的，因为在一次更新中删掉了所有用到的地方，但不确定是否在其他地方有隐式使用）
3. PUN2

## 需要的 DLL：
1. Cubirds-Online-Common 编译出来的 dll，放在 Assets/Plugins/Common 文件夹里