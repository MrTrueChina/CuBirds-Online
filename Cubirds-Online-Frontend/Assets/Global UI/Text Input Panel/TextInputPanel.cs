using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 文本输入面板
/// </summary>
public class TextInputPanel : MonoBehaviour
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
                lock (typeof(TextInputPanel))
                {
                    if (prefab == null)
                    {
                        prefab = Resources.Load<GameObject>("Global UI/Text Input Panel");
                    }

                    return prefab;
                }
            }
        }
    }
    private static GameObject prefab;

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="confirmEventHandler">输入确认的回调，参数是输入的文本</param>
    /// <param name="closeEventHandler">放弃输入的回调</param>
    public static void Show(string title, UnityAction<string> confirmEventHandler, UnityAction closeEventHandler = null)
    {
        // 实例化面板并初始化
        Instantiate(Prefab).GetComponent<TextInputPanel>().Init(title, confirmEventHandler, closeEventHandler);
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
    /// 输入框组件
    /// </summary>
    [Header("输入框组件")]
    [SerializeField]
    private InputField inputField;

    /// <summary>
    /// 文本输入面板关闭事件，这里使用 <see cref="UnityEvent"/> 保存而不是 <see cref="UnityAction"/> 保存的原因是：<see cref="UnityEvent"/> 是弱引用当物体销毁后引用会随之移除不会导致内存泄漏，而 <see cref="UnityAction"/> 是强引用当物体销毁后依然会保留引用导致难以发现的内存泄漏
    /// </summary>
    private UnityEvent closeEvent = new UnityEvent();
    /// <summary>
    /// 确认输入事件，参数是输入的文本
    /// </summary>
    private UnityEvent<string> confirmEvent = new UnityEvent<string>();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="confirmEventHandler">输入确认的回调，参数是输入的文本</param>
    /// <param name="closeEventHandler">放弃输入的回调</param>
    private void Init(string title, UnityAction<string> confirmEventHandler, UnityAction closeEventHandler = null)
    {
        // 显示标题
        titleText.text = title;

        // 保存回调
        if (confirmEventHandler != null)
        {
            confirmEvent.AddListener(confirmEventHandler);
        }
        if (closeEventHandler != null)
        {
            closeEvent.AddListener(closeEventHandler);
        }
    }

    /// <summary>
    /// 确认输入
    /// </summary>
    public void Confirm()
    {
        // 销毁面板物体
        Destroy(gameObject);

        // 执行回调
        confirmEvent.Invoke(inputField.text);
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
