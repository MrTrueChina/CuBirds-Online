using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// ���˵����
    /// </summary>
    [SerializeField]
    [Header("���˵����")]
    private GameObject mainMenuCanvas;
    /// <summary>
    /// �������
    /// </summary>
    [SerializeField]
    [Header("�������")]
    private GameObject ruleCanvas;
    /// <summary>
    /// �������
    /// </summary>
    [SerializeField]
    [Header("�������")]
    private GameObject settingCanvas;

    /// <summary>
    /// �л������ӳ���
    /// </summary>
    public void ToConnecct()
    {
        SceneManager.LoadScene("Connect Scene");
    }

    /// <summary>
    /// ��ʾ�������
    /// </summary>
    public void ShowRule()
    {
        mainMenuCanvas.SetActive(false);
        ruleCanvas.SetActive(true);
    }

    /// <summary>
    /// �رչ������
    /// </summary>
    public void CloseRule()
    {
        mainMenuCanvas.SetActive(true);
        ruleCanvas.SetActive(false);
    }

    /// <summary>
    /// ��ʾ�������
    /// </summary>
    public void ShowSetting()
    {
        mainMenuCanvas.SetActive(false);
        settingCanvas.SetActive(true);
    }

    /// <summary>
    /// �ر��������
    /// </summary>
    public void CloseSetting()
    {
        mainMenuCanvas.SetActive(true);
        settingCanvas.SetActive(false);
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
