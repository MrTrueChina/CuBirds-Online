using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins;

/// <summary>
/// ��ʾ��Ϣ�ĶԻ���Ŀ�����
/// </summary>
public class InfoDialogController : MonoBehaviour
{
    #region ��������
    /// <summary>
    /// ʵ��
    /// </summary>
    public static InfoDialogController Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (typeof(InfoDialogController))
            {
                if(instance == null)
                {
                    // �� Resources ����Ԥ��
                    GameObject prefab = Resources.Load<GameObject>("Info Dialog");

                    // ʵ�������������
                    instance = Instantiate(prefab).GetComponent<InfoDialogController>();

                    // ����ʵ����Ĭ�ϲ���ʾ
                    instance.gameObject.SetActive(false);
                }

                return instance;
            }
        }
    }
    private static InfoDialogController instance;
    #endregion

    [SerializeField]
    private RectTransform rectTransform;

    /// <summary>
    /// ��ʾ��Ϣ����ָ��ʱ���ر�
    /// </summary>
    /// <param name="info"></param>
    /// <param name="displayTime">��ʾ��Ϣ��ʱ�䣨�룩</param>
    public void Show(string info, float displayTime)
    {
        // ��ʾ��Ϣ
        Show(info);

        // ��Э����ʱ�ر�
        StartCoroutine(DelayClose(displayTime));
    }

    /// <summary>
    /// ��ʱ�ر����
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator DelayClose(float delayTime)
    {
        // �ȴ�ʱ��
        yield return new WaitForSeconds(delayTime);

        // �ر����
        Close();
    }

    /// <summary>
    /// ��ʾ��Ϣ�������Զ��رգ�ֻ�е��� <see cref="Close"/> �Ż�ر�
    /// </summary>
    /// <param name="info"></param>
    public void Show(string info)
    {
        // ����ʵ��
        gameObject.SetActive(true);

        // ���Ȱ����ŵ��� 0�����Ӿ�����������
        rectTransform.localScale = new Vector3(1, 0, 1);

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
            // ��ɺ����ʵ��
            .onComplete = () => { gameObject.SetActive(false); };
    }
}
