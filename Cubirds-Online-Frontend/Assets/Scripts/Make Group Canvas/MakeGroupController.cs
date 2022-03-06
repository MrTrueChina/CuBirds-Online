using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 进行组群的面板的控制器
/// </summary>
public class MakeGroupController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static MakeGroupController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(MakeGroupController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindWithTag("MakeGroupController").GetComponent<MakeGroupController>();
                }

                return instance;
            }
        }
    }
    private static MakeGroupController instance;

    /// <summary>
    /// 进行组群的操作面板
    /// </summary>
    [SerializeField]
    [Header("进行组群的操作面板")]
    private GameObject makeGroupCamvas;

    /// <summary>
    /// 开始组群操作
    /// </summary>
    public void StartMakeGroup()
    {
        // 显示组群操作面板
        makeGroupCamvas.SetActive(true);

        // 订阅输入控制器的卡牌被点击事件
        InputController.Instance.OnCardPointClickEvent.AddListener(OnCardPointClick);
    }

    /// <summary>
    /// 当卡牌被点击时这个方法会被调用
    /// </summary>
    /// <param name="card"></param>
    /// <param name="eventData"></param>
    private void OnCardPointClick(Card card, PointerEventData eventData)
    {
        // 如果点击的卡牌不是当前回合玩家的手牌，不处理
        if (!GameController.Instance.CurrentTrunPlayre.HandCardsContainsCard(card))
        {
            return;
        }

        // 如果玩家手里的这种类型的卡足够组成鸟群，通知输入控制器发出玩家组成鸟群事件
        if (GameController.Instance.CurrentTrunPlayre.GetHandCardsNumberByCardType(card.CardType) >= card.SmallGroupNumber)
        {
            // 发出事件
            InputController.Instance.CallPlayerMakeGroup(GameController.Instance.CurrentTrunPlayre.Id, card.CardType);

            // 将事件设为已使用
            eventData.Use();

            // 发出事件后关闭面板
            Close();
        }
    }

    /// <summary>
    /// 不组成鸟群按钮点击时这个方法会被调用
    /// </summary>
    public void OnDontMakeGroupButtonClick()
    {
        Debug.Log("不组成鸟群按钮被点击");

        // 通知输入控制器发出玩家不组成鸟群消息
        InputController.Instance.CallPlayerDontMakeGroup(GameController.Instance.CurrentTrunPlayre.Id);

        // 关闭面板
        Close();
    }

    /// <summary>
    /// 关闭组群操作面板
    /// </summary>
    private void Close()
    {
        Debug.Log("关闭组群操作面板");

        // 取消订阅卡牌点击事件
        InputController.Instance.OnCardPointClickEvent.RemoveListener(OnCardPointClick);

        // 关闭面板
        makeGroupCamvas.SetActive(false);
    }
}
