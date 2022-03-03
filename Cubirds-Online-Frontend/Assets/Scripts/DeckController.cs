using System;
using System.Collections;
using System.Collections.Generic;
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
    private void Shuffle(Action callBack)
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
    /// 按照卡牌列表显示卡组
    /// </summary>
    private void DisplayeDeck()
    {
        // 调整卡牌位置和显示顺序
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            // 设置卡牌位置，越靠后越高
            card.transform.position = deckPosition.position + (Vector3.up * i);
            // 设置卡牌显示次序，越靠后越靠前
            card.SetDisplaySort(i);
        }
    }

    /// <summary>
    /// 给所有玩家发牌
    /// </summary>
    /// <param name="callback"></param>
    public void DealCards(Action callback)
    {
        // 让协程进行发牌
        StartCoroutine("DealCardsCoroutine", callback);
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
