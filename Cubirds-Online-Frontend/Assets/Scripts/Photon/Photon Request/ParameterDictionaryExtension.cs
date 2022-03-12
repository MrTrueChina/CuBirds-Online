using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using CubirdsOnline.Common;
using UnityEngine;

/// <summary>
/// <see cref="ParameterDictionary"/> 类型的扩展方法类
/// </summary>
public static class ParameterDictionaryExtension
{
    /// <summary>
    /// 获取参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(this ParameterDictionary data, EventParamaterKey key)
    {
        return data.Get<T>((byte)key);
    }

    /// <summary>
    /// 获取参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(this ParameterDictionary data, RequestParamaterKey key)
    {
        return data.Get<T>((byte)key);
    }

    /// <summary>
    /// 获取参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(this ParameterDictionary data, ResponseParamaterKey key)
    {
        return data.Get<T>((byte)key);
    }
}
