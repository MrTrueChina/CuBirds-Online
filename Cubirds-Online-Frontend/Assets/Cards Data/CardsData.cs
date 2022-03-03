using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有卡牌的数据
/// </summary>
[CreateAssetMenu(fileName = "CardsData", menuName = "CuDirds-Online/CardData")]
public class CardsData : ScriptableObject
{
    /// <summary>
    /// 卡牌数据
    /// </summary>
    [SerializeField]
    public List<CardData> cardsData;
}
