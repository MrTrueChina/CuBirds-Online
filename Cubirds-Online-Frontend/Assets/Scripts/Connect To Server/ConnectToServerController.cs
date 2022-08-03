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
    /// ��ȡ�����������б�� URL
    /// </summary>
    private const string GET_SERVER_CONFIG_URL = "https://gitcode.net/M_t_C/Get-Config-From-Github-Development-And-Test/-/raw/main/Config/CO-Test-Config.json";
    /// <summary>
    /// ����ȸ��°汾�� URL ��׺
    /// </summary>
    private const string CHECK_UPDATE_URL_INFIX = "/hotUpdate/check";
    /// <summary>
    /// ������Դ���� URL ��׺
    /// </summary>
    private const string DOWNLOAD_ASSETBUNDLE_URL_INFIX = "/hotUpdate/downloadAssetBundle?assetBundleName=";
    /// <summary>
    /// ���� manifest �ļ��� URL ��׺
    /// </summary>
    private const string DOWNLOAD_MANIFEST_URL_INFIX = "/hotUpdate/downloadManifest?assetBundleName=";
    /// <summary>
    /// ���صļ�¼���汾�ŵ��ļ����ļ���
    /// </summary>
    private const string ASSETBUNDLES_VERSION_JSON_FILE_NAME = "AssetBundlesVerisonCheck";

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
    /// ���ӵ��ķ�����������
    /// </summary>
    private Dictionary<string, string> conenctServerConfig;

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
        InfoDialogController infoDialog = InfoDialogController.Show("��ȡ��������Ϣ");

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
        infoDialog.Close();

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

    #region ���ӵ���Ϸ����������
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="config"></param>
    public void StartConnect(Dictionary<string, string> config)
    {
        // û��д�����򷢳���ʾ
        if (nameInputField.text == null || nameInputField.text.Length == 0)
        {
            InfoDialogController.Show("����д����", 1);
            return;
        }

        // ��¼���ӵ��ķ�����
        conenctServerConfig = config;

        // ��ʼ����
        PhotonEngine.Instance.StartConnect(config["gameServer"], config["gameServerName"]);

        // ��¼��ʼ����
        connectStarted = true;

        // ��ʾ��ʾ
        InfoDialogController.Show("��ʼ������" + config["name"], 1);

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
                InfoDialogController.Show("����ʧ��", 1);

                // ��Ϊû�п�ʼ����״̬
                connectStarted = false;

                // ȷ������ʧ�ܺ�Ͳ���Ҫ����������
                return;
            }

            // �������״̬�������ӣ�˵�����ӳɹ�
            if (PhotonEngine.Peer.PeerState == PeerStateValue.Connected)
            {
                //// ��ʾ��ʾ
                //InfoDialogController.Show("���ӳɹ�", 1);

                // ��ȡ������� ID
                PlayerAPI.GetLocalPlayerId(localPlayerId =>
                {
                    Debug.LogFormat("��ȡ������� ID = {0}", localPlayerId);

                    // ���汾����� ID
                    GlobalModel.Instance.LocalPLayerId = localPlayerId;

                    // �����������
                    PlayerAPI.SetPLayerName(nameInputField.text, success =>
                    {
                        // �����ȸ��£����ȸ��º��л�����
                        StartCoroutine(HotUpdate(conenctServerConfig["hotUpdateServer"],
                            () => { SceneManager.LoadScene("Match Scene"); },
                            () => { SceneManager.LoadScene("Match Scene"); }));
                    }, () =>
                    {
                        // ���ӳ�ʱ��������Ϣ
                        InfoDialogController.Show("��������ʧ�ܣ��볢������", 2);
                    });
                }, () =>
                {
                    // ���ӳ�ʱ��������Ϣ
                    InfoDialogController.Show("��ȡ ID ʧ�ܣ��볢������", 2);
                });

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
                InfoDialogController.Show("���ӳ�ʱ", 1);

                // ��Ϊû�п�ʼ����״̬
                connectStarted = false;
            }
        }
    }
    #endregion

    /// <summary>
    /// �л������˵�����
    /// </summary>
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Open Scene");
    }

    #region �ȸ��²���
    /// <summary>
    /// �ȸ��µ�Э��
    /// </summary>
    /// <param name="host"></param>
    /// <param name="successHandler">�ȸ�����ɵĻص�</param>
    /// <param name="failHandler">�ȸ���ʧ�ܵĻص�</param>
    /// <returns></returns>
    private IEnumerator HotUpdate(string host, Action successHandler, Action failHandler)
    {
        Debug.Log("�ȸ���Э�̿�ʼִ��");


        ////// ��ȡ���������汾�б� //////

        // ׼��һ���ֵ������հ��汾��Ϣ
        Dictionary<string, DateTime> hotUpdateVersion;

        // ����һ����ȡ�ȸ�ϸ�汾������
        // ʹ�� using ���Զ���������ͷŵ�����
        using (UnityWebRequest webRequest = UnityWebRequest.Get(host + CHECK_UPDATE_URL_INFIX))
        {
            Debug.Log("��ȡ�ȸ��°汾������ = " + host + CHECK_UPDATE_URL_INFIX);

            // �������󲢵ȴ���Ӧ
            yield return webRequest.SendWebRequest();

            // ����������Ӵ���
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarning("��ȡ�ȸ��°汾ʱ�������Ӵ���: " + webRequest.error);

                // ��ʾ����ʧ����Ϣ
                InfoDialogController connectInfoDialog = InfoDialogController.Show("û�гɹ����ӵ��ȸ��·������������ȸ��²���");

                // �ȴ�һ��ʱ��
                yield return new WaitForSeconds(2);

                // �ر���Ϣ
                connectInfoDialog.Close();

                // �����ȸ���ʧ�ܵĻص�
                failHandler.Invoke();

                // ����Э��
                yield break;
            }

            // �ѻ�ȡ���� JSON �ַ���תΪ�ֵ�
            hotUpdateVersion = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(webRequest.downloadHandler.text);
        }


        ////// ��ȡ���صİ��汾�б� //////

        // ��ϳ����صİ��汾��¼�ļ�������·��
        string localAssetBundlesVersionFilePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + ASSETBUNDLES_VERSION_JSON_FILE_NAME;

        // ׼��һ���ֵ����洢
        Dictionary<string, DateTime> localAssetBundlesVersion = null;

        // �����������ļ�����
        if (File.Exists(localAssetBundlesVersionFilePath))
        {
            // ������ȡ������ using �Զ�����ر���������
            using (StreamReader streamReader = new StreamReader(localAssetBundlesVersionFilePath))
            {
                // ���ļ�����ȫ����ȡ����
                string fileContents = streamReader.ReadToEnd();

                // תΪ�ֵ����
                localAssetBundlesVersion = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(fileContents);
            }
        }

        // �������ֵ��� null�������������������һ���Ǳ���û�а汾�ļ����ڶ����Ǳ��صİ汾�ļ��ǿյ�
        if(localAssetBundlesVersion == null)
        {
            // ����һ�����ֵ�
            localAssetBundlesVersion = new Dictionary<string, DateTime>();
        }


        ////// �ҳ��ͷ������汾��һ�µİ� //////

        // ׼��һ���ֵ�����Ų�һ�µİ�
        Dictionary<string, DateTime> needUpdateBundles = new Dictionary<string, DateTime>();

        // �����������İ�����
        foreach (KeyValuePair<string, DateTime> bundle in hotUpdateVersion)
        {
            // ������ذ汾��¼��û������������߱��ؼ�¼�İ��汾�ͷ������İ��汾��һ��
            if (!localAssetBundlesVersion.ContainsKey(bundle.Key) || !Equals(localAssetBundlesVersion[bundle.Key], hotUpdateVersion[bundle.Key]))
            {
                // ������������������Է������İ���ϢΪ׼
                needUpdateBundles.Add(bundle.Key, bundle.Value);
            }
        }


        ////// �Բ���Ҫ������������� //////

        // ���û�в�һ�µİ�
        if (needUpdateBundles.Count == 0)
        {
            // ��������Ҫ������Ϣ
            Debug.Log("�Ѿ������°汾����Ҫ����");

            // ִ�гɹ��ص�
            successHandler.Invoke();

            // �����ȸ���Э��
            yield break;
        }


        ////// ���ذ� //////

        foreach (KeyValuePair<string, DateTime> bundleInfo in needUpdateBundles)
        {
            Debug.Log("���ذ� " + bundleInfo.Key);

            // ��¼һ�����Ƿ����سɹ��ı�־����
            bool downloadSuccess = false;

            // ������Դ���ļ��������޸ı�־������ ����Ϊ��־����һ��ʼ�� false��������һ������Ϊ׼
            yield return StartCoroutine(DownloadAssetBundleFile(host, bundleInfo.Key, false, () => { downloadSuccess |= true; }, () => { downloadSuccess |= false; }));

            // ���� manifest �ļ��������޸ı�־������ �룬��Դ���ļ��� manifest �ļ�ȱһ���ɣ����������� �룬ֻ�������ļ����ɹ�����ɹ�
            yield return StartCoroutine(DownloadAssetBundleFile(host, bundleInfo.Key, true, () => { downloadSuccess &= true; }, () => { downloadSuccess &= false; }));

            // ������سɹ���ѱ��صİ��汾��¼����Ϊ���صİ��İ汾
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


        ////// �����ڵİ��汾���浽����ȥ //////

        // �������û�а��汾��¼�ļ��򴴽�
        if (!File.Exists(localAssetBundlesVersionFilePath))
        {
            File.Create(localAssetBundlesVersionFilePath);
        }

        // ���µ���Ϣ��������ļ�
        using (StreamWriter streamWriter = new StreamWriter(localAssetBundlesVersionFilePath, false))
        {
            // �Ѱ汾��Ϣ�ֵ�תΪ JSON �ַ���д��ȥ
            streamWriter.Write(JsonConvert.SerializeObject(localAssetBundlesVersion));
        }


        ////// ж�������Ѽ��صİ������ȸ�����Ч //////

        // ж��������Դ�����������Ѿ��ڳ��ϼ��ص���Դ
        AssetBundleTools.Instance.UnloadAllAssetBundle(false);


        ////// ���������ʾ //////

        Debug.Log("�������");

        // �������������Ϣ
        InfoDialogController hotUpdateInfoDialog = InfoDialogController.Show("�ȸ������");

        // �ȴ���Ϣ��ʾ
        yield return new WaitForSeconds(1f);

        // �ر���Ϣ
        hotUpdateInfoDialog.Close();

        // ִ�гɹ��ص�
        successHandler.Invoke();
    }

    /// <summary>
    /// ���� AssetBundle �ļ���Э��
    /// </summary>
    /// <param name="host"></param>
    /// <param name="assetBundleName">��Դ������</param>
    /// <param name="isManifest">���ص��ǲ��� manifest �ļ���AssetBundle ��Ϊ��Դ���ļ���������׵� manifest �ļ�</param>
    /// <param name="successHandler">������ɵĻص�</param>
    /// <param name="failHandler">����ʧ�ܵĻص�</param>
    /// <returns></returns>
    private IEnumerator DownloadAssetBundleFile(string host, string assetBundleName, bool isManifest, Action successHandler, Action failHandler)
    {
        // �����Ƿ������� manifest �ļ�ѡ������ƴ����Դ������Ϊ����
        string url = host + (isManifest ? DOWNLOAD_MANIFEST_URL_INFIX : DOWNLOAD_ASSETBUNDLE_URL_INFIX) + assetBundleName;

        //Debug.Log(assetBundleName.Replace('/', Path.DirectorySeparatorChar));

        // ƴ�� StreamAssets ����Դ������Ϊ���ص���λ�ã�������Ҫ�Ȱ���Դ�����ķָ���תΪ��ƽ̨�ģ�������Ϊ Pth.Combine ��������ƽ̨�����ݵķָ���ʱ�����һ����������ƽ̨�ķָ����������Ƴ�ԭ�еĲ����ݷָ���
        string savePath = Path.Combine(Application.streamingAssetsPath, (assetBundleName.Replace('/', Path.DirectorySeparatorChar) + (isManifest ? ".manifest" : "")));

        Debug.Log("���ص���λ�� = " + savePath);

        // ׼��һ�� Get ����ķ�����
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // �������󲢵ȴ����
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("���� " + assetBundleName + " ʧ��");

                // ���Ӵ��󣬷�����ʾ��ִ��ʧ�ܻص�
                if (failHandler != null)
                {
                    failHandler.Invoke();
                }
            }
            else
            {
                // �������󣬽��к�������

                // ���� StreamAssets �ļ��У�ֻ����û�е�ʱ�򴴽�
                Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf(Path.DirectorySeparatorChar)));

                // д�뱾���ļ�
                File.WriteAllBytes(savePath, webRequest.downloadHandler.data);

                // ִ�гɹ��ص�
                if (successHandler != null)
                {
                    successHandler.Invoke();
                }
            }
        }
    }
    #endregion
}
