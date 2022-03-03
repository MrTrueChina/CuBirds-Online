using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 单张卡片的数据
/// </summary>
[Serializable]
public class CardData
{
    /// <summary>
    /// 名称，仅用于编辑时进行区分
    /// </summary>
    public string name;
    /// <summary>
    /// 鸟类
    /// </summary>
    public CardType cardType;
    /// <summary>
    /// 卡图图片
    /// </summary>
    public Sprite cardPicture;
    /// <summary>
    /// 鸟群数量角标图片
    /// </summary>
    public Sprite groupNumberImage;
    /// <summary>
    /// 鸟类角标图片
    /// </summary>
    public Sprite typeImage;
    /// <summary>
    /// 组成小鸟群所需卡牌数量
    /// </summary>
    public int smallGroupNumber;
    /// <summary>
    /// 组成大鸟群所需卡牌数量
    /// </summary>
    public int bigGroupNumber;
    /// <summary>
    /// 这种卡牌的数量
    /// </summary>
    public int cardNumber;
}
