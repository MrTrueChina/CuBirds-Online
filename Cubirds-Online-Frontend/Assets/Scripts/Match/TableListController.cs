using System.Collections;
using System.Collections.Generic;
using CubirdsOnline.Common;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// 桌子列表面板的控制器
/// </summary>
public class TableListController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static TableListController Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (typeof(TableListController))
            {
                if(instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("TableListController").GetComponent<TableListController>();
                }

                return instance;
            }
        }
    }
    private static TableListController instance;

    /// <summary>
    /// 桌子列表面板物体
    /// </summary>
    [SerializeField]
    [Header("桌子列表面板物体")]
    private GameObject tableListObject;
    /// <summary>
    /// 规则面板物体
    /// </summary>
    [SerializeField]
    [Header("规则面板物体")]
    private GameObject ruleObject;

    /// <summary>
    /// 显示桌子信息的父级
    /// </summary>
    [SerializeField]
    [Header("显示桌子信息的父级")]
    private Transform tableInfoBarListContent;

    /// <summary>
    /// 桌子信息行的预制
    /// </summary>
    [SerializeField]
    [Header("桌子信息行的预制")]
    private Transform tableInfoBarPrefab;

    /// <summary>
    /// 所有的桌子
    /// </summary>
    private List<TableInfoDTO> tablesInfos = new List<TableInfoDTO>();

    private void Start()
    {
        // 桌子列表 UI 是打开场景后就开着的，直接调用显示方法
        Show();
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public void Show()
    {
        Debug.Log("显示桌子列表面板");

        // 显示面板
        tableListObject.SetActive(true);

        // 订阅事件
        PhotonEngine.Subscribe(EventCode.TABLE_CREATED, OnTableCreatedEvent);
        PhotonEngine.Subscribe(EventCode.DISBAND_TABLE, OnTableDisbandedEvent);
        PhotonEngine.Subscribe(EventCode.START_GAME, OnStartGameEvent);

        // 获取所有桌子的信息
        MatchAPI.GetAllTablesInfos(tables =>
        {
            // 获取后保存下来
            tablesInfos.AddRange(tables);

            // 更新桌子列表的显示
            UpdateTableList();
        });
    }

    /// <summary>
    /// 回到连接场景
    /// </summary>
    public void BackToConnect()
    {
        // 断开连接
        PhotonEngine.Instance.Disconnect();

        // 打开连接场景
        SceneManager.LoadScene("Connect Scene");
    }

    /// <summary>
    /// 显示游戏规则
    /// </summary>
    public void ShowRule()
    {
        // 关闭面板
        Close();

        // 打开游戏规则面板
        ruleObject.SetActive(true);
    }

    /// <summary>
    /// 关闭游戏规则
    /// </summary>
    public void CloseRule()
    {
        // 关闭游戏规则面板
        ruleObject.SetActive(false);

        // 显示面板
        Show();
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    private void Close()
    {
        // 关闭面板
        tableListObject.SetActive(false);

        // 清空桌子列表
        tablesInfos.Clear();

        // 销毁所有的桌子信息条
        foreach (Transform child in tableInfoBarListContent.transform)
        {
            Destroy(child.gameObject);
        }

        // 取消订阅事件
        PhotonEngine.Unsubscribe(EventCode.TABLE_CREATED, OnTableCreatedEvent);
        PhotonEngine.Unsubscribe(EventCode.DISBAND_TABLE, OnTableDisbandedEvent);
        PhotonEngine.Unsubscribe(EventCode.START_GAME, OnStartGameEvent);
    }

    /// <summary>
    /// 当收到后台发出的开出新桌子事件时这个方法会被调用
    /// </summary>
    /// <param name="eventData"></param>
    private void OnTableCreatedEvent(EventData eventData)
    {
        // 获取新添加的桌子
        TableInfoDTO newTable = new TableInfoDTO(eventData.Parameters.Get<object[]>(EventParamaterKey.TABLE_INFO));

        Debug.LogFormat("收到新添加桌子消息，新添加桌子 {0}", newTable.Id);

        // 把桌子添加到列表里
        tablesInfos.Add(newTable);

        // 更新桌子列表的显示
        UpdateTableList();
    }

    /// <summary>
    /// 当收到后台发出的解散桌子事件时这个方法会被调用
    /// </summary>
    /// <param name="eventData"></param>
    private void OnTableDisbandedEvent(EventData eventData)
    {
        // 获取解散的桌子的 ID
        int disbandedTableId = eventData.Parameters.Get<int>(EventParamaterKey.TABLE_ID);

        Debug.LogFormat("收到解散桌子消息，解散桌子 {0}", disbandedTableId);

        // 从桌子列表里删除被解散的桌子
        tablesInfos.RemoveAll(t => t.Id == disbandedTableId);

        // 更新桌子列表的显示
        UpdateTableList();
    }

    /// <summary>
    /// 当收到后台发出的解散桌子事件时这个方法会被调用
    /// </summary>
    /// <param name="eventData"></param>
    private void OnStartGameEvent(EventData eventData)
    {
        // 获取开始游戏的桌子的 ID
        int startGameTableId = eventData.Parameters.Get<int>(EventParamaterKey.TABLE_ID);

        Debug.LogFormat("收到开始游戏消息，桌子 {0} 开始游戏", startGameTableId);

        // 从桌子列表里删除开始游戏的桌子
        tablesInfos.RemoveAll(t => t.Id == startGameTableId);

        // 更新桌子列表的显示
        UpdateTableList();
    }

    /// <summary>
    /// 更新桌子列表的显示
    /// </summary>
    private void UpdateTableList()
    {
        // 销毁所有的桌子信息条
        foreach(Transform child in tableInfoBarListContent.transform)
        {
            Destroy(child.gameObject);
        }

        // 遍历所有桌子信息添加桌子信息条
        tablesInfos.ForEach(table =>
        {
            Instantiate(tableInfoBarPrefab, tableInfoBarListContent).GetComponent<TableInfoBar>().Init(table);
        });
    }

    /// <summary>
    /// 加入一个桌子
    /// </summary>
    /// <param name="tableInfo"></param>
    public void JoinTable(TableInfoDTO tableInfo)
    {
        Debug.LogFormat("加入桌子 {0} {1}", tableInfo.Name, tableInfo.Id);

        if (tableInfo.HavePassword)
        {
            // 桌子有密码，先输入密码之后发出请求
            TextInputPanel.Show("请输入密码", password => SendJoinTableRequest(tableInfo.Id, password));
        }
        else
        {
            // 桌子没有密码，密码可以直接填空，发出请求
            SendJoinTableRequest(tableInfo.Id, "");
        }
    }

    /// <summary>
    /// 发出加入桌子请求
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="password"></param>
    private void SendJoinTableRequest(int tableId, string password)
    {
        // 发出加入桌子请求
        MatchAPI.JoinTable(tableId, password, paramater =>
        {
            if (paramater.Get<bool>(ResponseParamaterKey.SUCCESS))
            {
                // 加入成功

                // 设置返回的桌子为已加入的桌子
                GlobalModel.Instance.TableInfo = new TableInfoDTO(paramater.Get<object[]>(ResponseParamaterKey.TABLE_INFO));

                // 关闭面板
                Close();

                // 打开桌子面板
                TableCanvasController.Instance.Show();
            }
            else
            {
                // 加入失败

                // 显示加入失败原因
                InfoDialogController.Show("加入失败：" + paramater.Get<string>(ResponseParamaterKey.ERROR_MESSAGE_STRING), 2);
            }
        });
    }

    /// <summary>
    /// 开一个新的桌子
    /// </summary>
    public void CreateTable()
    {
        Debug.Log("开新桌子");

        // 关闭面板
        Close();

        // 打开开新桌面板
        CreateTableController.Instance.Show();
    }
}
