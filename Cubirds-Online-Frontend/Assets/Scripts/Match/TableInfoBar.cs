using System.Collections;
using System.Collections.Generic;
using CubirdsOnline.Common;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 桌子信息条
/// </summary>
public class TableInfoBar : MonoBehaviour
{
    /// <summary>
    /// 显示 ID 的文本
    /// </summary>
    [SerializeField]
    [Header("显示 ID 的文本")]
    private Text idText;
    /// <summary>
    /// 显示名字的文本
    /// </summary>
    [SerializeField]
    [Header("显示名字的文本")]
    private Text nameText;

    /// <summary>
    /// 这个信息条的桌子信息
    /// </summary>
    private TableInfoDTO tableInfo;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="tableInfo"></param>
    public void Init(TableInfoDTO tableInfo)
    {
        // 保存信息
        this.tableInfo = tableInfo;

        // 显示 ID
        idText.text = tableInfo.Id.ToString();

        // 显示名称
        nameText.text = tableInfo.Name;
    }

    /// <summary>
    /// 加入桌子
    /// </summary>
    public void JoinTable()
    {
        TableListController.Instance.JoinTable(tableInfo);
    }
}
