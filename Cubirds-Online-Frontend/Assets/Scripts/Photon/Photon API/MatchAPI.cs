﻿using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using System.Linq;
using UnityEngine;

/// <summary>
/// 与后台的匹配相关接口连接的 API 工具类
/// </summary>
public static class MatchAPI
{
    /// <summary>
    /// 获取所有桌子的信息
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void GetAllTablesInfos(Action<List<TableInfoDTO>> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.GET_ALL_TABLES_INFOS,
            new Dictionary<byte, object>(),
            SendOptions.SendReliable,
            response =>
            {
                // 取出所有的数据，转为 List
                List<object> objectList = response.Parameters.Get<object[]>(ResponseParamaterKey.TABLES_INFOS).ToList();

                // 转为桌子信息
                List<TableInfoDTO> tables = objectList.Select(o => new TableInfoDTO(o as object[])).ToList();

                // 执行回调
                handler.Invoke(tables);
            },
            timeoutHandler);
    }

    /// <summary>
    /// 开一个新的桌子
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void CreateTable(string tableName, Action<TableInfoDTO> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.CREATE_TABLE,
            new Dictionary<byte, object>() {
                { (byte)RequestParamaterKey.TABLE_NAME, tableName },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(new TableInfoDTO(response.Parameters.Get<object[]>(ResponseParamaterKey.TABLE_INFO))),
            timeoutHandler);
    }
}
