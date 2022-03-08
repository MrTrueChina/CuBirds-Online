using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    /// 确认组群的按钮
    /// </summary>
    [SerializeField]
    [Header("确认组群的按钮")]
    private Button makeGroupButton;

    /// <summary>
    /// 选择要组群的种类
    /// </summary>
    private CardType selectedCardType;

    /// <summary>
    /// 开始组群操作
    /// </summary>
    public void StartMakeGroup()
    {
        // 显示组群操作面板
        makeGroupCamvas.SetActive(true);

        // 禁用确定组群按钮
        makeGroupButton.interactable = false;

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
            // 记录这个鸟类
            selectedCardType = card.CardType;

            // 通知玩家抬高显示选择的卡
            GameController.Instance.CurrentTrunPlayre.UpDisplayCards(card.CardType);

            // 激活确认组群按钮
            makeGroupButton.interactable = true;

            // 将事件设为已使用
            eventData.Use();
        }
    }

    /// <summary>
    /// 确认组成鸟群按钮点击时这个方法会被调用
    /// </summary>
    public void OnMakeGroupButtonClick()
    {
        Debug.Log("确认组成鸟群按钮被点击");

        // 通知输入控制器发出玩家组成鸟群消息
        InputController.Instance.CallPlayerMakeGroup(GameController.Instance.CurrentTrunPlayre.Id, selectedCardType);

        // 通知玩家停止抬高显示
        GameController.Instance.CurrentTrunPlayre.CalcelUpDisplayCards();

        // 关闭面板
        Close();
    }

    /// <summary>
    /// 不组成鸟群按钮点击时这个方法会被调用
    /// </summary>
    public void OnDontMakeGroupButtonClick()
    {
        Debug.Log("不组成鸟群按钮被点击");

        // 通知输入控制器发出玩家不组成鸟群消息
        InputController.Instance.CallPlayerDontMakeGroup(GameController.Instance.CurrentTrunPlayre.Id);

        // 通知玩家停止抬高显示
        GameController.Instance.CurrentTrunPlayre.CalcelUpDisplayCards();

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
