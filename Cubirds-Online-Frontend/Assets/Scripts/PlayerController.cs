using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public void TakeHandCard(Card card, Action callback)
    {
        //Debug.LogFormat("玩家 {0} 获取卡牌 {1} {2}", Id, card.Id, card.CardType);

        // 把卡牌添加到手牌里
        handCards.Add(card);

        // 翻开牌
        card.SetOpen(true);

        // 显示手牌
        DisplayHandCards();

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 显示手牌
    /// </summary>
    private void DisplayHandCards()
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

    /// <summary>
    /// 获取鸟群卡
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback"></param>
    public void TakeGroupCard(Card card, Action callback = null)
    {
        // 添加到鸟群卡列表中
        groupCards.Add(card);

        // 显示鸟群卡
        DisplayGroupCards(callback);
    }

    /// <summary>
    /// 显示鸟群卡
    /// </summary>
    /// <param name="callback"></param>
    private void DisplayGroupCards(Action callback = null)
    {
        // 将鸟群卡进行排序
        groupCards.Sort((a, b) => (int)b.CardType - (int)a.CardType);

        // 提取出现有的鸟群卡的种类列表
        List<CardType> cardTypes = groupCards.Select(c => c.CardType).Distinct().ToList();

        // 转化出鸟类卡种类对应的横轴偏移量映射表
        Dictionary<CardType, float> typeToOffset = cardTypes.ToDictionary(t => t, t => ((cardTypes.Count - 1) * -60f) + (cardTypes.IndexOf(t) * 120));

        // 同种类的卡出现了多少次的计数器
        int typeNumber = 0;
        // 记录上一张卡的种类
        CardType lastType = groupCards.First().CardType;

        // 遍历卡
        groupCards.ForEach(card =>
        {
            // 如果这张卡的种类和上一张卡的种类不同，清空同种卡出现次数计数，记录新的种类
            if (card.CardType != lastType)
            {
                lastType = card.CardType;
                typeNumber = 0;
            }

            // 水平偏移量
            float horizontalOffset = typeToOffset[card.CardType];
            // 垂直偏移量，鸟群卡整体向上偏移，之后每张同类牌向上多偏移一点
            float verticalOffset = 140 + typeNumber * 40;

            // 移动卡牌
            card.MoveTo(transform.position + transform.up * verticalOffset - transform.right * horizontalOffset, 0.1f);

            // 每遍历一张，这个种类的计数器增加
            typeNumber++;
        });

        // 设置显示顺序
        for(int i = 0; i < groupCards.Count; i++)
        {
            // 越靠后的显示顺序越高，因为同类的卡的显示效果是最下面那张完全显示
            groupCards[i].SetDisplaySort(groupCards.Count - i);
        }

        if(callback != null)
        {
            callback.Invoke();
        }
    }
}
