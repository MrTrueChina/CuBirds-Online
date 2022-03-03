using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卡牌组件
/// </summary>
public class Card : MonoBehaviour
{
    /// <summary>
    /// 卡图组件
    /// </summary>
    [SerializeField]
    [Header("卡图组件")]
    private Image cardPictureComponent;
    /// <summary>
    /// 鸟群数量角标图片组件
    /// </summary>
    [SerializeField]
    [Header("鸟群数量角标图片组件")]
    private Image groupNumberImageComponent;
    /// <summary>
    /// 鸟类图片组件
    /// </summary>
    [SerializeField]
    [Header("鸟类图片组件")]
    private Image typeImageComponent;
    /// <summary>
    /// 卡图图片
    /// </summary>
    [SerializeField]
    [Header("卡图图片")]
    private Sprite cardPicture;
    /// <summary>
    /// 鸟群数量角标图片
    /// </summary>
    [SerializeField]
    [Header("鸟群数量角标图片")]
    private Sprite groupNumberImage;
    /// <summary>
    /// 鸟类图片
    /// </summary>
    [SerializeField]
    [Header("鸟类图片")]
    private Sprite typeImage;

    /// <summary>
    /// 鸟类
    /// </summary>
    public CardType CardType { get; private set; }
    /// <summary>
    /// 组成小鸟群所需卡牌数量
    /// </summary>
    public int SmallGroupNumber { get; private set; }
    /// <summary>
    /// 组成大鸟群所需卡牌数量
    /// </summary>
    public int BigGroupNumber { get; private set; }

    private void Awake()
    {
        // 设置卡面
        cardPictureComponent.sprite = cardPicture;
        groupNumberImageComponent.sprite = groupNumberImage;
        typeImageComponent.sprite = typeImage;
    }

    private void OnValidate()
    {
        // 卡图
        if (cardPictureComponent != null)
        {
            cardPictureComponent.sprite = cardPicture;
        }
        // 鸟群数量角标
        if (groupNumberImageComponent != null)
        {
            groupNumberImageComponent.sprite = groupNumberImage;
        }
        // 鸟类角标
        if (typeImageComponent != null)
        {
            typeImageComponent.sprite = typeImage;
        }
    }
}
