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
    #region 预制部分
    public static GameObject Prefab
    {
        get
        {
            if (prefab != null)
            {
                return prefab;
            }

            lock (typeof(InfoDialogController))
            {
                if (prefab == null)
                {
                    // 从 Resources 加载预制
                    prefab = Resources.Load<GameObject>("Global UI/Info Dialog");
                }

                return prefab;
            }
        }
    }
    private static GameObject prefab;
    #endregion

    /// <summary>
    /// 对话框预制的根物体
    /// </summary>
    [SerializeField]
    [Header("对话框预制的根物体")]
    private GameObject rootGameObject;

    /// <summary>
    /// 对话框的 Transfrom 组件
    /// </summary>
    [SerializeField]
    [Header("对话框的 Transfrom 组件")]
    private RectTransform rectTransform;

    /// <summary>
    /// 文本组件
    /// </summary>
    [SerializeField]
    [Header("文本组件")]
    private Text text;

    /// <summary>
    /// 定时显示信息
    /// </summary>
    /// <param name="info"></param>
    /// <param name="duration">显示时间</param>
    public static void Show(string info, float duration)
    {
        // 显示信息
        InfoDialogController instance = Show(info);

        // 延时关闭
        instance.StartCoroutine(DelayClose(instance, duration));
    }

    /// <summary>
    /// 显示信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns>显示信息的对话框的实例的控制脚本</returns>
    public static InfoDialogController Show(string info)
    {
        // 实例化并获取控制脚本
        InfoDialogController instance = Instantiate(Prefab).GetComponentInChildren<InfoDialogController>();

        // 显示信息
        instance.ShowInfo(info);

        // 返回控制脚本
        return instance;
    }

    /// <summary>
    /// 延时关闭指定面板
    /// </summary>
    /// <param name="dialog">要关闭的面板</param>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private static IEnumerator DelayClose(InfoDialogController dialog, float delayTime)
    {
        // 等待时间
        yield return new WaitForSeconds(delayTime);

        // 关闭面板
        dialog.Close();
    }

    /// <summary>
    /// 显示信息，不会自动关闭，只有调用 <see cref="Close"/> 才会关闭
    /// </summary>
    /// <param name="info"></param>
    private void ShowInfo(string info)
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
            // 完成后从根物体开始销毁整个对话框
            .onComplete = () => Destroy(rootGameObject);
    }
}
