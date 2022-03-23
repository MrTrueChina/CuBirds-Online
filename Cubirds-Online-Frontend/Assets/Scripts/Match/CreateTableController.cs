using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开新桌子面板的控制器
/// </summary>
public class CreateTableController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static CreateTableController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(CreateTableController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("CreateTableController").GetComponent<CreateTableController>();
                }

                return instance;
            }
        }
    }
    private static CreateTableController instance;

    /// <summary>
    /// 开新桌子面板
    /// </summary>
    [SerializeField]
    [Header("开新桌子面板")]
    private GameObject createTableCanvas;

    /// <summary>
    /// 桌子名称输入框
    /// </summary>
    [SerializeField]
    [Header("桌子名称输入框")]
    private InputField tableNameInput;
    /// <summary>
    /// 密码输入框
    /// </summary>
    [SerializeField]
    [Header("密码输入框")]
    private InputField passwordInput;

    /// <summary>
    /// 显示面板
    /// </summary>
    public void Show()
    {
        // 显示面板
        createTableCanvas.SetActive(true);
    }

    /// <summary>
    /// 开新桌
    /// </summary>
    public void CreateTable()
    {
        MatchAPI.CreateTable(tableNameInput.text, passwordInput.text, newTableInfo =>
        {
            Debug.LogFormat("开出新桌子 {0} {1}", newTableInfo.Id, newTableInfo.Name);

            // 设置返回的桌子为已加入的桌子
            GlobalModel.Instance.TableInfo = newTableInfo;

            // 关闭面板
            Close();

            // 打开桌子面板
            TableCanvasController.Instance.Show();
        });
    }

    /// <summary>
    /// 回到桌子列表面板
    /// </summary>
    public void BackToTableListCanvas()
    {
        // 关闭面板
        Close();

        // 打开桌子列表面板
        TableListController.Instance.Show();
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    private void Close()
    {
        // 关闭面板
        createTableCanvas.SetActive(false);

        // 清空输入框
        tableNameInput.text = "";
    }
}
