using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 玩家打出牌的控制器
/// </summary>
public class PlayCardsController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static PlayCardsController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(PlayCardsController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindWithTag("PlayCardsController").GetComponent<PlayCardsController>();
                }

                return instance;
            }
        }
    }
    private static PlayCardsController instance;

    /// <summary>
    /// 打牌操作面板
    /// </summary>
    [SerializeField]
    [Header("打牌操作面板")]
    private GameObject playCardCanvas;
    /// <summary>
    /// 选择打牌位置的按钮的画布
    /// </summary>
    [SerializeField]
    [Header("选择打牌位置的按钮的画布")]
    private GameObject selectPutLineButtonCanvas;

    /// <summary>
    /// 要打出的牌的种类
    /// </summary>
    private CardType selectedCardType;

    /// <summary>
    /// 开始打出牌
    /// </summary>
    public void StartPlayCards()
    {
        Debug.Log("开始打牌操作");

        // 订阅输入器的卡片被点击事件
        InputController.Instance.OnCardPointClickEvent.AddListener(OnCardClick);

        // 显示打牌操作面板
        playCardCanvas.SetActive(true);
    }

    /// <summary>
    /// 当输入控制器发出有卡牌被点击事件时这个方法会被调用
    /// </summary>
    /// <param name="card"></param>
    /// <param name="eventData"></param>
    private void OnCardClick(Card card, PointerEventData eventData)
    {
        Debug.LogFormat("打牌界面获取到卡牌被点击事件，卡牌 = {0} {1}", card.CardType, card.Id);

        // 如果点击的卡牌不是当前回合玩家的手牌，说明并不是玩家要打出牌，不处理
        if (!GameController.Instance.CurrentTrunPlayre.HandCardsContainsCard(card))
        {
            //Debug.Log("点击的不是当前回合玩家的手牌，不处理");
            return;
        }

        //Debug.LogFormat("是当前玩家的手牌，记录");

        // 记录选择的卡的种类
        selectedCardType = card.CardType;

        // 通知玩家抬高显示选择的牌
        GameController.Instance.CurrentTrunPlayre.UpDisplayCards(card.CardType);

        // 显示选择把牌打到哪里的按钮
        selectPutLineButtonCanvas.SetActive(true);

        // 将事件改为已使用
        eventData.Use();
    }

    /// <summary>
    /// 当玩家点击打出牌位置的按钮时这个方法会被调用
    /// </summary>
    /// <param name="lineIndex"></param>
    /// <param name="putOnLeft"></param>
    public void OnPutCardButtonClick(int lineIndex, bool putOnLeft)
    {
        Debug.LogFormat("确认打牌操作，打出 {0} 牌，在第 {1} 行，是否在左侧 {2}", selectedCardType, lineIndex, putOnLeft);

        // 通知输入控制器转发这个点击
        InputController.Instance.CallPlayerPlayCards(GameController.Instance.CurrentTrunPlayre.Id, selectedCardType, lineIndex, putOnLeft);

        // 打出牌后关闭这个面板
        Close();
    }

    /// <summary>
    /// 关闭这个面板
    /// </summary>
    private void Close()
    {
        Debug.Log("关闭打牌操作面板");

        // 关闭选择放置位置的按钮
        selectPutLineButtonCanvas.SetActive(false);

        // 关闭打牌操作面板
        playCardCanvas.SetActive(false);

        // 取消订阅输入控制器的卡片被点击事件
        InputController.Instance.OnCardPointClickEvent.RemoveListener(OnCardClick);
    }
}
