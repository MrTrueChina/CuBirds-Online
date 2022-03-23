using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using UnityEngine;

/// <summary>
/// 与后台的帧同步相关接口连接的 API 工具类
/// </summary>
public static class LockstepAPI
{
    /// <summary>
    /// 同步玩家打出牌操作
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="centerLineIndex"></param>
    /// <param name="putOnLeft"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void LockStepPlayerPlayCards(CardType cardType, int centerLineIndex, bool putOnLeft, Action<bool> handler = null, Action timeoutHandler = null)
    {
        // 补充成功回调，防止空异常
        if (handler == null)
        {
            handler = success => { };
        }

        // 发出请求
        PhotonEngine.SendOperation(
            RequestCode.LOCK_STEP_PLAYER_PLAY_CARDS,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, GlobalModel.Instance.TableInfo.Id },
                // 卡牌种类
                { (byte)RequestParamaterKey.CARD_TYPE, (int)cardType },
                // 打到的中央行的索引
                { (byte)RequestParamaterKey.CENTER_LINE_INDEX, centerLineIndex },
                // 是否打到左边
                { (byte)RequestParamaterKey.PUT_ON_CENTER_LINE_LEFT, putOnLeft },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 同步玩家组成鸟群操作
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void LockStepPlayerMakeGroup(CardType cardType, Action<bool> handler = null, Action timeoutHandler = null)
    {
        // 补充成功回调，防止空异常
        if (handler == null)
        {
            handler = success => { };
        }

        // 发出请求
        PhotonEngine.SendOperation(
            RequestCode.LOCK_STEP_PLAYER_MAKE_GROUP,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, GlobalModel.Instance.TableInfo.Id },
                // 卡牌种类
                { (byte)RequestParamaterKey.CARD_TYPE, (int)cardType },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 同步玩家不组成鸟群操作
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void LockStepPlayerDontMakeGroup(Action<bool> handler = null, Action timeoutHandler = null)
    {
        // 补充成功回调，防止空异常
        if (handler == null)
        {
            handler = success => { };
        }

        // 发出请求
        PhotonEngine.SendOperation(
            RequestCode.LOCK_STEP_PLAYER_DONT_MAKE_GROUP,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, GlobalModel.Instance.TableInfo.Id },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 同步玩家抽卡操作
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void LockStepPlayerDrawCards(Action<bool> handler = null, Action timeoutHandler = null)
    {
        // 补充成功回调，防止空异常
        if (handler == null)
        {
            handler = success => { };
        }

        // 发出请求
        PhotonEngine.SendOperation(
            RequestCode.LOCK_STEP_PLAYER_DRAW_CARDS,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, GlobalModel.Instance.TableInfo.Id },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 同步玩家选择不抽卡操作
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void LockStepPlayerDontDrawCards(Action<bool> handler = null, Action timeoutHandler = null)
    {
        // 补充成功回调，防止空异常
        if (handler == null)
        {
            handler = success => { };
        }

        // 发出请求
        PhotonEngine.SendOperation(
            RequestCode.LOCK_STEP_PLAYER_DONT_DRAW_CARDS,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, GlobalModel.Instance.TableInfo.Id },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 同步玩家超时
    /// </summary>
    /// <param name="timeOutPlayerId"></param>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void LockStepPlayerOutOfTime(int timeOutPlayerId, Action<bool> handler = null, Action timeoutHandler = null)
    {
        // 补充成功回调，防止空异常
        if (handler == null)
        {
            handler = success => { };
        }

        // 发出请求
        PhotonEngine.SendOperation(
            RequestCode.LOCK_STEP_PLAYER_OUT_OF_TIME,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, GlobalModel.Instance.TableInfo.Id },
                // 超时的玩家的 ID
                { (byte)RequestParamaterKey.TIME_OUT_PLAYER_ID, timeOutPlayerId },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }

    /// <summary>
    /// 同步玩家放弃游戏
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="timeoutHandler"></param>
    public static void LockStepPlayerGiveUp(Action<bool> handler = null, Action timeoutHandler = null)
    {
        // 补充成功回调，防止空异常
        if (handler == null)
        {
            handler = success => { };
        }

        // 发出请求
        PhotonEngine.SendOperation(
            RequestCode.LOCK_STEP_PLAYER_GIVE_UP,
            new Dictionary<byte, object>() {
                // 桌子 ID
                { (byte)RequestParamaterKey.TABLE_ID, GlobalModel.Instance.TableInfo.Id },
            },
            SendOptions.SendReliable,
            response => handler.Invoke(response.Parameters.Get<bool>(ResponseParamaterKey.SUCCESS)),
            timeoutHandler);
    }
}
