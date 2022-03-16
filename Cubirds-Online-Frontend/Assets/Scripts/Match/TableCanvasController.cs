using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 桌子内容面板的控制器
/// </summary>
public class TableCanvasController : MonoBehaviour
{
    /// <summary>
    /// 桌子信息画布
    /// </summary>
    [SerializeField]
    [Header("桌子信息画布")]
    private GameObject tableCanvas;
    /// <summary>
    /// 玩家信息条容器
    /// </summary>
    [SerializeField]
    [Header("玩家信息条容器")]
    private Transform playersInfosContent;
    /// <summary>
    /// 玩家数量文本
    /// </summary>
    [SerializeField]
    [Header("玩家数量文本")]
    private Text playersNumberText;
    /// <summary>
    /// 开始游戏按钮
    /// </summary>
    [SerializeField]
    [Header("开始游戏按钮")]
    private Button startButton;
    /// <summary>
    /// 退出桌子按钮
    /// </summary>
    [SerializeField]
    [Header("退出桌子按钮")]
    private Button quitButton;
    /// <summary>
    /// 解散桌子按钮
    /// </summary>
    [SerializeField]
    [Header("解散桌子按钮")]
    private Button disbandButton;

    /// <summary>
    /// 玩家名称条预制
    /// </summary>
    [SerializeField]
    [Header("玩家名称条预制")]
    private GameObject playerNameBarPrefab;

    /// <summary>
    /// 实例
    /// </summary>
    public static TableCanvasController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(TableCanvasController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("TableCanvasController").GetComponent<TableCanvasController>();
                }

                return instance;
            }
        }
    }
    private static TableCanvasController instance;

    /// <summary>
    /// 显示面板
    /// </summary>
    public void Show()
    {
        Debug.Log("显示桌子面板");

        // 显示面板
        tableCanvas.SetActive(true);

        MatchAPI.GetAllPlayersOnTable(GlobalModel.Instance.TableInfo.Id, players =>
        {
            // 记录玩家列表
            GlobalModel.Instance.TablePlayers = players;

            // 更新玩家列表的显示
            UpdatePlayersDisplay();
        });
    }

    /// <summary>
    /// 更新玩家列表的显示
    /// </summary>
    private void UpdatePlayersDisplay()
    {
        Debug.Log("更新玩家列表显示");

        // 销毁所有玩家
        foreach(Transform playerTransform in playersInfosContent.transform)
        {
            Destroy(playerTransform.gameObject);
        }

        // 重新实例化所有玩家名称条
        for(int i = 0;i < GlobalModel.Instance.TablePlayers.Count; i++)
        {
            // 实例化并直接设置名称显示
            Instantiate(playerNameBarPrefab, playersInfosContent).GetComponentInChildren<Text>().text = string.Format("玩家{0}：{1}", i + 1, GlobalModel.Instance.TablePlayers[i].Name);
        }
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void Start()
    {
        Debug.Log("开始游戏");
    }

    /// <summary>
    /// 退出桌子
    /// </summary>
    public void Quit()
    {
        Debug.Log("退出桌子");
    }

    /// <summary>
    /// 解散桌子
    /// </summary>
    public void Disband()
    {
        Debug.Log("解散桌子");
    }
}
