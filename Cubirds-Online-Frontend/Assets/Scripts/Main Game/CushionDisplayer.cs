using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制桌垫显隐的组件
/// </summary>
public class CushionDisplayer : MonoBehaviour
{
    /// <summary>
    /// 桌垫图片
    /// </summary>
    [SerializeField]
    [Header("桌垫图片")]
    private GameObject cushion;

    private void Start()
    {
        // 读取设置，根据设置控制桌垫图片物体是否激活
        cushion.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("UseCushion", Convert.ToInt32(true))));
    }
}
