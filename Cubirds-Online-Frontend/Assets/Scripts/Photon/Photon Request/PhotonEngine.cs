using System;
using System.Linq;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using UnityEngine;

/// <summary>
/// 与 Photon 后台通信的脚本
/// </summary>
public partial class PhotonEngine : MonoBehaviour, IPhotonPeerListener
{
    /// <summary>
    /// 这个通信脚本的实例
    /// </summary>
    public static PhotonEngine Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(PhotonEngine))
            {
                if (instance == null)
                {
                    // 生成挂载实例的物体
                    GameObject instanceObject = new GameObject("Photon Engine");

                    // 挂载组件
                    instance = instanceObject.AddComponent<PhotonEngine>();

                    // 设为不随场景加载销毁
                    DontDestroyOnLoad(instanceObject);
                }

                return instance;
            }
        }
    }
    private static PhotonEngine instance;

    /// <summary>
    /// Photon 的客户端实例
    /// </summary>
    public static PhotonPeer Peer { get; private set; }

    /// <summary>
    /// 进行连接
    /// </summary>
    /// <param name="serverAddress">，服务器地址，可以用 ip:端口号，也可以用网址</param>
    /// <param name="serverId">服务器ID（就是配置里的名称，不是显示名）</param>
    public void StartConnect(string serverAddress, string serverId)
    {
        Debug.LogFormat("连接 {0} 的服务器 {1}", serverAddress, serverId);

        // 创建客户端实例，参数是负责接收消息的类、连接方式
        Peer = new PhotonPeer(Instance, ConnectionProtocol.Udp);

        // 进行连接
        Peer.Connect(serverAddress, serverId);
    }

    private void Update()
    {
        // 如果没有客户端实例，说明还没有连接，不处理
        if(Peer == null)
        {
            return;
        }

        //Debug.Log(Peer.PeerState);

        // 好像是维持服务就需要在 Update 里面调用这个方法，可能这个方法类似于我写的那些“监听”，其实是每一帧对比一次数据
        Peer.Service();

        // 检测是否有请求超时
        CheckRequestTimeout();
    }

    public void Disconnect()
    {
        Debug.Log("断开连接");

        // 让客户端断开连接
        Peer.Disconnect();

        // 移除客户端
        Peer = null;
    }

    private void OnDestroy()
    {
        // 如果 Photon 客户端存在，并且已连接或正在连接，不确定正在连接是不是也需要处理但为了保险加上了
        if (Peer != null && (Peer.PeerState == PeerStateValue.Connected || Peer.PeerState == PeerStateValue.Connecting))
        {
            // 断开连接
            Peer.Disconnect();
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        Debug.Log("OnStatusChanged");
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }
}
