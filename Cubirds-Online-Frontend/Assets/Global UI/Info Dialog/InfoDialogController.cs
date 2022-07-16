using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins;
using UnityEngine.UI;

/// <summary>
/// 显示信息的对话框的控制器
/// </summary>
public class InfoDialogController : MonoBehaviour
{
    #region 单例部分
    /// <summary>
    /// 实例
    /// </summary>
    public static InfoDialogController Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (typeof(InfoDialogController))
            {
                if(instance == null)
                {
                    // 从 Resources 加载预制
                    GameObject prefab = Resources.Load<GameObject>("Info Dialog");

                    // 实例化并保存组件
                    instance = Instantiate(prefab).GetComponentInChildren<InfoDialogController>();

                    // 禁用实例，默认不显示
                    instance.gameObject.SetActive(false);
                }

                return instance;
            }
        }
    }
    private static InfoDialogController instance;
    #endregion

    [SerializeField]
    private RectTransform rectTransform;

    /// <summary>
    /// 文本组件
    /// </summary>
    [SerializeField]
    [Header("文本组件")]
    private Text text;

    /// <summary>
    /// 显示信息，在指定时间后关闭
    /// </summary>
    /// <param name="info"></param>
    /// <param name="displayTime">显示信息的时间（秒）</param>
    public void Show(string info, float displayTime)
    {
        // 显示信息
        Show(info);

        // 用协程延时关闭
        StartCoroutine(DelayClose(displayTime));
    }

    /// <summary>
    /// 延时关闭面板
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator DelayClose(float delayTime)
    {
        // 等待时间
        yield return new WaitForSeconds(delayTime);

        // 关闭面板
        Close();
    }

    /// <summary>
    /// 显示信息，不会自动关闭，只有调用 <see cref="Close"/> 才会关闭
    /// </summary>
    /// <param name="info"></param>
    public void Show(string info)
    {
        // 设置显示的文本
        text.text = info;

        // 首先把缩放调到 0，在视觉上隐藏起来
        rectTransform.localScale = new Vector3(1, 0, 1);

        // 激活实例
        rectTransform.gameObject.SetActive(true);

        // 渐变显示出来
        rectTransform.DOScaleY(1, 0.1f);
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public void Close()
    {
        // 渐变关闭
        rectTransform.DOScaleY(0, 0.1f)
            // 完成后禁用实例
            .onComplete = () => { rectTransform.gameObject.SetActive(false); };
    }
}
