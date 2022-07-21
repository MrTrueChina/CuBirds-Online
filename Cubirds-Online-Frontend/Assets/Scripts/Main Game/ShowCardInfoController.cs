using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 显示卡牌信息的控制器
/// </summary>
public class ShowCardInfoController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static ShowCardInfoController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(ShowCardInfoController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindWithTag("ShowCardInfoController").GetComponent<ShowCardInfoController>();
                }

                return instance;
            }
        }
    }
    private static ShowCardInfoController instance;

    /// <summary>
    /// 显示卡图的图片组件
    /// </summary>
    [SerializeField]
    [Header("显示卡图的图片组件")]
    private Image cardPictureComponent;
    /// <summary>
    /// 显示组群数量的图片组件
    /// </summary>
    [SerializeField]
    [Header("显示组群数量的图片组件")]
    private Image groupNumberComponent;
    /// <summary>
    /// 显示鸟类的图片组件
    /// </summary>
    [SerializeField]
    [Header("显示鸟类的图片组件")]
    private Image cardTypeComponent;
    /// <summary>
    /// 显示这张卡总数的文本组件
    /// </summary>
    [SerializeField]
    [Header("显示这张卡总数的文本组件")]
    private Text cardNumberText;

    private void Start()
    {
        // 卡牌显示图片改为显示已加载卡背，这个卡背图支持自定义和热更新
        cardPictureComponent.sprite = Card.LoadedCardBack;
    }

    /// <summary>
    /// 显示卡牌信息
    /// </summary>
    /// <param name="card"></param>
    public void ShowCardInfo(Card card)
    {
        if (card.IsOpening)
        {
            // 这张牌是明牌的

            // 显示卡面
            cardPictureComponent.sprite = card.CardPicture;

            // 显示组群数量角标
            groupNumberComponent.enabled = true;
            groupNumberComponent.sprite = card.GroupNumberImage;

            // 显示鸟类角标图片
            cardTypeComponent.enabled = true;
            cardTypeComponent.sprite = card.TypeImage;

            // 显示这种牌的卡牌总数
            cardNumberText.text = GameController.Instance.CardsData.cardsData.Find(c => c.cardType == card.CardType).cardNumber.ToString();
        }
        else
        {
            // 这张牌是扣下的

            // 显示卡背
            cardPictureComponent.sprite = card.CardBack;

            // 隐藏组群数量角标
            groupNumberComponent.enabled = false;

            // 隐藏鸟类角标
            cardTypeComponent.enabled = false;

            // 隐藏这种牌的卡牌总数
            cardNumberText.text = "";
        }
    }
}
