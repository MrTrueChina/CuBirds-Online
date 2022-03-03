using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 中央区单行控制器
/// </summary>
public class CenterAreaLineController : MonoBehaviour
{
    /// <summary>
    /// 这一行的位置
    /// </summary>
    public Transform LinePosition { get { return linePosition; } }
    [SerializeField]
    [Header("这一行的位置")]
    private Transform linePosition;

    /// <summary>
    /// 这一行的卡牌
    /// </summary>
    public List<Card> Cards { get; private set; } = new List<Card>();

    /// <summary>
    /// 将卡牌放到这一行里
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="right">是否放在右端，如果传入 false 则放在左端</param>
    /// <param name="callback"></param>
    public void PutCard(List<Card> cards, bool right, Action callback = null)
    {
        if (right)
        {
            // 如果放在右侧则添加到列表末尾
            Cards.AddRange(cards);
        }
        else
        {
            // 放在左边则添加到列表开头
            Cards.InsertRange(0, cards);
        }

        for(int i = 0;i < Cards.Count; i++)
        {
            Card card = Cards[i];

            // 计算卡牌距离中心点的偏移
            float offset = (Cards.Count - 1) * -60 + i * 120;

            // 移动卡牌
            card.MoveTo(LinePosition.position - LinePosition.right * offset, 0.2f);
        }

        // 执行回调
        callback.Invoke();
    }
    /// <summary>
    /// 将卡牌放到这一行里
    /// </summary>
    /// <param name="card"></param>
    /// <param name="right">是否放在右端，如果传入 false 则放在左端</param>
    /// <param name="callback"></param>
    public void PutCard(Card card, bool right, Action callback = null)
    {
        PutCard(new List<Card>() { card }, right, callback);
    }
}
