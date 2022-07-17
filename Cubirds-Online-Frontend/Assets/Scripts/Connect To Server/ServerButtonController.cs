using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������б�İ�ťԤ��
/// </summary>
public class ServerButtonController : MonoBehaviour
{
    /// <summary>
    /// ��ť�ı����
    /// </summary>
    [SerializeField]
    private Text buttonText;

    /// <summary>
    /// �����ť����ķ�����������Ϣ
    /// </summary>
    private Dictionary<string, string> serverConfig;

    /// <summary>
    /// ��ʼ����ť
    /// </summary>
    /// <param name="serverConfig"></param>
    public void Setup(Dictionary<string,string> serverConfig)
    {
        // ��������
        this.serverConfig = serverConfig;

        // ���ð�ť�ı�
        buttonText.text = serverConfig["name"];
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void Connect()
    {
        // ʹ�����ý�������
        ConnectToServerController.Instance.StartConnect(serverConfig);
    }
}
