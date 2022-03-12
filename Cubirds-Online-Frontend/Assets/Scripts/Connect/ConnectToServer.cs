using System.Collections;
using System.Collections.Generic;
using CubirdsOnline.Common;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 启动与后台进行连接的脚本，只负责开始连接，不负责连接后的功能
/// </summary>
public class ConnectToServer : MonoBehaviour
{
    /// <summary>
    /// ID:端口 输入框
    /// </summary>
    [SerializeField]
    [Header("IP:端口 输入框")]
    private InputField ipInputField;
    /// <summary>
    /// 网址输入框
    /// </summary>
    [SerializeField]
    [Header("网址输入框")]
    private InputField netInputField;
    /// <summary>
    /// 连接信息文本组件
    /// </summary>
    [SerializeField]
    [Header("连接信息文本组件")]
    private Text connectInfoText;

    /// <summary>
    /// 连接计时器
    /// </summary>
    private float connectTimer;
    /// <summary>
    /// 是否已经开始连接
    /// </summary>
    private bool connectStarted = false;

    /// <summary>
    /// 使用 IP:端口 进行连接
    /// </summary>
    public void StartConnectByIp()
    {
        // 使用 ip 输入框的内容进行连接
        StartConnect(ipInputField.text);
    }

    /// <summary>
    /// 使用网址进行连接
    /// </summary>
    public void StartConnectByNet()
    {
        // 使用网址输入框的内容进行连接
        StartConnect(netInputField.text);
    }

    /// <summary>
    /// 进行连接
    /// </summary>
    /// <param name="address"></param>
    private void StartConnect(string address)
    {
        PhotonEngine.Instance.StartConnect(address, "Cu-Birds-Online");

        // 记录开始连接
        connectStarted = true;

        // 显示提示
        connectInfoText.text = "连接中……";

        // 给计时器进行连接的时间
        connectTimer = 10;
    }

    private void Update()
    {
        // 如果已经开始连接，进行对连接是否成功的处理
        if (connectStarted)
        {
            // 如果连接状态是已断开，说明连接失败
            if(PhotonEngine.Peer.PeerState == PeerStateValue.Disconnected)
            {
                // 显示提示
                connectInfoText.text = "连接失败";

                // 改为没有开始连接状态
                connectStarted = false;

                // 确认连接失败后就不需要后续操作了
                return;
            }

            // 如果连接状态是已连接，说明连接成功
            if(PhotonEngine.Peer.PeerState == PeerStateValue.Connected)
            {
                // 显示提示
                connectInfoText.text = "连接成功";

                // 向服务器发出获取玩家 ID 请求
                PhotonEngine.SendOperation(RequestCode.GET_PLAYER_ID, new Dictionary<byte, object>(), SendOptions.SendUnreliable, getPlayerIdResponse =>
                {
                    // 获取 ID
                    int localPlayerId = getPlayerIdResponse.Parameters.Get<int>(ResponseParamaterKey.PLAYER_ID);

                    Debug.LogFormat("获取本地玩家 ID = {0}", localPlayerId);

                    // 保存本机玩家 ID
                    GlobalModel.Instance.LocalPLayerId = localPlayerId;
                });

                // 改为没有开始连接状态
                connectStarted = false;

                // 确认连接成功后就不需要后续操作了
                return;
            }

            // 对连接进行计时
            connectTimer -= Time.deltaTime;

            // 计时结束
            if(connectTimer <= 0)
            {
                // 显示提示
                connectInfoText.text = "连接超时";

                // 改为没有开始连接状态
                connectStarted = false;
            }
        }
    }
}
