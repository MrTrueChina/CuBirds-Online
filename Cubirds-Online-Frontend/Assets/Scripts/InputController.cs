using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 输入控制器
/// </summary>
public class InputController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static InputController Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (typeof(InputController))
            {
                if(instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("InputController").GetComponent<InputController>();
                }
            }

            return instance;
        }
    }
    private static InputController instance;

    /// <summary>
    /// 卡牌被点击事件
    /// </summary>
    public UnityEvent<Card, PointerEventData> OnCardPointClickEvent { get; } = new UnityEvent<Card, PointerEventData>();
    /// <summary>
    /// 玩家打出鸟牌事件，参数是 玩家id、牌的种类（鸟类）、打到的中央行的索引、是否打在左侧
    /// </summary>
    public UnityEvent<int, CardType, int, bool> OnPlayerPlayCardsEvent { get; } = new UnityEvent<int, CardType, int, bool>();
    /// <summary>
    /// 玩家组成鸟群事件，参数是 玩家id、组成鸟群的牌的种类（鸟类）
    /// </summary>
    public UnityEvent<int, CardType> OnPlayerMakeGroupEvent { get; } = new UnityEvent<int, CardType>();
    /// <summary>
    /// 玩家选择不组成鸟群事件，参数是 玩家id
    /// </summary>
    public UnityEvent<int> OnPlayerDontMakeGroupEvent { get; } = new UnityEvent<int>();
    /// <summary>
    /// 玩家选择抽牌事件，参数是 玩家id
    /// </summary>
    public UnityEvent<int> OnPlayerDrawCardsEvent { get; } = new UnityEvent<int>();
    /// <summary>
    /// 玩家选择不抽牌事件，参数是 玩家id
    /// </summary>
    public UnityEvent<int> OnPlayerDontDrawCardsEvent { get; } = new UnityEvent<int>();

    /// <summary>
    /// 通知输入控制器有卡牌被点击
    /// </summary>
    /// <param name="card">被点击到的卡牌</param>
    /// <param name="eventData">点击事件</param>
    public void CallCardPointClick(Card card, PointerEventData eventData)
    {
        //Debug.LogFormat("卡牌 {0} ({1}) 被点击", card.CardType, card.Id);

        // 转发出卡牌被点击事件
        OnCardPointClickEvent.Invoke(card, eventData);
    }

    /// <summary>
    /// 通知输入控制器有玩家打出鸟牌
    /// </summary>
    /// <param name="playerId">打牌的玩家的 id</param>
    /// <param name="cardType">打出的牌的种类</param>
    /// <param name="lineIndex">打到的中央行的索引</param>
    /// <param name="putOnLeft">是否打在左侧</param>
    public void CallPlayerPlayCards(int playerId, CardType cardType, int lineIndex, bool putOnLeft)
    {
        Debug.LogFormat("玩家 {0} 打出种类为 {1} 的牌，打到 {2} 行，是否在左侧 {3}", playerId, cardType, lineIndex, putOnLeft);

        // 转发玩家打出鸟牌事件
        OnPlayerPlayCardsEvent.Invoke(playerId, cardType, lineIndex, putOnLeft);
    }

    /// <summary>
    /// 通知输入控制器有玩家组成鸟群
    /// </summary>
    /// <param name="playerId">组成鸟群的玩家的 id</param>
    /// <param name="cardType">组成鸟群的牌的种类</param>
    public void CallPlayerMakeGroup(int playerId, CardType cardType)
    {
        Debug.LogFormat("玩家 {0} 组成 {1} 鸟类的鸟群", playerId, cardType);

        // 转发玩家组成鸟群事件
        OnPlayerMakeGroupEvent.Invoke(playerId, cardType);
    }

    /// <summary>
    /// 通知输入控制器有玩家选择不组成鸟群
    /// </summary>
    /// <param name="playerId"></param>
    public void CallPlayerDontMakeGroup(int playerId)
    {
        Debug.LogFormat("玩家 {0} 选择不组成鸟群", playerId);

        // 转发玩家选择不组成鸟群事件
        OnPlayerDontMakeGroupEvent.Invoke(playerId);
    }

    /// <summary>
    /// 通知输入控制器有玩家选择抽牌
    /// </summary>
    /// <param name="playerId"></param>
    public void CallPlayerDrawCards(int playerId)
    {
        Debug.LogFormat("玩家 {0} 选择抽牌", playerId);

        // 转发玩家选择抽牌事件
        OnPlayerDrawCardsEvent.Invoke(playerId);
    }

    /// <summary>
    /// 通知输入控制器有玩家选择不抽牌
    /// </summary>
    /// <param name="playerId"></param>
    public void CallPlayerDontDrawCards(int playerId)
    {
        Debug.LogFormat("玩家 {0} 选择不抽牌", playerId);

        // 转发玩家选择不抽牌事件
        OnPlayerDontDrawCardsEvent.Invoke(playerId);
    }
}
