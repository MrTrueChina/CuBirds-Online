using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 服务器列表的按钮预制
/// </summary>
public class ServerButtonController : MonoBehaviour
{
    /// <summary>
    /// 按钮文本组件
    /// </summary>
    [SerializeField]
    private Text buttonText;

    /// <summary>
    /// 这个按钮保存的服务器配置信息
    /// </summary>
    private Dictionary<string, string> serverConfig;

    /// <summary>
    /// 初始化按钮
    /// </summary>
    /// <param name="serverConfig"></param>
    public void Setup(Dictionary<string,string> serverConfig)
    {
        // 保存配置
        this.serverConfig = serverConfig;

        // 设置按钮文本
        buttonText.text = serverConfig["name"];
    }

    /// <summary>
    /// 进行连接
    /// </summary>
    public void Connect()
    {
        // 使用配置进行连接
        ConnectToServerController.Instance.StartConnect(serverConfig);
    }
}
