using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.CustomPlugins;

/// <summary>
/// ���� bgm �����
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
    #region ��������
    /// <summary>
    /// ʵ��
    /// </summary>
    public static BackgroundMusicPlayer Instance {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (typeof(BackgroundMusicPlayer))
            {
                if (instance == null)
                {
                    // ����Ԥ��
                    GameObject prefab = Resources.Load<GameObject>("Global UI/BGM Player");

                    // ʵ��������
                    GameObject instanceObject = Instantiate(prefab);
                    
                    // ��ȡ�������¼
                    instance = instanceObject.GetComponent<BackgroundMusicPlayer>();

                    // ������Ϊ���泡����������
                    DontDestroyOnLoad(instanceObject);
                }

                return instance;
            }
        }
    }
    private static BackgroundMusicPlayer instance;
    #endregion

    /// <summary>
    /// ͨ�� BGM ������
    /// </summary>
    [SerializeField]
    [Header("ͨ�� BGM ������")]
    private AudioSource normalAudioSource;
    /// <summary>
    /// ���� BGM ������
    /// </summary>
    [SerializeField]
    [Header("���� BGM ������")]
    private AudioSource advantageAudioSource;
    /// <summary>
    /// ���� BGM ������
    /// </summary>
    [SerializeField]
    [Header("���� BGM ������")]
    private AudioSource inferiorityAudioSource;
    /// <summary>
    /// ���� BGM ������
    /// </summary>
    [SerializeField]
    [Header("���� BGM ������")]
    private AudioSource fierceAudioSource;

    /// <summary>
    /// BGM ��Դ����
    /// </summary>
    [SerializeField]
    [Header("BGM ��Դ����")]
    private string bgmAssetBundleName;

    /// <summary>
    /// ͨ�� BGM ��Դ���ļ���
    /// </summary>
    [SerializeField]
    [Header("ͨ�� BGM ��Դ���ļ���")]
    private string normalBgmAssetBundleName;
    /// <summary>
    /// ���� BGM ��Դ���ļ���
    /// </summary>
    [SerializeField]
    [Header("���� BGM ��Դ���ļ���")]
    private string advantageBgmAssetBundleName;
    /// <summary>
    /// ���� BGM ��Դ���ļ���
    /// </summary>
    [SerializeField]
    [Header("���� BGM ��Դ���ļ���")]
    private string inferiorityBgmAssetBundleName;
    /// <summary>
    /// ���� BGM ��Դ���ļ���
    /// </summary>
    [SerializeField]
    [Header("���� BGM ��Դ���ļ���")]
    private string fierceBgmAssetBundleName;

    /// <summary>
    /// ����ͨ�� BGM
    /// </summary>
    public void PlayNormalBGM()
    {
        StartAudioSourcePlay(normalAudioSource, normalBgmAssetBundleName);
    }

    /// <summary>
    /// �������� BGM
    /// </summary>
    public void PlayAdvantageBGM()
    {
        StartAudioSourcePlay(advantageAudioSource, advantageBgmAssetBundleName);
    }

    /// <summary>
    /// �������� BGM
    /// </summary>
    public void PlayInferiorityBGM()
    {
        StartAudioSourcePlay(inferiorityAudioSource, inferiorityBgmAssetBundleName);
    }

    /// <summary>
    /// ���ż�ս BGM
    /// </summary>
    public void PlayFierceBGM()
    {
        StartAudioSourcePlay(fierceAudioSource, fierceBgmAssetBundleName);
    }

    /// <summary>
    /// ����ָ����Դ�Ĳ���
    /// </summary>
    /// <param name="playAudioSource"></param>
    /// <param name="audioClipAssetName">��Ƶ�� AssetBundle ����ļ���</param>
    private void StartAudioSourcePlay(AudioSource playAudioSource, string audioClipAssetName)
    {
        // ����Դ�ϲ����б����ں�������
        List<AudioSource> audioSources = new List<AudioSource>() { normalAudioSource, advantageAudioSource, inferiorityAudioSource, fierceAudioSource };

        // ������Դ�б�
        audioSources.ForEach(audioSource =>
        {
            if (audioSource == playAudioSource)
            {
                // �������������Դ��Ҫ���ŵ���Դ

                // ��������Դû�ڲ���
                if (!audioSource.isPlaying)
                {
                    // �� AssetBundle �������Ƶ���ø������Դ
                    audioSource.clip = AssetBundleTools.Instance.LoadAsset<AudioClip>(bgmAssetBundleName, audioClipAssetName);

                    // �������������
                    audioSource.DOFade(1, 2);

                    // ������ʼ����
                    audioSource.Play();
                }
            }
            else
            {
                // �������������Դ����Ҫ���ŵ���Դ

                // ��������Դ���ڲ���
                if (audioSource.isPlaying)
                {
                    // ������������� 0
                    audioSource.DOFade(0, 2)
                    // ������ɺ�رղ���
                    .onComplete = () => audioSource.Stop();
                }
            }
        });
    }
}
