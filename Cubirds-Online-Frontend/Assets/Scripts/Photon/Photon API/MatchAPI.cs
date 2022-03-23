using System;
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
    /// <param name="password"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void CreateTable(string tableName, string password, Action<TableInfoDTO> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.CREATE_TABLE,
            new Dictionary<byte, object>() {
                { (byte)RequestParamaterKey.TABLE_NAME, tableName },
                { (byte)RequestParamaterKey.TABLE_PASSWORD, password },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(new TableInfoDTO(response.Parameters.Get<object[]>(ResponseParamaterKey.TABLE_INFO))),
            timeoutHandler);
    }

    /// <summary>
    /// 加入桌子
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="password"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void JoinTable(int tableId, string password, Action<ParameterDictionary> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.JOIN_TABLE,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, tableId },
                // 密码
                { (byte)RequestParamaterKey.TABLE_PASSWORD, password },
            },
            SendOptions.SendReliable,
            response => {
                // 这个处理比较麻烦，不如直接把参数传给回调
                handler.Invoke(response.Parameters);
            },
            timeoutHandler);
    }

    /// <summary>
    /// 获取一个桌子上的所有玩家
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void GetAllPlayersOnTable(int tableId, Action<List<PlayerInfoDTO>> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.GET_ALL_PLAYER_ON_TABLE,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, tableId },
            },
            SendOptions.SendReliable,
            response =>
            {
                // 获取返回
                object[] playersArray = response.Parameters.Get<object[]>(ResponseParamaterKey.PLAYERS_INFOS);

                // 转为玩家信息列表
                List<PlayerInfoDTO> players = playersArray.ToList().Select(a => new PlayerInfoDTO(a as object[])).ToList();

                // 执行回调
                handler.Invoke(players);
            },
            timeoutHandler);
    }

    /// <summary>
    /// 退出桌子
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void QuitTable(int tableId, Action<bool> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.QUIT_TABLE,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, tableId },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 解散桌子
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void DisbandTable(int tableId, Action<bool> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.DISBAND_TABLE,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, tableId },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void StartGame(int tableId, Action<bool> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.START_GAME,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, tableId },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void GameEnd(int tableId, Action<bool> handler, Action timeoutHandler = null)
    {
        PhotonEngine.SendOperation(
            RequestCode.GAME_END,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, tableId },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }
}
