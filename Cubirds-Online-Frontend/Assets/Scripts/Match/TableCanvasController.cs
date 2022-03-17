using System.Collections;
using System.Collections.Generic;
using CubirdsOnline.Common;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 桌子内容面板的控制器
/// </summary>
public class TableCanvasController : MonoBehaviour
{
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
    /// 显示面板
    /// </summary>
    public void Show()
    {
        Debug.Log("显示桌子面板");

        // 显示面板
        tableCanvas.SetActive(true);

        // 订阅事件
        PhotonEngine.Subscribe(EventCode.PLAYER_QUIT_TABLE, OnPlayerQuitEvent);
        PhotonEngine.Subscribe(EventCode.DISBAND_TABLE, OnDisbandTableEvent);
        PhotonEngine.Subscribe(EventCode.PLAYER_JOIN_TABLE, OnPlayerJoinTableEvent);

        // 检测并记录本机玩家是不是房主
        bool isMaster = GlobalModel.Instance.TableInfo.MasterId == GlobalModel.Instance.LocalPLayerId;
        // 根据是不是房主显示不同的按钮
        startButton.gameObject.SetActive(isMaster);
        quitButton.gameObject.SetActive(!isMaster);
        disbandButton.gameObject.SetActive(isMaster);

        MatchAPI.GetAllPlayersOnTable(GlobalModel.Instance.TableInfo.Id, players =>
        {
            // 记录玩家列表
            GlobalModel.Instance.TablePlayers = players;

            // 更新玩家列表的显示
            UpdatePlayersDisplay();
        });
    }

    /// <summary>
    /// 当收到玩家退出桌子事件时这个方法会被调用
    /// </summary>
    /// <param name="eventData"></param>
    private void OnPlayerQuitEvent(EventData eventData)
    {
        // 获取退出桌子的玩家的 ID
        int quitPlayerId = eventData.Parameters.Get<int>(EventParamaterKey.PLAYER_ID);

        Debug.LogFormat("玩家 {0} 退出桌子", quitPlayerId);

        if(quitPlayerId != GlobalModel.Instance.LocalPLayerId)
        {
            // 如果不是本机玩家退出桌子

            // 把这个 ID 的玩家移除出玩家列表
            GlobalModel.Instance.TablePlayers.RemoveAll(p => p.Id == quitPlayerId);

            // 更新玩家列表显示
            UpdatePlayersDisplay();
        }
        else
        {
            // 如果是本机玩家退出桌子

            // 回到桌子列表面板
            BackToTableList();
        }
    }

    /// <summary>
    /// 当收到玩家退出桌子事件时这个方法会被调用
    /// </summary>
    /// <param name="eventData"></param>
    private void OnDisbandTableEvent(EventData eventData)
    {
        Debug.Log("桌子解散");

        // 回到桌子列表面板
        BackToTableList();
    }

    /// <summary>
    /// 当收到玩家加入桌子事件时这个方法会被调用
    /// </summary>
    /// <param name="eventData"></param>
    private void OnPlayerJoinTableEvent(EventData eventData)
    {
        // 获取玩家信息
        PlayerInfoDTO playerInfo = new PlayerInfoDTO(eventData.Parameters.Get<object[]>(EventParamaterKey.PLAYER_INFO));

        Debug.LogFormat("玩家 {0} 加入桌子", playerInfo.Id);

        // 把玩家添加到玩家列表里
        GlobalModel.Instance.TablePlayers.Add(playerInfo);

        // 更新玩家列表的显示
        UpdatePlayersDisplay();
    }

    /// <summary>
    /// 回到桌子列表面板
    /// </summary>
    private void BackToTableList()
    {
        // 关闭面板
        Close();

        // 打开桌子列表面板
        TableListController.Instance.Show();
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

        // 发出退出请求
        MatchAPI.QuitTable(GlobalModel.Instance.TableInfo.Id, quitSuccess => { });
    }

    /// <summary>
    /// 解散桌子
    /// </summary>
    public void Disband()
    {
        Debug.Log("解散桌子");

        // 发出解散桌子请求
        MatchAPI.DisbandTable(GlobalModel.Instance.TableInfo.Id, quitSuccess => { });
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    private void Close()
    {
        Debug.Log("关闭桌子信息面板");

        // 移除玩家列表
        GlobalModel.Instance.TablePlayers = null;

        // 移除桌子记录
        GlobalModel.Instance.TableInfo = null;

        // 关闭面板
        tableCanvas.SetActive(false);

        // 取消订阅事件
        PhotonEngine.Unsubscribe(EventCode.PLAYER_QUIT_TABLE, OnPlayerQuitEvent);
        PhotonEngine.Unsubscribe(EventCode.DISBAND_TABLE, OnDisbandTableEvent);
        PhotonEngine.Unsubscribe(EventCode.PLAYER_JOIN_TABLE, OnPlayerJoinTableEvent);
    }
}
