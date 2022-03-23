using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using UnityEngine;
using UnityEngine.Events;

//// 这部分主要负责接收后端的事件和转发 ////
public partial class PhotonEngine : MonoBehaviour, IPhotonPeerListener
{
    /// <summary>
    /// 事件码到处理方法的映射表
    /// </summary>
    private static readonly Dictionary<byte, UnityEvent<EventData>> eventHandlers = new Dictionary<byte, UnityEvent<EventData>>();
    // UnityAction 是弱引用的可以防止内存泄漏

    /// <summary>
    /// 当服务器向这个客户端发送事件时这个方法会被调用
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEvent(EventData eventData)
    {
        Debug.Log("收到服务器事件：Code = " + eventData.Code + "，Parameters = " + eventData.Parameters);

        // 如果映射表里有这个事件码则进行转发
        if (eventHandlers.ContainsKey(eventData.Code))
        {
            // 转发给所有订阅了这个事件的方法
            eventHandlers[eventData.Code]?.Invoke(eventData);
        }
    }

    /// <summary>
    /// 订阅指定事件码的事件
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="handler"></param>
    public static void Subscribe(EventCode eventCode, UnityAction<EventData> handler)
    {
        // 如果映射表里还没有这个事件码则添加
        if (!eventHandlers.ContainsKey((byte)eventCode))
        {
            eventHandlers.Add((byte)eventCode, new UnityEvent<EventData>());
        }

        // 添加订阅
        eventHandlers[(byte)eventCode].AddListener(handler);
    }

    /// <summary>
    /// 取消订阅指定事件码的事件
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="handler"></param>
    public static void Unsubscribe(EventCode eventCode, UnityAction<EventData> handler)
    {
        // 如果映射表里没有这个事件码，说明没有任何一个这个事件码的订阅，不需要取消，直接返回
        if (!eventHandlers.ContainsKey((byte)eventCode))
        {
            return;
        }

        // 添加订阅
        eventHandlers[(byte)eventCode].RemoveListener(handler);
    }
}
