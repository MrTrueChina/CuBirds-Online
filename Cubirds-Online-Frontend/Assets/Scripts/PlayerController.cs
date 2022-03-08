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
    private float handCardWidthDistance = 40;
    /// <summary>
    /// 手牌横着的中心距离
    /// </summary>
    [SerializeField]
    [Header("鸟群卡横着的中心距离")]
    private float groupCardWidthDistance = 80;

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
    public List<Card> GroupCards { get; private set; } = new List<Card>();

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
    /// 获取手牌
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback">获取卡牌后的回调</param>
    /// <param name="duration">移动卡牌的时间</param>
    public void TakeHandCard(Card card, Action callback, float duration = 0.5f)
    {
        // 交给协程处理
        StartCoroutine(TakeHandCardCoroutine(card, callback, duration));
    }
    /// <summary>
    /// 获取手牌的协程
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback">获取卡牌后的回调</param>
    /// <param name="duration">移动卡牌的时间</param>
    public IEnumerator TakeHandCardCoroutine(Card card, Action callback, float duration = 0.5f)
    {
        //Debug.LogFormat("玩家 {0} 获取卡牌 {1} {2} 协程启动", Id, card.Id, card.CardType);

        // 移动卡牌到玩家位置并等待卡牌移动到位
        bool moved = false;
        card.MoveToAndRotateTo(transform.position, transform.rotation, duration, () => { moved = true; });
        yield return new WaitUntil(() => moved);

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
    /// 调整手牌的显示顺序和位置
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
            float offset = (handCards.Count - 1) * -(handCardWidthDistance / 2) + i * handCardWidthDistance;

            // 移动卡牌
            card.MoveToAndRotateTo(transform.position - transform.right * offset, transform.rotation, 0.2f);
        }
    }

    /// <summary>
    /// 获取鸟群卡
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动时间</param>
    public void TakeGroupCard(Card card, Action callback = null, float duration = 0.5f)
    {
        // 交给协程进行
        StartCoroutine(TakeGroupCardCoroutine(card, callback, duration));
    }
    public IEnumerator TakeGroupCardCoroutine(Card card, Action callback = null, float duration = 0.5f)
    {
        // 移动卡牌到玩家位置并等待卡牌移动到位
        bool moved = false;
        card.MoveToAndRotateTo(transform.position, transform.rotation, duration, () => { moved = true; });
        yield return new WaitUntil(() => moved);

        // 添加到鸟群卡列表中
        GroupCards.Add(card);

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
        GroupCards.Sort((a, b) => (int)b.CardType - (int)a.CardType);

        // 提取出现有的鸟群卡的种类列表
        List<CardType> cardTypes = GroupCards.Select(c => c.CardType).Distinct().ToList();

        // 转化出鸟类卡种类对应的横轴偏移量映射表
        Dictionary<CardType, float> typeToOffset = cardTypes.ToDictionary(t => t, t => ((cardTypes.Count - 1) * -(groupCardWidthDistance / 2)) + (cardTypes.IndexOf(t) * groupCardWidthDistance));

        // 同种类的卡出现了多少次的计数器
        int typeNumber = 0;
        // 记录上一张卡的种类
        CardType lastType = GroupCards.First().CardType;

        // 遍历卡
        GroupCards.ForEach(card =>
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
            card.MoveToAndRotateTo(transform.position + transform.up * verticalOffset - transform.right * horizontalOffset, transform.rotation, 0.1f);

            // 每遍历一张，这个种类的计数器增加
            typeNumber++;
        });

        // 设置显示顺序
        for(int i = 0; i < GroupCards.Count; i++)
        {
            // 越靠后的显示顺序越高，因为同类的卡的显示效果是最下面那张完全显示
            GroupCards[i].SetDisplaySort(GroupCards.Count - i);
        }

        if(callback != null)
        {
            callback.Invoke();
        }
    }

    /// <summary>
    /// 打出牌
    /// </summary>
    /// <param name="cardType">打出的牌的种类</param>
    /// <param name="line">打到的行的索引</param>
    /// <param name="isLeft">是否打在左边</param>
    /// <param name="callBack">回调，参数是收到的卡牌数量</param>
    public void PlayCards(CardType cardType, CenterAreaLineController line, bool isLeft, Action<int> callBack)
    {
        // 交给协程处理
        StartCoroutine(PlayCardsCorotine(cardType, line, isLeft, callBack));
    }
    /// <summary>
    /// 打出牌的协程
    /// </summary>
    /// <param name="cardType">打出的牌的种类</param>
    /// <param name="line">打到的行的索引</param>
    /// <param name="isLeft">是否打在左边</param>
    /// <param name="callBack">回调，参数是收到的卡牌数量</param>
    /// <returns></returns>
    private IEnumerator PlayCardsCorotine(CardType cardType, CenterAreaLineController line, bool isLeft, Action<int> callBack)
    {
        // 找出手牌中所有这个类型的卡
        List<Card> cards = handCards.FindAll(c => c.CardType == cardType);

        // 从手牌中移除这些卡
        handCards.RemoveAll(c => cards.Contains(c));

        // 调整手牌的显示
        DisplayHandCards();

        // 通知行收卡
        line.PutCard(this, cards, !isLeft, callBack);

        yield return null;
    }

    /// <summary>
    /// 组群
    /// </summary>
    /// <param name="cardType">组群的鸟类</param>
    /// <param name="callback"></param>
    public void MakeGroup(CardType cardType, Action callback)
    {
        // 交给协程进行
        StartCoroutine(MakeGroupCoroutine(cardType, callback));
    }
    /// <summary>
    /// 组群的协程
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator MakeGroupCoroutine(CardType cardType, Action callback)
    {
        Debug.LogFormat("玩家 {0} 进行 {1} 鸟类的组群", Id, cardType);

        // 找出手牌中这些鸟类的牌
        List<Card> makeGroupCards = handCards.FindAll(c => c.CardType == cardType);

        // 从手牌中移除
        handCards.RemoveAll(c => makeGroupCards.Contains(c));

        // 记录能够获取的鸟群卡的数量，如果能组成大鸟群是两张，否则是一张
        int getGroupCardsNumber = makeGroupCards.Count >= makeGroupCards[0].BigGroupNumber ? 2 : 1;

        // 记录需要发送的卡的数量
        int needSendCardsNumber = makeGroupCards.Count;
        // 已经完成发送的卡的数量
        int sendedCardsNumber = 0;

        // 把能收到的鸟群卡放到鸟群区
        for (int i = 0; i < getGroupCardsNumber; i++)
        {
            // 拿最前面那张卡当鸟群卡
            Card groupCard = makeGroupCards[0];

            // 把这张卡移除出手牌
            makeGroupCards.Remove(groupCard);

            // 把这张牌加入鸟群卡
            TakeGroupCard(groupCard, () => {
                // 加入鸟群卡后增加完成发送的卡的数量
                sendedCardsNumber++;
            });
        }

        // 把剩下的牌扔到弃牌区
        foreach(Card discardCard in makeGroupCards)
        {
            // 把牌丢入弃牌区
            GameController.Instance.DiscardCardsController.TakeCard(discardCard, () =>
            {
                // 牌扔到弃牌区后增加完成发送的卡的数量
                sendedCardsNumber++;
            });
        }

        // 更新手卡的显示
        DisplayHandCards();

        // 等待所有的卡发送完毕
        yield return new WaitUntil(() => sendedCardsNumber >= needSendCardsNumber);

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 把所有手卡丢弃
    /// </summary>
    /// <param name="callback"></param>
    public void DiscardAllHandCards(Action callback)
    {
        // 交给协程处理
        StartCoroutine(DiscardAllHandCardsCoroutine(callback));
    }
    /// <summary>
    /// 把所有手卡丢弃的协程
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator DiscardAllHandCardsCoroutine(Action callback)
    {
        Debug.LogFormat("玩家 {0} 丢弃所有手牌", Id);

        // 已经完成发送的卡的数量
        int sendedCardsNumber = 0;

        // 复制一份手牌作为要丢弃的牌的列表
        List<Card> needDiscardCards = new List<Card>(handCards);
        // 清空手牌
        handCards.Clear();

        // 遍历要丢弃的牌
        needDiscardCards.ForEach(handcard =>
        {
            // 交给弃牌堆
            GameController.Instance.DiscardCardsController.TakeCard(handcard, () =>
            {
                // 已发送的卡计数器增加
                sendedCardsNumber++;
            }, 0.4f);
        });

        // 等所有的牌都送入弃牌堆
        yield return new WaitUntil(() => sendedCardsNumber >= needDiscardCards.Count);

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 检测这个玩家手牌中是否有指定的牌
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool HandCardsContainsCard(Card card)
    {
        Debug.LogFormat("检测卡牌 {0} {1} 是不是玩家的手牌", card.CardType, card.Id);

        return handCards.Contains(card);
    }

    /// <summary>
    /// 获取这个玩家手牌中指定类型的牌的数量
    /// </summary>
    /// <param name="cardType"></param>
    /// <returns></returns>
    public int GetHandCardsNumberByCardType(CardType cardType)
    {
        Debug.LogFormat("检测玩家 {0} 手中 {1} 牌的数量", Id, cardType);

        // 返回数量
        return handCards.Count(c => c.CardType == cardType);
    }

    /// <summary>
    /// 获取这个玩家的手牌数量
    /// </summary>
    /// <returns></returns>
    public int HandCardsCount()
    {
        return handCards.Count;
    }
}
