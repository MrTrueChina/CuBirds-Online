using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 卡组控制器
/// </summary>
public class DeckController : MonoBehaviour
{
    /// <summary>
    /// 卡组位置
    /// </summary>
    public Transform DeckPosition { get { return deckPosition; } }
    [SerializeField]
    [Header("卡组位置")]
    private Transform deckPosition;
    /// <summary>
    /// 发牌间隔
    /// </summary>
    [SerializeField]
    [Header("发牌间隔")]
    private float sendCardInterval;
    
    /// <summary>
    /// 卡组中的卡牌
    /// </summary>
    public List<Card> cards { get; private set; } = new List<Card>();

    /// <summary>
    /// 上次发牌时间
    /// </summary>
    private float lastSendCardTime = float.NegativeInfinity;

    /// <summary>
    /// 初始化卡组
    /// </summary>
    /// <param name="callBack">初始化卡组完成后的回调</param>
    public void InitDeck(Action callBack)
    {
        // id 计数器
        int id = 0;

        // 遍历所有卡牌数据
        GameController.Instance.CardsData.cardsData.ForEach(cardData =>
        {
            // 生成对应数量的牌
            for (int i = 0; i < cardData.cardNumber; i++)
            {
                // id 计数器前进
                id++;

                // 创建卡牌并获取卡牌组件
                Card card = Instantiate(GameController.Instance.CardPrefab, GameController.Instance.PlayGameCanvas.transform).GetComponent<Card>();

                // 初始化卡牌
                card.InitData(id, cardData);

                // 把牌扣下去
                card.SetOpen(false);

                // 添加到卡组列表中
                cards.Add(card);
            }
        });

        // 洗牌
        Shuffle(callBack);
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    /// <param name="callBack">洗完牌后的回调</param>
    public void Shuffle(Action callBack)
    {
        Debug.Log("洗牌");

        // 洗牌
        List<Card> shuffledCards = new List<Card>();
        // 循环直到旧牌堆的牌都洗进新牌堆
        while (cards.Count > 0)
        {
            // 随机一个索引
            int cardIndex = Random.Range(0, cards.Count);
            // 把这个索引的牌加入到新牌堆
            shuffledCards.Add(cards[cardIndex]);
            // 把这个索引的牌从旧牌堆里移除
            cards.RemoveAt(cardIndex);
        }
        // 洗牌后的牌堆设为牌堆
        cards = shuffledCards;

        // 洗牌后更新卡组的显示
        DisplayeDeck();

        Debug.Log("洗牌完毕");

        // 洗完牌后执行回调
        callBack.Invoke();
    }

    /// <summary>
    /// 将卡牌放入卡组
    /// </summary>
    /// <param name="takeCards"></param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动的时间</param>
    public void TakeCard(List<Card> takeCards, Action callback = null, float duration = 0.5f)
    {
        // 交给协程进行
        StartCoroutine(TakeCardCoroutine(takeCards, callback, duration));
    }
    /// <summary>
    /// 将卡牌放入卡组的协程
    /// </summary>
    /// <param name="takeCards"></param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动的时间</param>
    public IEnumerator TakeCardCoroutine(List<Card> takeCards, Action callback = null, float duration = 0.5f)
    {
        // 移动完成的卡牌的计数器
        int movedCardsNumber = 0;

        // 遍历所有要加入卡组的牌
        for (int i = 0; i < takeCards.Count; i++)
        {
            Card takeCard = takeCards[i];

            // 移动卡牌，直接移动到正确的高度
            takeCard.MoveToAndRotateTo(DeckPosition.position + Vector3.up * (cards.Count + i), DeckPosition.rotation, duration, () =>
            {
                // 增加移动完成的卡牌的计数器
                movedCardsNumber++;
                // 把这张牌扣下去
                takeCard.SetOpen(false);
            });
        }

        // 等待所有牌移动完成
        yield return new WaitUntil(() => movedCardsNumber >= takeCards.Count);

        // 把卡片添加到列表里
        cards.AddRange(takeCards);

        // 执行回调
        if (callback != null)
        {
            callback.Invoke();
        }
    }

    /// <summary>
    /// 按照卡牌列表显示卡组
    /// </summary>
    private void DisplayeDeck()
    {
        // 调整卡牌位置和显示顺序
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            // 设置卡牌位置，越靠后越高
            card.MoveToAndRotateTo(DeckPosition.position + (Vector3.up * i), DeckPosition.rotation, 0.1f);
            // 设置卡牌显示次序，越靠后越靠前
            card.SetDisplaySort(i);
        }
    }

    /// <summary>
    /// 开场时填充中央区的方法
    /// </summary>
    /// <param name="callback"></param>
    public void FillCenterArea(Action callback)
    {
        // 让协程进行填充
        StartCoroutine(FillCenterAreaCoroutine(callback));
    }
    /// <summary>
    /// 开场时填充中央区的协程
    /// </summary>
    private IEnumerator FillCenterAreaCoroutine(Action callback)
    {
        // 获取中央区行的控制器，并创建一个字典以记录发出的牌的数量。这里是副本，防止操作了主控制器的数据
        List<CenterAreaLineController> lines = new List<CenterAreaLineController>(GameController.Instance.CenterAreaLineControllers);

        // 有卡片正在发送的标志变量
        bool cardSending = false;

        // 填充完毕的中央行会移除，一直循环到所有中央行都填充完毕
        while (lines.Count > 0)
        {
            // 获取牌库顶的卡
            Card targetCard = cards.Last();

            // 从卡组中移除这张卡
            cards.Remove(targetCard);

            // 记录有卡牌正在发送
            cardSending = true;

            // 翻开
            targetCard.SetOpen(true);

            // 查找可以放入的中央行
            CenterAreaLineController targetLine = lines.Find(line =>
                // 如果这一行里没有类型和翻开的卡同类的卡，则这张卡可以放入这一行
                line.Cards.Find(lineCard => Equals(lineCard.CardType, targetCard.CardType)) == null
            );

            if (targetLine != null)
            {
                // 成功找到可以放入的中央行

                // 把牌打到中央区的行里，从卡组出牌没有玩家，玩家传的是 null
                targetLine.PutCard(null, targetCard, true, getCardsNumber =>
                {
                    // 当这一行达到 3 张卡时，这一行已填满，移除这一行
                    if (targetLine.Cards.Count >= 3)
                    {
                        lines.Remove(targetLine);
                    }

                    // 记录没有卡牌正在发送
                    cardSending = false;
                }, 0.4f);
            }
            else
            {
                // 没找到可以放入的中央行

                // 把牌扔进弃牌区
                GameController.Instance.DiscardCardsController.TakeCard(targetCard, () =>
                {
                    // 记录没有卡牌正在发送
                    cardSending = false;
                }, 0.3f);
            }

            // 等待正在发送的卡片发送完毕再进行下一张卡的发送
            yield return new WaitUntil(() => !cardSending);
        }

        // 等所有发出的牌都接收到
        yield return new WaitUntil(() => !cardSending);

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 向中央行补充卡牌
    /// </summary>
    /// <param name="centerLine">要补充到的中央行</param>
    /// <param name="right">是否放在右边</param>
    /// <param name="callback"></param>
    public void SupplementCardToCenterLine(CenterAreaLineController centerLine, bool right, Action callback)
    {
        // 交给协程处理
        StartCoroutine(SupplementCardToCenterLineCoroutine(centerLine, right, callback));
    }
    /// <summary>
    /// 向中央行补充卡牌的协程
    /// </summary>
    /// <param name="centerLine">要补充到的中央行</param>
    /// <param name="right">是否放在右边</param>
    /// <param name="callback"></param>
    private IEnumerator SupplementCardToCenterLineCoroutine(CenterAreaLineController centerLine, bool right, Action callback)
    {
        // 如果卡牌发完了，把弃牌堆的牌洗回来
        if (cards.Count == 0)
        {
            Debug.Log("向中央行补充卡牌时卡组抽空，把弃牌堆返回卡组");

            // 启动洗牌协程并等待洗牌协程完成
            yield return StartCoroutine(ReturnDiscardCards());

            Debug.Log("弃牌堆返回卡组完成");

            // 如果弃牌堆洗回牌堆后还是没有牌，说明所有牌都耗尽了
            if (cards.Count == 0)
            {
                // 通知游戏主控制器卡牌耗尽
                GameController.Instance.RunOutCards();

                // 卡牌耗尽直接结束游戏，直接切断流程不执行回调
                yield break;
            }
        }

        // 获取牌库顶的卡
        Card targetCard = cards.Last();

        // 从卡组中移除这张卡
        cards.Remove(targetCard);

        // 记录卡牌是否已经发送到
        bool sended = false;

        // 翻开
        targetCard.SetOpen(true);

        // 把卡放到行中
        centerLine.PutCard(null, targetCard, right, getCardsNumber =>
        {
            sended = true;
        }, 0.5f);

        // 等待卡牌放到行中
        yield return new WaitUntil(() => sended);

        callback.Invoke();
    }

    /// <summary>
    /// 给玩家发牌
    /// </summary>
    /// <param name="player">发牌给哪个玩家</param>
    /// <param name="cardsNumber">每个玩家的发牌数量</param>
    /// <param name="callback"></param>
    public void DealCards(PlayerController player, int cardsNumber, Action callback)
    {
        DealCards(new List<PlayerController>() { player }, cardsNumber, callback);
    }
    /// <summary>
    /// 给玩家发牌
    /// </summary>
    /// <param name="players">发牌给哪些玩家</param>
    /// <param name="cardsNumber">每个玩家的发牌数量</param>
    /// <param name="callback"></param>
    public void DealCards(List<PlayerController> players, int cardsNumber, Action callback)
    {
        // 让协程进行发牌
        StartCoroutine(DealCardsCoroutine(players, cardsNumber, callback));
    }
    /// <summary>
    /// 给玩家发牌的协程
    /// </summary>
    /// <param name="players">发牌给哪些玩家</param>
    /// <param name="cardsNumber">每个玩家的发牌数量</param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DealCardsCoroutine(List<PlayerController> players, int cardsNumber, Action callback)
    {
        Debug.LogFormat("给玩家发牌");

        // 记录总共发出的卡牌数量的计数器
        int sendedCardNumber = 0;
        // 记录已经到了玩家手里的卡的数量的计数器
        int takedCardNumber = 0;

        // 遍历玩家
        foreach (PlayerController player in players)
        {
            //Debug.LogFormat("给玩家 {0} 发牌", player.Id);

            // 循环发指定数量的牌
            for (int i = 0; i < cardsNumber; i++)
            {
                //Debug.LogFormat("给玩家 {0} 发第 {1} 张牌", player.Id, i);

                // 等待发牌间隔
                yield return new WaitUntil(() => Time.time - lastSendCardTime >= sendCardInterval);

                // 如果卡牌发完了，把弃牌堆的牌洗回来
                if(cards.Count == 0)
                {
                    Debug.Log("向玩家发牌时卡组抽空，把弃牌堆返回卡组");

                    // 启动洗牌协程并等待洗牌协程完成
                    yield return StartCoroutine(ReturnDiscardCards());

                    Debug.Log("弃牌堆返回卡组完成");

                    // 如果弃牌堆洗回牌堆后还是没有牌，说明所有牌都耗尽了
                    if (cards.Count == 0)
                    {
                        // 通知游戏主控制器卡牌耗尽
                        GameController.Instance.RunOutCards();

                        // 卡牌耗尽直接结束游戏，直接切断流程不执行回调
                        yield break;
                    }
                }

                // 获取最后一张卡
                Card card = cards.Last();

                // 从卡组中移除这张卡
                cards.Remove(card);

                // 发出的牌计数器增加
                sendedCardNumber++;

                // 把这张卡给玩家
                player.TakeHandCard(card, () =>
                {
                    // 玩家拿走卡后完成发送的卡的计数器增加
                    takedCardNumber++;
                });

                // 记录这次发牌时间
                lastSendCardTime = Time.time;
            }
        }

        // 等待所有卡牌都送到
        yield return new WaitUntil(() => takedCardNumber == sendedCardNumber);

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 给每个玩家一个初始鸟群
    /// </summary>
    /// <param name="callback"></param>
    public void GivePlayersStartGroup(Action callback)
    {
        // 交给协程进行
        StartCoroutine(GivePlayersStartGroupCoroutine(callback));
    }
    /// <summary>
    /// 给每个玩家一个初始鸟群的协程
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator GivePlayersStartGroupCoroutine(Action callback)
    {
        // 是否有卡在转移的标志变量
        bool cardSending = false;

        // 遍历每一个玩家
        foreach (PlayerController player in GameController.Instance.players)
        {
            // 获取牌库顶的卡
            Card card = cards.Last();

            // 改为有卡在转移
            cardSending = true;

            // 翻到正面
            card.SetOpen(true);

            // 从牌库里移除
            cards.Remove(card);

            // 把牌给玩家
            player.TakeGroupCard(card, () =>
            {
                // 玩家拿到卡后改为没有卡在转移
                cardSending = false;
            });

            // 等到卡牌转移完毕
            yield return new WaitUntil(() => !cardSending);
        }

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 把弃牌堆的牌收回牌组
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnDiscardCards()
    {
        Debug.Log("回收弃牌堆的牌到卡组里");

        // 记录卡牌是否回到卡组的标志变量
        bool geted = false;

        // 通知弃牌堆返回卡牌
        GameController.Instance.DiscardCardsController.BackToDeck(() =>
        {
            // 记录已经回到了卡组
            geted = true;
        });

        // 等待返回完成
        yield return new WaitUntil(() => geted);

        // 记录是否洗牌完毕
        bool shuffled = false;

        // 洗牌
        Shuffle(() =>
        {
            // 记录洗牌完毕
            shuffled = true;
        });

        // 等待洗牌完毕
        yield return new WaitUntil(() => shuffled);
    }
}
