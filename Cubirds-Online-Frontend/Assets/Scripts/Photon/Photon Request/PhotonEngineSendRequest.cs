using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using UnityEngine;

//// 这部分主要负责前后端请求和返回 ////
public partial class PhotonEngine : MonoBehaviour, IPhotonPeerListener
{
    /// <summary>
    /// 向后端发请求的映射表里存的对象的类
    /// </summary>
    class RequestHandler
    {
        /// <summary>
        /// 发出请求的时间
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// 收到返回后的回调
        /// </summary>
        public Action<OperationResponse> Handler { get; set; }
        /// <summary>
        /// 超时回调
        /// </summary>
        public Action TimeoutHandler { get; set; }
    }

    /// <summary>
    /// 超时时间，单位是刻（Tick）
    /// </summary>
    public static long TIMEOUT_TIME = TimeSpan.TicksPerSecond * 5;

    /// <summary>
    /// 操作 ID 的计数器
    /// </summary>
    private static long operationIdCounter = 0;

    /// <summary>
    /// 发出的请求的处理映射表
    /// </summary>
    private static Dictionary<long, RequestHandler> requestHandlers = new Dictionary<long, RequestHandler>();


    /// <summary>
    /// 检测请求是否超时
    /// </summary>
    private void CheckRequestTimeout()
    {
        //Debug.LogFormat("检测请求是否超时，现在时间 = {0}, 现存请求数量 = {1}", DateTime.Now.Ticks, requestHandlers.Count);

        //// 测试代码，输出每个请求的发送时间
        //var enumerator = requestHandlers.GetEnumerator();
        //while (enumerator.MoveNext())
        //{
        //    Debug.LogFormat("Key 为 {0} 的请求，发送时间为 {1}", enumerator.Current.Key, enumerator.Current.Value.Time);
        //}

        // 从映射表里筛选出超时的请求的操作 ID
        long[] timeoutIds = requestHandlers
            .Where(pair => DateTime.Now.Ticks - pair.Value.Time > TIMEOUT_TIME)
            .Select(pair => pair.Key)
            .ToArray();

        //Debug.LogFormat("检测到 {0} 个请求超时", timeoutIds.Length);

        // 遍历
        foreach (long timeoutId in timeoutIds)
        {
            // 获取请求
            RequestHandler timeoutHandler = requestHandlers[timeoutId];

            // 有超时回调则执行
            if (timeoutHandler.TimeoutHandler != null)
            {
                timeoutHandler.TimeoutHandler.Invoke();
            }

            // 移除这个回调
            requestHandlers.Remove(timeoutId);
        }
    }

    /// <summary>
    /// 向后台发出操作请求
    /// </summary>
    /// <param name="operationCode">操作码</param>
    /// <param name="operationParameters">参数</param>
    /// <param name="sendOptions">发送设置</param>
    /// <param name="handler">服务器响应后的回调</param>
    /// <param name="timeoutHandler">超时回调</param>
    public static void SendOperation(RequestCode operationCode, Dictionary<byte, object> operationParameters, SendOptions sendOptions, Action<OperationResponse> handler = null, Action timeoutHandler = null)
    {
        // 获取操作 ID
        long operationId = operationIdCounter;

        // 操作 ID 计数器前进
        operationIdCounter++;

        // 添加操作 ID
        operationParameters.Add((byte)RequestParamaterKey.OPERATION_ID, operationId);

        // 添加到映射表里
        requestHandlers.Add(operationId, new RequestHandler()
        {

            // 发出请求的时间，就是当前时间，单位是毫秒
            Time = DateTime.Now.Ticks,

            // 收到返回后的回调
            Handler = handler,

            // 超时回调
            TimeoutHandler = timeoutHandler,
        });

        // 向后台发出操作请求
        Peer.SendOperation((byte)operationCode, operationParameters, sendOptions);

        //Debug.LogFormat("已经发出请求，操作码 = {0}，操作 ID = {1}，现在记录的请求数量 = {2}", operationCode, operationId, requestHandlers.Count);
    }

    /// <summary>
    /// 当服务器对这个客户端的发信进行回应时这个方法会被调用
    /// </summary>
    /// <param name="operationResponse"></param>
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        Debug.Log("收到服务器返回：ReturnCode = " + operationResponse.ReturnCode + "，Parameters = " + operationResponse.Parameters);

        // 收到的返回没有操作 ID，发出警告并忽略，没有操作 ID 无法确认回调
        if (!operationResponse.Parameters.ContainsKey((byte)ResponseParamaterKey.OPERATION_ID))
        {
            Debug.LogWarning("服务器发出一个没有操作 ID 的返回，请确认前端是否有没有通过可以添加操作 ID 的发送请求方法进行请求的代码，请确认后台是否使用了没有添加操作 ID 发出返回的代码");
            return;
        }

        // 获取操作 ID
        long operationId = (long)operationResponse.Parameters[(byte)ResponseParamaterKey.OPERATION_ID];

        // 如果映射表里没有这个操作 ID 也不进行处理
        if (!requestHandlers.ContainsKey(operationId))
        {
            Debug.LogWarningFormat("发现操作 ID 为 {0} 的返回，没有该 ID 的回调，可能是该回调已超时", operationId);
            return;
        }

        // 找到对应的回调
        RequestHandler handler = requestHandlers[operationId];

        // 如果有回调则调用
        if (handler.Handler != null)
        {
            handler.Handler.Invoke(operationResponse);
        }

        // 移除这个使用过的回调
        requestHandlers.Remove(operationId);
    }
}
