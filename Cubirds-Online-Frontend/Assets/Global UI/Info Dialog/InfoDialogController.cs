using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins;
using UnityEngine.UI;

/// <summary>
/// ��ʾ��Ϣ�ĶԻ���Ŀ�����
/// </summary>
public class InfoDialogController : MonoBehaviour
{
    #region Ԥ�Ʋ���
    public static GameObject Prefab
    {
        get
        {
            if (prefab != null)
            {
                return prefab;
            }

            lock (typeof(InfoDialogController))
            {
                if (prefab == null)
                {
                    // �� Resources ����Ԥ��
                    prefab = Resources.Load<GameObject>("Global UI/Info Dialog");
                }

                return prefab;
            }
        }
    }
    private static GameObject prefab;
    #endregion

    /// <summary>
    /// �Ի���Ԥ�Ƶĸ�����
    /// </summary>
    [SerializeField]
    [Header("�Ի���Ԥ�Ƶĸ�����")]
    private GameObject rootGameObject;

    /// <summary>
    /// �Ի���� Transfrom ���
    /// </summary>
    [SerializeField]
    [Header("�Ի���� Transfrom ���")]
    private RectTransform rectTransform;

    /// <summary>
    /// �ı����
    /// </summary>
    [SerializeField]
    [Header("�ı����")]
    private Text text;

    /// <summary>
    /// ��ʱ��ʾ��Ϣ
    /// </summary>
    /// <param name="info"></param>
    /// <param name="duration">��ʾʱ��</param>
    public static void Show(string info, float duration)
    {
        // ��ʾ��Ϣ
        InfoDialogController instance = Show(info);

        // ��ʱ�ر�
        instance.StartCoroutine(DelayClose(instance, duration));
    }

    /// <summary>
    /// ��ʾ��Ϣ
    /// </summary>
    /// <param name="info"></param>
    /// <returns>��ʾ��Ϣ�ĶԻ����ʵ���Ŀ��ƽű�</returns>
    public static InfoDialogController Show(string info)
    {
        // ʵ��������ȡ���ƽű�
        InfoDialogController instance = Instantiate(Prefab).GetComponentInChildren<InfoDialogController>();

        // ��ʾ��Ϣ
        instance.ShowInfo(info);

        // ���ؿ��ƽű�
        return instance;
    }

    /// <summary>
    /// ��ʱ�ر�ָ�����
    /// </summary>
    /// <param name="dialog">Ҫ�رյ����</param>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private static IEnumerator DelayClose(InfoDialogController dialog, float delayTime)
    {
        // �ȴ�ʱ��
        yield return new WaitForSeconds(delayTime);

        // �ر����
        dialog.Close();
    }

    /// <summary>
    /// ��ʾ��Ϣ�������Զ��رգ�ֻ�е��� <see cref="Close"/> �Ż�ر�
    /// </summary>
    /// <param name="info"></param>
    private void ShowInfo(string info)
    {
        // ������ʾ���ı�
        text.text = info;

        // ���Ȱ����ŵ��� 0�����Ӿ�����������
        rectTransform.localScale = new Vector3(1, 0, 1);

        // ����ʵ��
        rectTransform.gameObject.SetActive(true);

        // ������ʾ����
        rectTransform.DOScaleY(1, 0.1f);
    }

    /// <summary>
    /// �ر����
    /// </summary>
    public void Close()
    {
        // ����ر�
        rectTransform.DOScaleY(0, 0.1f)
            // ��ɺ�Ӹ����忪ʼ���������Ի���
            .onComplete = () => Destroy(rootGameObject);
    }
}
