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
    /// 获取服务器配置列表的 URL
    /// </summary>
    private const string GET_SERVER_CONFIG_URL = "https://gitcode.net/M_t_C/Get-Config-From-Github-Development-And-Test/-/raw/main/Config/CO-Test-Config.json";

    #region 单例部分
    /// <summary>
    /// 实例
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
    /// 昵称输入框
    /// </summary>
    [SerializeField]
    [Header("昵称输入框")]
    private InputField nameInputField;

    /// <summary>
    /// 选择服务器画布
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("选择服务器画布")]
    private GameObject selectServerCanvas;
    /// <summary>
    /// 服务器按钮容器
    /// </summary>
    [SerializeField]
    [Header("服务器按钮容器")]
    private Transform serverButtonContentTransform;
    /// <summary>
    /// 服务器按钮预制
    /// </summary>
    [SerializeField]
    [Header("服务器按钮预制")]
    private GameObject serverButtonPrefab;

    /// <summary>
    /// 手输服务器画布
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("手输服务器画布")]
    private GameObject inputServerCanvas;
    /// <summary>
    /// 游戏服务器输入框
    /// </summary>
    [SerializeField]
    [Header("游戏服务器输入框")]
    private InputField gameServerInputField;
    /// <summary>
    /// 游戏服务器名称输入框
    /// </summary>
    [SerializeField]
    [Header("游戏服务器名称输入框")]
    private InputField gameServerNameInputField;
    /// <summary>
    /// 更新服务器输入框
    /// </summary>
    [SerializeField]
    [Header("更新服务器输入框")]
    private InputField updateServerInputField;

    /// <summary>
    /// 服务器配置列表
    /// </summary>
    private List<Dictionary<string, string>> serverConfigs;

    /// <summary>
    /// 是否已经开始连接
    /// </summary>
    private bool connectStarted = false;
    /// <summary>
    /// 连接计时器
    /// </summary>
    private float connectTimer;

    private void Start()
    {
        StartCoroutine(GetServerConfig(GET_SERVER_CONFIG_URL));
    }

    #region 获取服务器配置部分
    /// <summary>
    /// 获取服务器配置的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetServerConfig(string url)
    {
        Debug.Log("开始获取服务器信息");

        // 显示信息对话框
        InfoDialogController.Instance.Show("获取服务器信息");

        // 准备一个字典树组来接收配置信息
        List<Dictionary<string, string>> serverConfigList;

        // 使用 using 来自动解决请求释放的问题
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 发出请求并等待响应
            yield return webRequest.SendWebRequest();

            // 如果发生连接错误
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("获取服务器配置列表时发生连接错误: " + webRequest.error);

                // 直接结束协程
                yield break;
            }

            // 把获取到的 JSON 字符串转为字典列表
            serverConfigList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(webRequest.downloadHandler.text);
        }

        Debug.Log("获取到服务器配置列表：");
        serverConfigList.ForEach(config => {
            Debug.Log(config["name"] + " | " + config["type"] + " | " + config["gameServer"] + " | " + config["hotUpdateServer"]);
        });

        // 关闭对话框
        InfoDialogController.Instance.Close();

        // 保存到服务器配置列表里
        serverConfigs = serverConfigList;

        // 显示服务器按钮
        ShowServerButtons();
    }
    #endregion

    /// <summary>
    /// 显示选择连接服务器的按钮
    /// </summary>
    private void ShowServerButtons()
    {
        // 删除现有的所有按钮
        foreach(Transform child in serverButtonContentTransform)
        {
            Destroy(child.gameObject);
        }

        // 遍历服务器配置
        serverConfigs.ForEach(config => 
        {
            // 实例化按钮 -> 获取按钮控制组件 -> 初始化
            Instantiate(serverButtonPrefab, serverButtonContentTransform).GetComponent<ServerButtonController>().Setup(config);
        });
    }

    /// <summary>
    /// 切换到选择服务器的 UI
    /// </summary>
    public void ToSelectServerUI()
    {
        selectServerCanvas.SetActive(true);
        inputServerCanvas.SetActive(false);
    }

    /// <summary>
    /// 切换到手输服务器的 UI
    /// </summary>
    public void ToInputServerUI()
    {
        selectServerCanvas.SetActive(false);
        inputServerCanvas.SetActive(true);
    }

    /// <summary>
    /// 进行连接
    /// </summary>
    /// <param name="config"></param>
    public void StartConnect(Dictionary<string, string> config)
    {
        // 没有写名字则发出提示
        if (nameInputField.text == null || nameInputField.text.Length == 0)
        {
            InfoDialogController.Instance.Show("请填写名称", 1);
            return;
        }

        // 开始连接
        PhotonEngine.Instance.StartConnect(config["gameServer"], config["gameServerName"]);

        // 记录开始连接
        connectStarted = true;

        // 显示提示
        InfoDialogController.Instance.Show("开始连接至" + config["name"]);

        // 给计时器进行连接的时间
        connectTimer = 10;
    }

    private void Update()
    {
        // 如果已经开始连接，进行对连接是否成功的处理
        if (connectStarted)
        {
            // 如果连接状态是已断开，说明连接失败
            if (PhotonEngine.Peer.PeerState == PeerStateValue.Disconnected)
            {
                // 显示提示
                InfoDialogController.Instance.Show("连接失败", 1);

                // 改为没有开始连接状态
                connectStarted = false;

                // 确认连接失败后就不需要后续操作了
                return;
            }

            // 如果连接状态是已连接，说明连接成功
            if (PhotonEngine.Peer.PeerState == PeerStateValue.Connected)
            {
                // 显示提示
                InfoDialogController.Instance.Show("连接成功", 1);

                //// 获取本机玩家 ID
                //PlayerAPI.GetLocalPlayerId(localPlayerId =>
                //{
                //    Debug.LogFormat("获取本地玩家 ID = {0}", localPlayerId);

                //    // 保存本机玩家 ID
                //    GlobalModel.Instance.LocalPLayerId = localPlayerId;

                //    // 设置玩家名称
                //    PlayerAPI.SetPLayerName(nameInputField.text, success =>
                //    {
                //        // 切换到桌子列表面板
                //        ToTablesCanvas();
                //    }, () =>
                //    {
                //        // 连接超时，发出信息
                //        connectInfoText.text = "设置名称失败，请尝试重连";
                //    });
                //}, () =>
                //{
                //    // 连接超时，发出信息
                //    connectInfoText.text = "获取 ID 失败，请尝试重连";
                //});

                // 改为没有开始连接状态
                connectStarted = false;

                // 确认连接成功后就不需要后续操作了
                return;
            }

            // 对连接进行计时
            connectTimer -= Time.deltaTime;

            // 计时结束
            if (connectTimer <= 0)
            {
                // 显示提示
                InfoDialogController.Instance.Show("连接超时", 1);

                // 改为没有开始连接状态
                connectStarted = false;
            }
        }
    }

    /// <summary>
    /// 退出游戏
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
