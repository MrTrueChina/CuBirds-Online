using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using UnityEngine;

/// <summary>
/// 与后台的用户相关接口连接的 API 工具类
/// </summary>
public static class UserAPI
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
}
