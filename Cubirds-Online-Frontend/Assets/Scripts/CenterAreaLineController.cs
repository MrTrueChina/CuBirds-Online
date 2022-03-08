using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    /// 放下牌的位置到行中心的距离
    /// </summary>
    [SerializeField]
    [Header("放下牌的位置到行中心的距离")]
    private float putCardOffset = 400;

    /// <summary>
    /// 每张牌中心点的水平距离
    /// </summary>
    [SerializeField]
    [Header("每张牌中心点的水平距离")]
    private float horizontalDistance = 90;
    /// <summary>
    /// 这一行卡牌显示的最大宽度
    /// </summary>
    [SerializeField]
    [Header("这一行卡牌显示的最大宽度")]
    private float maxWidth = 650;

    /// <summary>
    /// 这一行的卡牌，在桌面上从左到右排列，即数组索引越大牌越靠右
    /// </summary>
    public List<Card> Cards { get; private set; } = new List<Card>();

    /// <summary>
    /// 记录上一次显示是否是使用水平距离显示的卡牌
    /// </summary>
    private bool lastDisplayUseHorizontalDistance = true;

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
        // 放下卡牌的位置的偏移值
        Vector3 putCardPositionOffset = right ? Vector3.right * putCardOffset : Vector3.left * putCardOffset;

        // 记录移动到位置的卡的数量的计数器
        int movedCardNumbers = 0;
        // 遍历所有要打到行里的的牌
        putCards.ForEach(putCard =>
        {
            // 移动到行里
            putCard.MoveToAndRotateTo(LinePosition.position + putCardPositionOffset, LinePosition.rotation, duration, () =>
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

        if(player != null)
        {
            // 传入了玩家，表示是玩家打的牌，需要进行收牌判断，没有传入玩家则是卡组在铺场和补牌，没有收牌步骤

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
        else
        {
            // 没有传入玩家，直接显示

            // 显示卡牌
            DisplayCards();
        }

        // 执行回调
        callback.Invoke(getCards.Count);
    }

    /// <summary>
    /// 更新卡牌的显示
    /// </summary>
    private void DisplayCards()
    {
        // 设置卡牌显示顺序
        for (int i = 0;i < Cards.Count; i++)
        {
            Cards[i].SetDisplaySort(i);
        }

        // 判断按照卡牌距离显示是否会超过最大显示宽度
        if ((Cards.Count - 1) * horizontalDistance > maxWidth)
        {
            // 超过了最大显示宽度

            // 移动卡牌位置到在最大宽度范围内均匀铺开
            for (int i = 0; i < Cards.Count; i++)
            {
                Card card = Cards[i];

                // 计算卡牌距离中心点的偏移
                float offset = -(maxWidth / 2) + i * (maxWidth / (Cards.Count - 1));

                // 移动卡牌
                card.MoveToAndRotateTo(LinePosition.position + LinePosition.right * offset, linePosition.rotation, 0.2f);
            }

            // 记录这次卡牌显示不是使用水平距离显示
            lastDisplayUseHorizontalDistance = false;
        }
        else
        {
            // 没超过最大宽度

            // 根据上一次显示是不是按照水平距离显示进行处理
            if (!lastDisplayUseHorizontalDistance)
            {
                // 如果上一次显示不是按照水平距离显示，说明这一行之前卡牌太多显示不开，需要调整回来

                //Debug.Log("行卡牌数量减少到可以按距离显示");

                // 选择中间的牌作为这一行的中心位置的牌，如果牌是双数则选靠右那个，这样显示的牌右边空会大一点，看起来像是从左往右排列
                int centerCardIndex = Cards.Count / 2;

                // 移动卡牌位置
                for(int i = 0; i < Cards.Count; i++)
                {
                    // 中间的牌的水平偏移是 0，越靠前的越向左，越靠后的越向右
                    float offset = (i - centerCardIndex) * horizontalDistance;

                    // 移动卡牌
                    Cards[i].MoveToAndRotateTo(LinePosition.position + LinePosition.right * offset, linePosition.rotation, 0.2f);
                }
            }
            else
            {
                // 如果上一次显示是按照水平距离显示，则这一行可以在上次显示的基础上进行显示

                // 复制一份卡牌列表，按照卡牌距离中心的距离进行排序，这里要使用本地坐标系的相对坐标
                List<Card> distanceSortedCards = Cards.ToList();
                distanceSortedCards.Sort((a, b) => (int)(Mathf.Abs(a.transform.localPosition.x) - Math.Abs(b.transform.localPosition.x)));

                // 最靠近中心的牌作为中心位置牌，这个牌的索引就是中心位置的索引
                int centerCardIndex = Cards.IndexOf(distanceSortedCards.First());

                if ((0 - centerCardIndex) * horizontalDistance < -(maxWidth / 2))
                {
                    // 如果中心牌放在最中间，最左边的牌的显示位置会超出显示范围，需要调整中心位置牌的索引（超出范围的标准是这个牌的坐标在最大范围的一半之外，因为行的位置在显示范围的中心，前后超出一半就是超出范围）

                    // 计算出中心位置牌在中心的情况下单侧可以显示的牌的数量
                    int singleSigeDisplayCardsNumber = (int)(maxWidth / 2 / horizontalDistance);

                    // 从最左侧牌的索引开始，走过可显示的牌的数量，就是要让最左侧的牌能够显示需要的中心位置的牌的索引
                    centerCardIndex = singleSigeDisplayCardsNumber - 0;
                }
                else if (((Cards.Count - 1) - centerCardIndex) * horizontalDistance > (maxWidth / 2))
                {
                    // 如果中心牌放在最中间，最右边的牌的显示位置会超出显示范围，需要调整中心位置牌的索引

                    // 计算出中心位置牌在中心的情况下单侧可以显示的牌的数量
                    int singleSigeDisplayCardsNumber = (int)(maxWidth / 2 / horizontalDistance);

                    // 从最右侧牌的索引开始，走过可显示的牌的数量，就是要让最右侧的牌能够显示需要的中心位置的牌的索引
                    centerCardIndex = (Cards.Count - 1) - singleSigeDisplayCardsNumber;
                }

                // 移动卡牌位置
                for (int i = 0; i < Cards.Count; i++)
                {
                    // 中间的牌的水平偏移是 0，越靠前的越向左，越靠后的越向右
                    float offset = (i - centerCardIndex) * horizontalDistance;

                    // 移动卡牌
                    Cards[i].MoveToAndRotateTo(LinePosition.position + LinePosition.right * offset, linePosition.rotation, 0.2f);
                }
            }

            // 记录这次卡牌显示是使用水平距离显示
            lastDisplayUseHorizontalDistance = true;
        }
    }
}
