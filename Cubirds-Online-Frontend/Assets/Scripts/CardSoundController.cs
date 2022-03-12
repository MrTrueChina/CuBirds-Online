using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡片音效播放控制器
/// </summary>
public class CardSoundController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static CardSoundController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(CardSoundController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindWithTag("CardSoundController").GetComponent<CardSoundController>();
                }

                return instance;
            }
        }
    }
    private static CardSoundController instance;

    /// <summary>
    /// 音频播放器
    /// </summary>
    [SerializeField]
    [Header("音频播放器")]
    private AudioSource audioSource;

    /// <summary>
    /// 放下卡牌音频
    /// </summary>
    [SerializeField]
    [Header("放下卡牌音频")]
    private AudioClip putCardClip;
    /// <summary>
    /// 卡牌摩擦音频
    /// </summary>
    [SerializeField]
    [Header("卡牌摩擦音频")]
    private AudioClip cardFrictionClip;

    /// <summary>
    /// 音量随机范围（百分比）
    /// </summary>
    [SerializeField]
    [Header("音量随机范围（百分比）")]
    private float volumeRandomRange;
    /// <summary>
    /// 音调随机范围
    /// </summary>
    [SerializeField]
    [Header("音调随机范围")]
    private Vector2 pitchRandomRange;

    /// <summary>
    /// 播放放下卡牌音效
    /// </summary>
    /// <param name="volume">音量</param>
    public void PlayPutCardSound(float volume = 0.2f)
    {
        // 设置音频为放下卡牌音频
        audioSource.clip = putCardClip;

        // 播放音频
        Play(volume);
    }

    /// <summary>
    /// 播放卡牌摩擦音效
    /// </summary>
    /// <param name="volume">音量</param>
    public void PlayCardFrictionSound(float volume = 0.03f)
    {
        // 设置音频为卡牌摩擦音频
        audioSource.clip = cardFrictionClip;

        // 播放音频
        Play(volume);
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="volume">音量</param>
    private void Play(float volume)
    {


        // 设置音量
        audioSource.volume = volume * (1 + (Random.Range(-volumeRandomRange, volumeRandomRange) / 100));

        // 设置随机的音调
        audioSource.pitch = Random.Range(pitchRandomRange.x, pitchRandomRange.y);

        // 播放
        audioSource.Play();
    }
}
