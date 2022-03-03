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
        for(int i = 0;i < 110; i++)
        {
            // 创建卡牌并获取卡牌组件
            Card card = Instantiate(GameController.Instance.CardPrefab, GameController.Instance.TableCanvas.transform).GetComponent<Card>();

            // 把牌扣下去
            card.SetOpen(false);

            // 添加到卡组列表中
            cards.Add(card);
        }

        // 调整卡牌位置和显示顺序
        for(int i = 0;i < cards.Count; i++)
        {
            Card card = cards[i];
            // 设置卡牌位置，越靠后越高
            card.transform.position = deckPosition.position + (Vector3.up * i);
            // 设置卡牌显示次序，越靠后越靠前
            card.SetDisplaySort(i);
        }
    }
}
