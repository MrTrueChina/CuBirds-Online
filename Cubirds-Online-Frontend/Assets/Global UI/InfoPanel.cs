using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 信息显示面板
/// </summary>
public class InfoPanel : MonoBehaviour
{
    #region 静态与调用部分

    /// <summary>
    /// 这个面板的预制
    /// </summary>
    private static GameObject Prefab
    {
        get
        {
            if (prefab != null)
            {
                return prefab;
            }
            else
            {
                lock (typeof(InfoPanel))
                {
                    if (prefab == null)
                    {
                        prefab = Resources.Load<GameObject>("Global UI/Info Panel");
                    }

                    return prefab;
                }
            }
        }
    }
    private static GameObject prefab;

    /// <summary>
    /// 显示信息
    /// </summary>
    /// <param name="title">显示信息的标题</param>
    /// <param name="info">显示信息的内容</param>
    /// <param name="closeEventHandler">显示信息面板关闭后执行的回调</param>
    public static void ShowInfo(string title, string info, UnityAction closeEventHandler = null)
    {
        // 实例化面板并初始化
        Instantiate(Prefab).GetComponent<InfoPanel>().Init(title, info, closeEventHandler);
    }

    #endregion

    #region 动态处理部分

    /// <summary>
    /// 标题文本组件
    /// </summary>
    [Header("标题文本组件")]
    [SerializeField]
    private Text titleText;
    /// <summary>
    /// 信息文本组件
    /// </summary>
    [Header("信息文本组件")]
    [SerializeField]
    private Text infoText;

    /// <summary>
    /// 显示信息面板关闭事件，这里使用 <see cref="UnityEvent"/> 保存而不是 <see cref="UnityAction"/> 保存的原因是：<see cref="UnityEvent"/> 是弱引用当物体销毁后引用会随之移除不会导致内存泄漏，而 <see cref="UnityAction"/> 是强引用当物体销毁后依然会保留引用导致难以发现的内存泄漏
    /// </summary>
    private UnityEvent closeEvent = new UnityEvent();

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init(string title, string info, UnityAction closeEventHandler = null)
    {
        // 显示文本
        titleText.text = title;
        infoText.text = info;

        // 保存回调
        if(closeEventHandler != null)
        {
            closeEvent.AddListener(closeEventHandler);
        }
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public void Close()
    {
        // 销毁面板物体
        Destroy(gameObject);

        // 执行回调
        closeEvent.Invoke();
    }

    #endregion
}
