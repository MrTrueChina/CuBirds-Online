using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using UnityEngine;

/// <summary>
/// 与后台的用户相关接口连接的 API 工具类
/// </summary>
public static class PlayerAPI
{
    /// <summary>
    /// 获取本机用户的 ID
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void GetLocalPlayerId(Action<int> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.GET_PLAYER_ID,
            new Dictionary<byte, object>(),
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<int>(ResponseParamaterKey.PLAYER_ID)),
            timeoutHandler);
    }

    /// <summary>
    /// 设置玩家名称
    /// </summary>
    /// <param name="name"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void SetPLayerName(string name, Action<bool> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.SET_PLAYER_NAME,
            new Dictionary<byte, object>() {
                { (byte)RequestParamaterKey.PLAYER_NAME, name },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }
}
