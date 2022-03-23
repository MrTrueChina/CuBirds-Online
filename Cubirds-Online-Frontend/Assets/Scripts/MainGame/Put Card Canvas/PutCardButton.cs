using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选择把牌打到哪个位置的按钮的组件
/// </summary>
public class PutCardButton : MonoBehaviour
{
    /// <summary>
    /// 玩家打出牌操作的控制器
    /// </summary>
    [SerializeField]
    [Header("玩家打出牌操作的控制器")]
    private PlayCardsController playCardsController;

    /// <summary>
    /// 这个按钮所在的行的索引
    /// </summary>
    [SerializeField]
    [Header("这个按钮所在的行的索引（从 0 开始计数）")]
    private int lineIndex;
    /// <summary>
    /// 这个按钮是否在左边
    /// </summary>
    [SerializeField]
    [Header("这个按钮是否在左边")]
    private bool isLeft;

    /// <summary>
    /// 当按钮点击时这个方法会被调用
    /// </summary>
    public void OnSelectPositionButtonClick()
    {
        // 转发消息给玩家打牌操作控制器
        playCardsController.OnPutCardButtonClick(lineIndex, isLeft);
    }
}
