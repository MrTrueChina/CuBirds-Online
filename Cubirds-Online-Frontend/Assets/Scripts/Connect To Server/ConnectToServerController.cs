using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class ConnectToServerController : MonoBehaviour
{
    /// <summary>
    /// 获取服务器配置列表的 URL
    /// </summary>
    private const string GET_SERVER_CONFIG_URL = "https://gitcode.net/M_t_C/Get-Config-From-Github-Development-And-Test/-/raw/main/Config/CO-Test-Config.json";
    /// <summary>
    /// 检测热更新版本的 URL 中缀
    /// </summary>
    private const string CHECK_UPDATE_URL_INFIX = "/hotUpdate/check";
    /// <summary>
    /// 下载资源包的 URL 中缀
    /// </summary>
    private const string DOWNLOAD_ASSETBUNDLE_URL_INFIX = "/hotUpdate/downloadAssetBundle?assetBundleName=";
    /// <summary>
    /// 下载 manifest 文件的 URL 中缀
    /// </summary>
    private const string DOWNLOAD_MANIFEST_URL_INFIX = "/hotUpdate/downloadManifest?assetBundleName=";
    /// <summary>
    /// 本地的记录包版本号的文件的文件名
    /// </summary>
    private const string ASSETBUNDLES_VERSION_JSON_FILE_NAME = "AssetBundlesVerisonCheck";

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
    /// 连接到的服务器的配置
    /// </summary>
    private Dictionary<string, string> conenctServerConfig;

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
        InfoDialogController infoDialog = InfoDialogController.Show("获取服务器信息");

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
        infoDialog.Close();

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

    #region 连接到游戏服务器部分
    /// <summary>
    /// 进行连接
    /// </summary>
    /// <param name="config"></param>
    public void StartConnect(Dictionary<string, string> config)
    {
        // 没有写名字则发出提示
        if (nameInputField.text == null || nameInputField.text.Length == 0)
        {
            InfoDialogController.Show("请填写名称", 1);
            return;
        }

        // 记录连接到的服务器
        conenctServerConfig = config;

        // 开始连接
        PhotonEngine.Instance.StartConnect(config["gameServer"], config["gameServerName"]);

        // 记录开始连接
        connectStarted = true;

        // 显示提示
        InfoDialogController.Show("开始连接至" + config["name"], 1);

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
                InfoDialogController.Show("连接失败", 1);

                // 改为没有开始连接状态
                connectStarted = false;

                // 确认连接失败后就不需要后续操作了
                return;
            }

            // 如果连接状态是已连接，说明连接成功
            if (PhotonEngine.Peer.PeerState == PeerStateValue.Connected)
            {
                //// 显示提示
                //InfoDialogController.Show("连接成功", 1);

                // 获取本机玩家 ID
                PlayerAPI.GetLocalPlayerId(localPlayerId =>
                {
                    Debug.LogFormat("获取本地玩家 ID = {0}", localPlayerId);

                    // 保存本机玩家 ID
                    GlobalModel.Instance.LocalPLayerId = localPlayerId;

                    // 设置玩家名称
                    PlayerAPI.SetPLayerName(nameInputField.text, success =>
                    {
                        // 进行热更新，在热更新后切换场景
                        StartCoroutine(HotUpdate(conenctServerConfig["hotUpdateServer"],
                            () => { SceneManager.LoadScene("Match Scene"); },
                            () => { SceneManager.LoadScene("Match Scene"); }));
                    }, () =>
                    {
                        // 连接超时，发出信息
                        InfoDialogController.Show("设置名称失败，请尝试重连", 2);
                    });
                }, () =>
                {
                    // 连接超时，发出信息
                    InfoDialogController.Show("获取 ID 失败，请尝试重连", 2);
                });

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
                InfoDialogController.Show("连接超时", 1);

                // 改为没有开始连接状态
                connectStarted = false;
            }
        }
    }
    #endregion

    /// <summary>
    /// 切换到主菜单场景
    /// </summary>
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Open Scene");
    }

    #region 热更新部分
    /// <summary>
    /// 热更新的协程
    /// </summary>
    /// <param name="host"></param>
    /// <param name="successHandler">热更新完成的回调</param>
    /// <param name="failHandler">热更新失败的回调</param>
    /// <returns></returns>
    private IEnumerator HotUpdate(string host, Action successHandler, Action failHandler)
    {
        Debug.Log("热更新协程开始执行");


        ////// 获取服务器包版本列表 //////

        // 准备一个字典来接收包版本信息
        Dictionary<string, DateTime> hotUpdateVersion;

        // 创建一个获取热更细版本的请求
        // 使用 using 来自动解决请求释放的问题
        using (UnityWebRequest webRequest = UnityWebRequest.Get(host + CHECK_UPDATE_URL_INFIX))
        {
            Debug.Log("获取热更新版本，请求 = " + host + CHECK_UPDATE_URL_INFIX);

            // 发出请求并等待响应
            yield return webRequest.SendWebRequest();

            // 如果发生连接错误
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarning("获取热更新版本时发生连接错误: " + webRequest.error);

                // 显示连接失败消息
                InfoDialogController connectInfoDialog = InfoDialogController.Show("没有成功连接到热跟新服务器，跳过热更新步骤");

                // 等待一段时间
                yield return new WaitForSeconds(2);

                // 关闭消息
                connectInfoDialog.Close();

                // 调用热更新失败的回调
                failHandler.Invoke();

                // 结束协程
                yield break;
            }

            // 把获取到的 JSON 字符串转为字典
            hotUpdateVersion = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(webRequest.downloadHandler.text);
        }


        ////// 获取本地的包版本列表 //////

        // 组合出本地的包版本记录文件的完整路径
        string localAssetBundlesVersionFilePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + ASSETBUNDLES_VERSION_JSON_FILE_NAME;

        // 准备一个字典来存储
        Dictionary<string, DateTime> localAssetBundlesVersion = null;

        // 如果本地这个文件存在
        if (File.Exists(localAssetBundlesVersionFilePath))
        {
            // 创建读取流，用 using 自动处理关闭流的问题
            using (StreamReader streamReader = new StreamReader(localAssetBundlesVersionFilePath))
            {
                // 把文件内容全部读取出来
                string fileContents = streamReader.ReadToEnd();

                // 转为字典对象
                localAssetBundlesVersion = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(fileContents);
            }
        }

        // 如果这个字典是 null，这里有两种情况，第一种是本地没有版本文件，第二种是本地的版本文件是空的
        if(localAssetBundlesVersion == null)
        {
            // 创建一个空字典
            localAssetBundlesVersion = new Dictionary<string, DateTime>();
        }


        ////// 找出和服务器版本不一致的包 //////

        // 准备一个字典来存放不一致的包
        Dictionary<string, DateTime> needUpdateBundles = new Dictionary<string, DateTime>();

        // 遍历服务器的包数据
        foreach (KeyValuePair<string, DateTime> bundle in hotUpdateVersion)
        {
            // 如果本地版本记录里没有这个包，或者本地记录的包版本和服务器的包版本不一致
            if (!localAssetBundlesVersion.ContainsKey(bundle.Key) || !Equals(localAssetBundlesVersion[bundle.Key], hotUpdateVersion[bundle.Key]))
            {
                // 把这个包保存下来，以服务器的包信息为准
                needUpdateBundles.Add(bundle.Key, bundle.Value);
            }
        }


        ////// 对不需要更新情况的拦截 //////

        // 如果没有不一致的包
        if (needUpdateBundles.Count == 0)
        {
            // 发出不需要更新消息
            Debug.Log("已经是最新版本不需要更新");

            // 执行成功回调
            successHandler.Invoke();

            // 结束热更新协程
            yield break;
        }


        ////// 下载包 //////

        foreach (KeyValuePair<string, DateTime> bundleInfo in needUpdateBundles)
        {
            Debug.Log("下载包 " + bundleInfo.Key);

            // 记录一个包是否下载成功的标志变量
            bool downloadSuccess = false;

            // 下载资源包文件，这里修改标志变量是 或，因为标志变量一开始是 false，会以这一步操作为准
            yield return StartCoroutine(DownloadAssetBundleFile(host, bundleInfo.Key, false, () => { downloadSuccess |= true; }, () => { downloadSuccess |= false; }));

            // 下载 manifest 文件，这里修改标志变量是 与，资源包文件和 manifest 文件缺一不可，所以这里用 与，只有两个文件都成功才算成功
            yield return StartCoroutine(DownloadAssetBundleFile(host, bundleInfo.Key, true, () => { downloadSuccess &= true; }, () => { downloadSuccess &= false; }));

            // 如果下载成功则把本地的包版本记录更新为下载的包的版本
            if (downloadSuccess)
            {
                if (localAssetBundlesVersion.ContainsKey(bundleInfo.Key))
                {
                    localAssetBundlesVersion[bundleInfo.Key] = bundleInfo.Value;
                }
                else
                {
                    localAssetBundlesVersion.Add(bundleInfo.Key, bundleInfo.Value);
                }
            }
        }


        ////// 把现在的包版本保存到本地去 //////

        // 如果本地没有包版本记录文件则创建
        if (!File.Exists(localAssetBundlesVersionFilePath))
        {
            File.Create(localAssetBundlesVersionFilePath);
        }

        // 用新的信息覆盖这个文件
        using (StreamWriter streamWriter = new StreamWriter(localAssetBundlesVersionFilePath, false))
        {
            // 把版本信息字典转为 JSON 字符串写进去
            streamWriter.Write(JsonConvert.SerializeObject(localAssetBundlesVersion));
        }


        ////// 卸载所有已加载的包，让热更新起效 //////

        // 卸载所有资源包，但保留已经在场上加载的资源
        AssetBundleTools.Instance.UnloadAllAssetBundle(false);


        ////// 更新完成提示 //////

        Debug.Log("更新完毕");

        // 发出更新完毕消息
        InfoDialogController hotUpdateInfoDialog = InfoDialogController.Show("热更新完毕");

        // 等待消息显示
        yield return new WaitForSeconds(1f);

        // 关闭消息
        hotUpdateInfoDialog.Close();

        // 执行成功回调
        successHandler.Invoke();
    }

    /// <summary>
    /// 下载 AssetBundle 文件的协程
    /// </summary>
    /// <param name="host"></param>
    /// <param name="assetBundleName">资源包名称</param>
    /// <param name="isManifest">下载的是不是 manifest 文件，AssetBundle 分为资源包文件本身和配套的 manifest 文件</param>
    /// <param name="successHandler">下载完成的回调</param>
    /// <param name="failHandler">下载失败的回调</param>
    /// <returns></returns>
    private IEnumerator DownloadAssetBundleFile(string host, string assetBundleName, bool isManifest, Action successHandler, Action failHandler)
    {
        // 根据是否是下载 manifest 文件选择请求，拼接资源包名作为参数
        string url = host + (isManifest ? DOWNLOAD_MANIFEST_URL_INFIX : DOWNLOAD_ASSETBUNDLE_URL_INFIX) + assetBundleName;

        //Debug.Log(assetBundleName.Replace('/', Path.DirectorySeparatorChar));

        // 拼接 StreamAssets 和资源包名作为下载到的位置，这里需要先把资源包名的分隔符转为跨平台的，这是因为 Pth.Combine 在遇到与平台不兼容的分隔符时会添加一个附和运行平台的分隔符但不会移除原有的不兼容分隔符
        string savePath = Path.Combine(Application.streamingAssetsPath, (assetBundleName.Replace('/', Path.DirectorySeparatorChar) + (isManifest ? ".manifest" : "")));

        Debug.Log("下载到的位置 = " + savePath);

        // 准备一个 Get 请求的发信器
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 发出请求并等待完成
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("下载 " + assetBundleName + " 失败");

                // 连接错误，发出提示，执行失败回调
                if (failHandler != null)
                {
                    failHandler.Invoke();
                }
            }
            else
            {
                // 连接无误，进行后续处理

                // 创建 StreamAssets 文件夹，只会在没有的时候创建
                Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf(Path.DirectorySeparatorChar)));

                // 写入本地文件
                File.WriteAllBytes(savePath, webRequest.downloadHandler.data);

                // 执行成功回调
                if (successHandler != null)
                {
                    successHandler.Invoke();
                }
            }
        }
    }
    #endregion
}
