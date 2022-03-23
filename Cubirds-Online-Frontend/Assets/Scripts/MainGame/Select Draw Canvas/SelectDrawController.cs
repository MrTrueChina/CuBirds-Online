using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家选择是否抽卡的操作面板的控制器
/// </summary>
public class SelectDrawController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static SelectDrawController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(SelectDrawController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindWithTag("SelectDrawController").GetComponent<SelectDrawController>();
                }

                return instance;
            }
        }
    }
    private static SelectDrawController instance;

    /// <summary>
    /// 选择是否抽卡的面板
    /// </summary>
    [SerializeField]
    [Header("选择是否抽卡的面板")]
    GameObject selectDrawCardsCanvas;

    /// <summary>
    /// 开始选择是否抽卡
    /// </summary>
    public void StartSelect()
    {
        // 激活操作面板
        selectDrawCardsCanvas.SetActive(true);
    }

    /// <summary>
    /// 选择抽卡
    /// </summary>
    public void SelectDrawCards()
    {
        // 通知输入控制器转发选择抽卡消息
        InputController.Instance.CallPlayerDrawCards(GameController.Instance.CurrentTrunPlayre.Id);

        // 关闭操作面板
        Close();
    }

    /// <summary>
    /// 选择不抽卡
    /// </summary>
    public void SelectDontDrawCards()
    {
        // 通知输入控制器转发选择不抽卡消息
        InputController.Instance.CallPlayerDontDrawCards(GameController.Instance.CurrentTrunPlayre.Id);

        // 关闭操作面板
        Close();
    }

    /// <summary>
    /// 关闭操作面板
    /// </summary>
    public void Close()
    {
        // 关闭操作面板
        selectDrawCardsCanvas.SetActive(false);
    }
}
