using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 卡牌种类（鸟类）
/// </summary>
[Serializable]
public enum CardType : int
{
    /// <summary>
    /// 火烈鸟
    /// </summary>
    [InspectorName("火烈鸟")]
    PHOENICOPTERIDAE = 1,
    /// <summary>
    /// 猫头鹰
    /// </summary>
    [InspectorName("猫头鹰")]
    STRIGIFORMES = 2,
    /// <summary>
    /// 鵎鵼（巨嘴鸟）
    /// </summary>
    [InspectorName("鵎鵼（巨嘴鸟）")]
    RAMPHASTIDAE = 3,
    /// <summary>
    /// 绿头鸭
    /// </summary>
    [InspectorName("绿头鸭")]
    ANAS_PLATYRHYNCHOS = 4,
    /// <summary>
    /// 金刚鹦鹉
    /// </summary>
    [InspectorName("金刚鹦鹉")]
    PSITTACIDAE = 5,
    /// <summary>
    /// 喜鹊
    /// </summary>
    [InspectorName("喜鹊")]
    PICA_PICA = 6,
    /// <summary>
    /// 东方木皮威
    /// </summary>
    [InspectorName("东方木皮威")]
    EASTERN_WOOD_PEWEE = 7,
    /// <summary>
    /// 知更鸟
    /// </summary>
    [InspectorName("知更鸟")]
    ROBIN = 8,
}
