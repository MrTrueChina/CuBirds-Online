using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingController : MonoBehaviour
{
    /// <summary>
    /// 音量滑条
    /// </summary>
    [SerializeField]
    [Header("音量滑条")]
    private Slider volumeSlider;
    /// <summary>
    /// 音频混合器
    /// </summary>
    [SerializeField]
    [Header("音频混合器")]
    private AudioMixer audioMixer;

    private void Start()
    {
        // 初始化总音量滑条
        InitMainVolumeSetting();
    }

    #region 总音量设置部分
    /// <summary>
    /// 初始化总音量滑条
    /// </summary>
    private void InitMainVolumeSetting()
    {
        // 读取记录的设置内容，没有设置过则默认为 1
        float savedVolume = PlayerPrefs.GetFloat("Setting/MainVolume", 1);

        // 订阅滑条的数值变化事件
        volumeSlider.onValueChanged.AddListener(OnMainVolumeSliderValueChange);

        // 设置滑条范围
        volumeSlider.maxValue = 1;
        volumeSlider.minValue = float.Epsilon; // 设为 Epsilon 是因为后面使用 Log10 运算，0 的底是 1，这会导致滑条拉到 0 音量跳跃性的回到了更大的位置

        // 设置为保存的音量，这里先读取后保存是因为调整滑条范围可能导致滑条值变化，进而触发音量设置功能
        volumeSlider.value = savedVolume;
    }
    /// <summary>
    /// 总音量滑条数值变化时调用的方法
    /// </summary>
    /// <param name="volume"></param>
    private void OnMainVolumeSliderValueChange(float volume)
    {
        // Log10：取以 10 为底的对数，即 10 的 n 次方等于参数，计算返回那个 n
        // 任何底的 0 次方都是 1，所以当滑条为 1 时 n = 0，音量就会设置为 0*20=0，0 在音频混合器里是不调整音量。
        // 当滑条接近 0 时，n 接近 -45，此时的音量设置接近 -45*20=-900，在音频混合器里 -80 即为静音，所以这里就是完全的静音
        //
        // 由于音频混合器的减小音量效果很强，通常音量到 -40 的时候很多声音已经听不见了，所以需要使用 Log10 这种有很大弧度的函数来进行调节，让滑条数值更大的那部分对音量的控制更敏感，数值更小的那部分则要不敏感。这样才能让滑条整体感觉是“均匀”的
        //
        // 任何数的 0 次方是 1，所以一旦滑条拉到 0，n 反而会变成 1，将会设置音量为 20，这是非常不合理的，所以滑条最小值需要限制为 Epsilon
        audioMixer.SetFloat("MainVolume", Mathf.Log10(volume) * 20);

        // 把总音量的变化记录到硬盘
        PlayerPrefs.SetFloat("Setting/MainVolume", volume);
        PlayerPrefs.Save();
    }
    #endregion
}
