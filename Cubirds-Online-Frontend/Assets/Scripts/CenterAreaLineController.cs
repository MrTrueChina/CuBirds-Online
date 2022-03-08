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
    /// 每张牌中心点的水平距离
    /// </summary>
    [SerializeField]
    [Header("每张牌中心点的水平距离")]
    private float horizontalDistance = 90;

    /// <summary>
    /// 这一行的卡牌，在桌面上从左到右排列，即数组索引越大牌越靠右
    /// </summary>
    public List<Card> Cards { get; private set; } = new List<Card>();

    /// <summary>
    /// 将卡牌放到这一行里
    /// </summary>
    /// <param name="player">放下这些牌的玩家</param>
    /// <param name="putCards">放下的牌</param>
    /// <param name="right">是否放在右端，如果传入 false 则放在左端</param>
    /// <param name="callback">回调，参数是收到的牌的数量</param>
    /// <param name="duration">卡牌移动时间</param>
    public void PutCard(PlayerController player, List<Card> putCards, bool right, Action<int> callback = null, float duration = 0.5f)
    {
        // 交给协程进行
        StartCoroutine(PutCardCorotine(player, putCards, right, callback, duration));
    }
    /// <summary>
    /// 将卡牌放到这一行里
    /// </summary>
    /// <param name="player">放下卡的玩家</param>
    /// <param name="putCard"></param>
    /// <param name="right">是否放在右端，如果传入 false 则放在左端</param>
    /// <param name="callback">回调，参数是收到的牌的数量</param>
    /// <param name="duration">卡牌移动时间</param>
    public void PutCard(PlayerController player, Card putCard, bool right, Action<int> callback = null, float duration = 0.5f)
    {
        PutCard(player, new List<Card>() { putCard }, right, callback, duration);
    }
    /// <summary>
    /// 将卡牌放到这一行里的协程
    /// </summary>
    /// <param name="player">放下这些牌的玩家</param>
    /// <param name="putCards">放下的牌</param>
    /// <param name="right">是否放在右端，如果传入 false 则放在左端</param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动时间</param>
    /// <returns></returns>
    private IEnumerator PutCardCorotine(PlayerController player, List<Card> putCards, bool right, Action<int> callback, float duration)
    {
        // 记录移动到位置的卡的数量的计数器
        int movedCardNumbers = 0;
        // 遍历所有要打到行里的的牌
        putCards.ForEach(putCard =>
        {
            // 移动到行里
            putCard.MoveToAndRotateTo(LinePosition.position, LinePosition.rotation, duration, () =>
            {
                // 移动到位后增加计数器
                movedCardNumbers++;
            });
        });

        // 等待所有牌移动到位
        yield return new WaitUntil(() => movedCardNumbers >= putCards.Count);

        // 收到的牌的列表
        List<Card> getCards = new List<Card>();

        if (right)
        {
            // 如果放在右侧

            // 获取现有的卡牌中最靠右的和放下的牌一样种类的牌的索引
            int lastSameTypeCardIndex = Cards.FindLastIndex((c) => c.CardType == putCards[0].CardType);

            Debug.LogFormat("现有卡牌中最靠右的和放下的牌相同种类的牌是索引是 {0}", lastSameTypeCardIndex);

            // 如果找到了同类型的卡，而且这张卡不在最右边，这张卡右边的卡就是可以收走的牌
            if (lastSameTypeCardIndex > -1 && lastSameTypeCardIndex < Cards.Count - 1)
            {
                // 将这张卡右边的卡加入到收到的牌的列表里
                getCards.AddRange(Cards.GetRange(lastSameTypeCardIndex + 1, Cards.Count - (lastSameTypeCardIndex + 1)));
            }

            // 将打出的牌添加到列表末尾
            Cards.AddRange(putCards);
        }
        else
        {
            // 放在左边

            // 获取现有的卡牌中最靠左的和放下的牌一样种类的牌的索引
            int firstSameTypeCardIndex = Cards.FindIndex((c) => c.CardType == putCards[0].CardType);

            Debug.LogFormat("现有卡牌中最靠左的和放下的牌相同种类的牌是索引是 {0}", firstSameTypeCardIndex);

            // 如果找到了同类型的卡，而且这张卡不在最左边（不在最左边的条件是大于零，覆盖了找到同类型的不等于负一），这张卡左边的卡就是可以收走的牌
            if (firstSameTypeCardIndex > 0)
            {
                // 将这张卡左边的卡加入到收到的牌的列表里
                getCards.AddRange(Cards.GetRange(0, firstSameTypeCardIndex));
            }

            // 将打出的牌添加到列表开头
            Cards.InsertRange(0, putCards);
        }

        // 传入了玩家，表示是玩家打的牌，需要进行收牌判断，没有传入玩家则是卡组在铺场和补牌，没有收牌步骤
        if(player != null)
        {
            Debug.LogFormat("有 {0} 张卡被收走", getCards.Count);

            // 把收走的卡从卡牌列表中移除
            Cards.RemoveAll(c => getCards.Contains(c));

            // 已经发送出去的卡的数量的计数器
            int sendedCardsNumber = 0;
            // 遍历收走的牌
            getCards.ForEach(getCard =>
            {
                // 把牌给玩家
                player.TakeHandCard(getCard, () =>
                {
                    // 玩家拿到牌后增加计数器
                    sendedCardsNumber++;
                });
            });
            // 等待收走的牌全部交到玩家手里
            yield return new WaitUntil(() => sendedCardsNumber >= getCards.Count);

            Debug.Log("被收走的卡已经全部交给玩家");

            // 显示卡牌
            DisplayCards();

            // 如果这一行的牌全是同类的牌，进行补牌，直到这一行出现不同种类的牌。这里判断的方式是：类型和第一张卡相同的卡的数量和所有卡数量相同 => 所有卡都和第一张卡类型相同 => 所有卡都是同类卡
            while (Cards.FindAll(c=>c.CardType == Cards[0].CardType).Count == Cards.Count)
            {
                // 记录是否填充了卡片
                bool geted = false;

                // 让卡组往这一行填充牌
                GameController.Instance.DeckController.SupplementCardToCenterLine(this, right, () =>
                {
                    geted = true;
                });

                // 等待收到卡牌
                yield return new WaitUntil(() => geted);
            }
        }

        // 显示卡牌
        DisplayCards();

        // 执行回调
        callback.Invoke(getCards.Count);
    }

    /// <summary>
    /// 更新卡牌的显示
    /// </summary>
    private void DisplayCards()
    {
        // 移动卡牌位置
        for (int i = 0; i < Cards.Count; i++)
        {
            Card card = Cards[i];

            // 计算卡牌距离中心点的偏移
            float offset = (Cards.Count - 1) * -(horizontalDistance / 2) + i * horizontalDistance;

            // 移动卡牌
            card.MoveToAndRotateTo(LinePosition.position + LinePosition.right * offset, linePosition.rotation, 0.2f);
        }
    }
}
