using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class GetServerTester : MonoBehaviour
{
    /// <summary>
    /// 获取服务器配置列表的 URL
    /// </summary>
    private const string GET_SERVER_CONFIG_URL = "https://gitcode.net/M_t_C/Get-Config-From-Github-Development-And-Test/-/raw/main/Config/CO-Test-Config.json";

    /// <summary>
    /// 服务器配置列表
    /// </summary>
    public static List<Dictionary<string, string>> ServerConfigs { get; set; } = new List<Dictionary<string, string>>();

    private void Start()
    {
        StartCoroutine(GetServerConfig(GET_SERVER_CONFIG_URL));
    }

    /// <summary>
    /// 获取服务器配置列表的协程
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private IEnumerator GetServerConfig(string uri)
    {
        Debug.Log("开始获取服务器信息");

        InfoDialogController.Instance.Show("获取服务器信息");

        // 准备一个字典树组来接收配置信息
        List<Dictionary<string, string>> serverConfigList;

        // 使用 using 来自动解决请求释放的问题
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
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

            // 把获取到的 JSON 字符串转为字典
            serverConfigList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(webRequest.downloadHandler.text);
        }

        Debug.Log("获取到服务器配置列表：");
        serverConfigList.ForEach(config => {
            Debug.Log(config["name"] + " | " + config["gameServer"] + " | " + config["hotUpdateServer"]);
        });

        InfoDialogController.Instance.Close();

        // 保存到服务器配置列表里
        ServerConfigs = serverConfigList;
    }
}
