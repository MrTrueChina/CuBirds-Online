using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据 UI 缩放进行长度计算的组件
/// </summary>
public class ScaleByMainUILength : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    private static ScaleByMainUILength Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(ScaleByMainUILength))
            {
                if (instance == null)
                {
                    instance = GameObject.FindWithTag("ScaleByMainUILength").GetComponent<ScaleByMainUILength>();
                }

                return instance;
            }
        }
    }
    private static ScaleByMainUILength instance;

    /// <summary>
    /// 初始缩放，这是编辑状态下主 UI 显示的缩放比例
    /// </summary>
    private const float START_SCALE = 0.675f;

    /// <summary>
    /// UI 主画布
    /// </summary>
    [SerializeField]
    [Header("UI 主画布")]
    private Canvas mainCanvas;

    /// <summary>
    /// 获取根据主 UI 缩放的长度
    /// </summary>
    /// <param name="originLength"></param>
    /// <returns></returns>
    public static float GetScaledLength(float originLength)
    {
        return originLength * Instance.mainCanvas.transform.lossyScale.x / START_SCALE;
    }
}
