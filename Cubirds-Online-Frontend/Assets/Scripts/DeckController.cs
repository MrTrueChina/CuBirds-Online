﻿using System;
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
                Card card = Instantiate(GameController.Instance.CardPrefab, GameController.Instance.TableCanvas.transform).GetComponent<Card>();

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

        // 洗完牌后执行回调
        callBack.Invoke();
    }

    /// <summary>
    /// 将卡牌放入卡组
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback"></param>
    public void TakeCard(Card card, Action callback = null)
    {
        // 把卡扣过来
        card.SetOpen(false);

        // 把卡片添加到列表里
        cards.Add(card);

        // 把卡片移动到顶层
        card.MoveTo(deckPosition.position + Vector3.up * cards.Count, 0.2f);

        // 执行回调
        if(callback != null)
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
            card.MoveTo(DeckPosition.position + (Vector3.up * i), 0.1f);
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
        StartCoroutine(nameof(FillCenterAreaCoroutine), callback);
    }
    /// <summary>
    /// 开场时填充中央区的协程
    /// </summary>
    private IEnumerator FillCenterAreaCoroutine(Action callback)
    {
        // 获取中央区行的控制器，并创建一个字典以记录发出的牌的数量
        List<CenterAreaLineController> lines = GameController.Instance.CenterAreaLineControllers;

        // 有卡片正在发送
        bool cardSending = false;

        // 填充完毕的中央行会移除，一直循环到所有中央行都填充完毕
        while (lines.Count > 0)
        {
            // 获取牌库顶的卡
            Card targetCard = cards.FindLast(c => true);

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

                // 将牌移动到行的位置
                targetCard.MoveTo(targetLine.LinePosition.position, 0.3f, () =>
                {
                    // 到位置后通知行放下卡
                    targetLine.PutCard(targetCard, true, () =>
                    {
                        // 当这一行达到 3 张卡时，这一行已填满，移除这一行
                        if(targetLine.Cards.Count >= 3)
                        {
                            lines.Remove(targetLine);
                        }

                        // 记录没有卡牌正在发送
                        cardSending = false;
                    });
                });
            }
            else
            {
                // 没找到可以放入的中央行

                // 把牌移动到弃牌区的位置
                targetCard.MoveTo(GameController.Instance.DiscardCardsController.GetDiscardPosition(), 0.3f, () =>
                {
                    // 把牌扔进弃牌区
                    GameController.Instance.DiscardCardsController.TakeCard(targetCard, () =>
                    {
                        // 记录没有卡牌正在发送
                        cardSending = false;
                    });
                });
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
    /// 给所有玩家发牌
    /// </summary>
    /// <param name="callback"></param>
    public void DealCards(Action callback)
    {
        // 让协程进行发牌
        StartCoroutine(nameof(DealCardsCoroutine), callback);
    }
    /// <summary>
    /// 给所有玩家发牌的协程
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DealCardsCoroutine(Action callback)
    {
        Debug.LogFormat("给所有玩家发牌");

        // 记录总共发出的卡牌数量的计数器
        int sendedCardNumber = 0;
        // 记录已经到了玩家手里的卡的数量的计数器
        int takedCardNumber = 0;

        // 遍历所有玩家
        foreach (PlayerController player in GameController.Instance.players)
        {
            //Debug.LogFormat("给玩家 {0} 发牌", player.Id);

            for (int i = 0; i < 8; i++)
            {
                //Debug.LogFormat("给玩家 {0} 发第 {1} 张牌", player.Id, i);

                // 等待发牌间隔
                yield return new WaitUntil(() => Time.time - lastSendCardTime >= sendCardInterval);

                // 获取最后一张卡
                Card card = cards.FindLast((c) => true);

                // 从卡组中移除这张卡
                cards.Remove(card);

                // 发出的牌计数器增加
                sendedCardNumber++;

                // 将卡旋转到玩家的朝向
                card.RotateTo(player.transform.rotation);

                // 将卡移动到玩家的位置
                card.MoveTo(player.transform.position, () =>
                {
                    // 移动完成时通知玩家拿走这张卡
                    player.TakeCard(card, () =>
                    {
                        // 玩家拿走卡后完成发送的卡的计数器增加
                        takedCardNumber++;
                    });
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
}