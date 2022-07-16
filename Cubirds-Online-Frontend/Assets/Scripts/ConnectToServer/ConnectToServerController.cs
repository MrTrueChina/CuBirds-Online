using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ConnectToServerController : MonoBehaviour
{
    /// <summary>
    /// ��ȡ�����������б�� URL
    /// </summary>
    private const string GET_SERVER_CONFIG_URL = "https://gitcode.net/M_t_C/Get-Config-From-Github-Development-And-Test/-/raw/main/Config/CO-Test-Config.json";

    #region ��������
    /// <summary>
    /// ʵ��
    /// </summary>
    public static ConnectToServerController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(ConnectToServerController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("ConnectToServer").GetComponent<ConnectToServerController>();
                }

                return instance;
            }
        }
    }
    private static ConnectToServerController instance;
    #endregion

    /// <summary>
    /// �ǳ������
    /// </summary>
    [SerializeField]
    [Header("�ǳ������")]
    private InputField nameInputField;

    /// <summary>
    /// ѡ�����������
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("ѡ�����������")]
    private GameObject selectServerCanvas;
    /// <summary>
    /// ��������ť����
    /// </summary>
    [SerializeField]
    [Header("��������ť����")]
    private Transform serverButtonContentTransform;
    /// <summary>
    /// ��������ťԤ��
    /// </summary>
    [SerializeField]
    [Header("��������ťԤ��")]
    private GameObject serverButtonPrefab;

    /// <summary>
    /// �������������
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("�������������")]
    private GameObject inputServerCanvas;
    /// <summary>
    /// ��Ϸ�����������
    /// </summary>
    [SerializeField]
    [Header("��Ϸ�����������")]
    private InputField gameServerInputField;
    /// <summary>
    /// ��Ϸ���������������
    /// </summary>
    [SerializeField]
    [Header("��Ϸ���������������")]
    private InputField gameServerNameInputField;
    /// <summary>
    /// ���·����������
    /// </summary>
    [SerializeField]
    [Header("���·����������")]
    private InputField updateServerInputField;

    /// <summary>
    /// �����������б�
    /// </summary>
    private List<Dictionary<string, string>> serverConfigs;

    /// <summary>
    /// �Ƿ��Ѿ���ʼ����
    /// </summary>
    private bool connectStarted = false;
    /// <summary>
    /// ���Ӽ�ʱ��
    /// </summary>
    private float connectTimer;

    private void Start()
    {
        StartCoroutine(GetServerConfig(GET_SERVER_CONFIG_URL));
    }

    #region ��ȡ���������ò���
    /// <summary>
    /// ��ȡ���������õ�Э��
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetServerConfig(string url)
    {
        Debug.Log("��ʼ��ȡ��������Ϣ");

        // ��ʾ��Ϣ�Ի���
        InfoDialogController.Instance.Show("��ȡ��������Ϣ");

        // ׼��һ���ֵ�����������������Ϣ
        List<Dictionary<string, string>> serverConfigList;

        // ʹ�� using ���Զ���������ͷŵ�����
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // �������󲢵ȴ���Ӧ
            yield return webRequest.SendWebRequest();

            // ����������Ӵ���
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("��ȡ�����������б�ʱ�������Ӵ���: " + webRequest.error);

                // ֱ�ӽ���Э��
                yield break;
            }

            // �ѻ�ȡ���� JSON �ַ���תΪ�ֵ��б�
            serverConfigList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(webRequest.downloadHandler.text);
        }

        Debug.Log("��ȡ�������������б�");
        serverConfigList.ForEach(config => {
            Debug.Log(config["name"] + " | " + config["type"] + " | " + config["gameServer"] + " | " + config["hotUpdateServer"]);
        });

        // �رնԻ���
        InfoDialogController.Instance.Close();

        // ���浽�����������б���
        serverConfigs = serverConfigList;

        // ��ʾ��������ť
        ShowServerButtons();
    }
    #endregion

    /// <summary>
    /// ��ʾѡ�����ӷ������İ�ť
    /// </summary>
    private void ShowServerButtons()
    {
        // ɾ�����е����а�ť
        foreach(Transform child in serverButtonContentTransform)
        {
            Destroy(child.gameObject);
        }

        // ��������������
        serverConfigs.ForEach(config => 
        {
            // ʵ������ť -> ��ȡ��ť������� -> ��ʼ��
            Instantiate(serverButtonPrefab, serverButtonContentTransform).GetComponent<ServerButtonController>().Setup(config);
        });
    }

    /// <summary>
    /// �л���ѡ��������� UI
    /// </summary>
    public void ToSelectServerUI()
    {
        selectServerCanvas.SetActive(true);
        inputServerCanvas.SetActive(false);
    }

    /// <summary>
    /// �л�������������� UI
    /// </summary>
    public void ToInputServerUI()
    {
        selectServerCanvas.SetActive(false);
        inputServerCanvas.SetActive(true);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="config"></param>
    public void StartConnect(Dictionary<string, string> config)
    {
        // û��д�����򷢳���ʾ
        if (nameInputField.text == null || nameInputField.text.Length == 0)
        {
            InfoDialogController.Instance.Show("����д����", 1);
            return;
        }

        // ��ʼ����
        PhotonEngine.Instance.StartConnect(config["gameServer"], config["gameServerName"]);

        // ��¼��ʼ����
        connectStarted = true;

        // ��ʾ��ʾ
        InfoDialogController.Instance.Show("��ʼ������" + config["name"]);

        // ����ʱ���������ӵ�ʱ��
        connectTimer = 10;
    }

    private void Update()
    {
        // ����Ѿ���ʼ���ӣ����ж������Ƿ�ɹ��Ĵ���
        if (connectStarted)
        {
            // �������״̬���ѶϿ���˵������ʧ��
            if (PhotonEngine.Peer.PeerState == PeerStateValue.Disconnected)
            {
                // ��ʾ��ʾ
                InfoDialogController.Instance.Show("����ʧ��", 1);

                // ��Ϊû�п�ʼ����״̬
                connectStarted = false;

                // ȷ������ʧ�ܺ�Ͳ���Ҫ����������
                return;
            }

            // �������״̬�������ӣ�˵�����ӳɹ�
            if (PhotonEngine.Peer.PeerState == PeerStateValue.Connected)
            {
                // ��ʾ��ʾ
                InfoDialogController.Instance.Show("���ӳɹ�", 1);

                //// ��ȡ������� ID
                //PlayerAPI.GetLocalPlayerId(localPlayerId =>
                //{
                //    Debug.LogFormat("��ȡ������� ID = {0}", localPlayerId);

                //    // ���汾����� ID
                //    GlobalModel.Instance.LocalPLayerId = localPlayerId;

                //    // �����������
                //    PlayerAPI.SetPLayerName(nameInputField.text, success =>
                //    {
                //        // �л��������б����
                //        ToTablesCanvas();
                //    }, () =>
                //    {
                //        // ���ӳ�ʱ��������Ϣ
                //        connectInfoText.text = "��������ʧ�ܣ��볢������";
                //    });
                //}, () =>
                //{
                //    // ���ӳ�ʱ��������Ϣ
                //    connectInfoText.text = "��ȡ ID ʧ�ܣ��볢������";
                //});

                // ��Ϊû�п�ʼ����״̬
                connectStarted = false;

                // ȷ�����ӳɹ���Ͳ���Ҫ����������
                return;
            }

            // �����ӽ��м�ʱ
            connectTimer -= Time.deltaTime;

            // ��ʱ����
            if (connectTimer <= 0)
            {
                // ��ʾ��ʾ
                InfoDialogController.Instance.Show("���ӳ�ʱ", 1);

                // ��Ϊû�п�ʼ����״̬
                connectStarted = false;
            }
        }
    }

    /// <summary>
    /// �˳���Ϸ
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
