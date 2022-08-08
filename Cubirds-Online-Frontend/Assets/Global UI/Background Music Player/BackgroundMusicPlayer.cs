using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.CustomPlugins;

/// <summary>
/// 控制 bgm 的组件
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
    #region 单例部分
    /// <summary>
    /// 实例
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
                    // 加载预制
                    GameObject prefab = Resources.Load<GameObject>("Global UI/BGM Player");

                    // 实例化物体
                    GameObject instanceObject = Instantiate(prefab);
                    
                    // 获取组件并记录
                    instance = instanceObject.GetComponent<BackgroundMusicPlayer>();

                    // 对象设为不随场景加载销毁
                    DontDestroyOnLoad(instanceObject);
                }

                return instance;
            }
        }
    }
    private static BackgroundMusicPlayer instance;
    #endregion

    /// <summary>
    /// 通常 BGM 播放器
    /// </summary>
    [SerializeField]
    [Header("通常 BGM 播放器")]
    private AudioSource normalAudioSource;
    /// <summary>
    /// 优势 BGM 播放器
    /// </summary>
    [SerializeField]
    [Header("优势 BGM 播放器")]
    private AudioSource advantageAudioSource;
    /// <summary>
    /// 劣势 BGM 播放器
    /// </summary>
    [SerializeField]
    [Header("劣势 BGM 播放器")]
    private AudioSource inferiorityAudioSource;
    /// <summary>
    /// 激烈 BGM 播放器
    /// </summary>
    [SerializeField]
    [Header("激烈 BGM 播放器")]
    private AudioSource fierceAudioSource;

    /// <summary>
    /// BGM 资源包名
    /// </summary>
    [SerializeField]
    [Header("BGM 资源包名")]
    private string bgmAssetBundleName;

    /// <summary>
    /// 通常 BGM 资源包文件名
    /// </summary>
    [SerializeField]
    [Header("通常 BGM 资源包文件名")]
    private string normalBgmAssetBundleName;
    /// <summary>
    /// 优势 BGM 资源包文件名
    /// </summary>
    [SerializeField]
    [Header("优势 BGM 资源包文件名")]
    private string advantageBgmAssetBundleName;
    /// <summary>
    /// 劣势 BGM 资源包文件名
    /// </summary>
    [SerializeField]
    [Header("劣势 BGM 资源包文件名")]
    private string inferiorityBgmAssetBundleName;
    /// <summary>
    /// 激烈 BGM 资源包文件名
    /// </summary>
    [SerializeField]
    [Header("激烈 BGM 资源包文件名")]
    private string fierceBgmAssetBundleName;

    /// <summary>
    /// 播放通常 BGM
    /// </summary>
    public void PlayNormalBGM()
    {
        StartAudioSourcePlay(normalAudioSource, normalBgmAssetBundleName);
    }

    /// <summary>
    /// 播放优势 BGM
    /// </summary>
    public void PlayAdvantageBGM()
    {
        StartAudioSourcePlay(advantageAudioSource, advantageBgmAssetBundleName);
    }

    /// <summary>
    /// 播放劣势 BGM
    /// </summary>
    public void PlayInferiorityBGM()
    {
        StartAudioSourcePlay(inferiorityAudioSource, inferiorityBgmAssetBundleName);
    }

    /// <summary>
    /// 播放激战 BGM
    /// </summary>
    public void PlayFierceBGM()
    {
        StartAudioSourcePlay(fierceAudioSource, fierceBgmAssetBundleName);
    }

    /// <summary>
    /// 启动指定音源的播放
    /// </summary>
    /// <param name="playAudioSource"></param>
    /// <param name="audioClipAssetName">音频在 AssetBundle 里的文件名</param>
    private void StartAudioSourcePlay(AudioSource playAudioSource, string audioClipAssetName)
    {
        // 把音源合并成列表，便于后续遍历
        List<AudioSource> audioSources = new List<AudioSource>() { normalAudioSource, advantageAudioSource, inferiorityAudioSource, fierceAudioSource };

        // 遍历音源列表
        audioSources.ForEach(audioSource =>
        {
            if (audioSource == playAudioSource)
            {
                // 如果遍历到的音源是要播放的音源

                // 如果这个音源没在播放
                if (!audioSource.isPlaying)
                {
                    // 从 AssetBundle 里加载音频设置给这个音源
                    audioSource.clip = AssetBundleTools.Instance.LoadAsset<AudioClip>(bgmAssetBundleName, audioClipAssetName);

                    // 渐变把音量拉满
                    audioSource.DOFade(1, 2);

                    // 立即开始播放
                    audioSource.Play();
                }
            }
            else
            {
                // 如果遍历到的音源不是要播放的音源

                // 如果这个音源正在播放
                if (audioSource.isPlaying)
                {
                    // 渐变把音量拉到 0
                    audioSource.DOFade(0, 2)
                    // 渐变完成后关闭播放
                    .onComplete = () => audioSource.Stop();
                }
            }
        });
    }
}
