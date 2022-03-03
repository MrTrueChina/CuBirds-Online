using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 这个玩家的 Id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 这个玩家的上方向
    /// </summary>
    public Vector3 upDirection = Vector3.up;

    /// <summary>
    /// 手牌
    /// </summary>
    private List<Card> cards = new List<Card>();
    /// <summary>
    /// 鸟群卡
    /// </summary>
    private List<Card> groupCards = new List<Card>();
}
