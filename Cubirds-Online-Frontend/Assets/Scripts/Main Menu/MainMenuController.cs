using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// 主菜单面板
    /// </summary>
    [SerializeField]
    [Header("主菜单面板")]
    private GameObject mainMenuCanvas;
    /// <summary>
    /// 规则面板
    /// </summary>
    [SerializeField]
    [Header("规则面板")]
    private GameObject ruleCanvas;
    /// <summary>
    /// 设置面板
    /// </summary>
    [SerializeField]
    [Header("设置面板")]
    private GameObject settingCanvas;
    /// <summary>
    /// 自定义面板
    /// </summary>
    [SerializeField]
    [Header("自定义面板")]
    private GameObject customCanvas;

    /// <summary>
    /// 切换到连接场景
    /// </summary>
    public void ToConnecct()
    {
        SceneManager.LoadScene("Connect Scene");
    }

    /// <summary>
    /// 显示规则面板
    /// </summary>
    public void ShowRule()
    {
        mainMenuCanvas.SetActive(false);
        ruleCanvas.SetActive(true);
    }

    /// <summary>
    /// 关闭规则面板
    /// </summary>
    public void CloseRule()
    {
        mainMenuCanvas.SetActive(true);
        ruleCanvas.SetActive(false);
    }

    /// <summary>
    /// 显示设置面板
    /// </summary>
    public void ShowSetting()
    {
        mainMenuCanvas.SetActive(false);
        settingCanvas.SetActive(true);
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    public void CloseSetting()
    {
        mainMenuCanvas.SetActive(true);
        settingCanvas.SetActive(false);
    }

    /// <summary>
    /// 显示设置面板
    /// </summary>
    public void ShowCustom()
    {
        mainMenuCanvas.SetActive(false);
        customCanvas.SetActive(true);
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    public void CloseCustom()
    {
        mainMenuCanvas.SetActive(true);
        customCanvas.SetActive(false);
    }

    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log("Unity 编辑器");
#elif UNITY_WEBGL
        Debug.Log("Unity WebGL");
#elif UNITY_STANDALONE_WIN
        Debug.Log("Unity Windows");
#endif
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
