using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 手牌横着的中心距离
    /// </summary>
    [SerializeField]
    [Header("手牌横着的中心距离")]
    private float handCardWidthDistance = 50;

    /// <summary>
    /// 这个玩家的 Id
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 手牌
    /// </summary>
    private List<Card> handCards = new List<Card>();
    /// <summary>
    /// 鸟群卡
    /// </summary>
    private List<Card> groupCards = new List<Card>();

    /// <summary>
    /// 初始化这个玩家
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    /// <param name="table"></param>
    public void Init(int id, Transform position, Canvas table)
    {
        // 保存 ID
        Id = id;

        // 把这个玩家物体移到桌子画布上，这样可以跟着桌子对宽高进行自适应
        transform.parent = table.transform;

        // 移动到玩家位置
        transform.position = position.position;
        transform.rotation = position.rotation;
    }

    /// <summary>
    /// 获取卡牌
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback">获取卡牌后的回调</param>
    public void TakeCard(Card card, Action callback)
    {
        //Debug.LogFormat("玩家 {0} 获取卡牌 {1} {2}", Id, card.Id, card.CardType);

        // 把卡牌添加到手牌里
        handCards.Add(card);

        // 翻开牌
        card.SetOpen(true);

        // 显示手牌
        DisplayCards();

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 显示手牌
    /// </summary>
    private void DisplayCards()
    {
        // 玩家可能空手，此时不进行显示手牌的操作
        if(handCards.Count == 0)
        {
            return;
        }

        // 按照手牌的卡的鸟类进行排序
        handCards.Sort((a, b) => (int)b.CardType - (int)a.CardType);

        // 按照排序设置手牌的卡的显示顺序和位置
        for (int i = 0; i < handCards.Count; i++)
        {
            Card card = handCards[i];

            // 设置卡牌显示顺序
            card.SetDisplaySort(i);

            // 计算卡牌距离中心点的偏移
            float offset = (handCards.Count - 1) * -25 + i * 50;

            // 移动卡牌
            card.MoveTo(transform.position - transform.right * offset, 0.2f);
        }
    }
}
