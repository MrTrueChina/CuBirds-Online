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
    /// ����ͨ�� BGM
    /// </summary>
    public void PlayNormalBGM()
    {
        StartAudioSourcePlay(normalAudioSource);
    }

    /// <summary>
    /// �������� BGM
    /// </summary>
    public void PlayAdvantageBGM()
    {
        StartAudioSourcePlay(advantageAudioSource);
    }

    /// <summary>
    /// �������� BGM
    /// </summary>
    public void PlayInferiorityBGM()
    {
        StartAudioSourcePlay(inferiorityAudioSource);
    }

    /// <summary>
    /// ���ż�ս BGM
    /// </summary>
    public void PlayFierceBGM()
    {
        StartAudioSourcePlay(fierceAudioSource);
    }

    /// <summary>
    /// ����ָ����Դ�Ĳ���
    /// </summary>
    /// <param name="playAudioSource"></param>
    private void StartAudioSourcePlay(AudioSource playAudioSource)
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
