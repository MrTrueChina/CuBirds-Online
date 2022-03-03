using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// 卡组中的卡牌
    /// </summary>
    public List<Card> cards = new List<Card>();

    /// <summary>
    /// 初始化卡组
    /// </summary>
    public void InitDeck()
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
        Shuffle();
    }

    // 洗牌
    private void Shuffle()
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
}
