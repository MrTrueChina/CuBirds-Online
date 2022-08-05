using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingController : MonoBehaviour
{
    /// <summary>
    /// �����������õ�����
    /// </summary>
    [System.Serializable]
    private class AudioSettingConfig
    {
        /// <summary>
        /// ��������
        /// </summary>
        [Header("��������")]
        public Slider volumeSlider;
        /// <summary>
        /// ����Ƶ������еĲ�����
        /// </summary>
        [Header("����Ƶ������еĲ�����")]
        public string audioMixerParamName;
        /// <summary>
        /// �� PlayerPrefs �е� Key
        /// </summary>
        [Header("�� PlayerPrefs �е� Key")]
        public string playerPrefsKey;
    }

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField]
    [Header("��������")]
    private List<AudioSettingConfig> volumeSettings;

    /// <summary>
    /// ��Ƶ�����
    /// </summary>
    [SerializeField]
    [Header("��Ƶ�����")]
    private AudioMixer audioMixer;

    private void Start()
    {
        // ��ʼ����������
        InitVolumeSetting();
    }

    /// <summary>
    /// ��ʼ����������
    /// </summary>
    private void InitVolumeSetting()
    {
        volumeSettings.ForEach(setting =>
        {
            // ��ȡ��¼���������ݣ�û�����ù���Ĭ��Ϊ 0.8������ҽ�һ���Ӵ�Ŀռ�
            float savedVolume = PlayerPrefs.GetFloat(setting.playerPrefsKey, 0.8f);

            // ���Ļ�������ֵ�仯�¼�
            setting.volumeSlider.onValueChanged.AddListener(volume => OnVolumeSliderValueChange(setting, volume));

            // ���û�����Χ
            setting.volumeSlider.maxValue = 1;
            setting.volumeSlider.minValue = float.Epsilon; // ��Ϊ Epsilon ����Ϊ����ʹ�� Log10 ���㣬0 �ĵ��� 1����ᵼ�»������� 0 ������Ծ�ԵĻص��˸����λ��

            // ����Ϊ����������������ȶ�ȡ�󱣴�����Ϊ����������Χ���ܵ��»���ֵ�仯�����������������ù���
            setting.volumeSlider.value = savedVolume;
        });
    }

    /// <summary>
    /// ����������ֵ�仯ʱ���õķ���
    /// </summary>
    /// <param name="config"></param>
    /// <param name="volume"></param>
    private void OnVolumeSliderValueChange(AudioSettingConfig config, float volume)
    {
        // Log10��ȡ�� 10 Ϊ�׵Ķ������� 10 �� n �η����ڲ��������㷵���Ǹ� n
        // �κε׵� 0 �η����� 1�����Ե�����Ϊ 1 ʱ n = 0�������ͻ�����Ϊ 0*20=0��0 ����Ƶ��������ǲ�����������
        // �������ӽ� 0 ʱ��n �ӽ� -45����ʱ���������ýӽ� -45*20=-900������Ƶ������� -80 ��Ϊ�������������������ȫ�ľ���
        //
        // ������Ƶ������ļ�С����Ч����ǿ��ͨ�������� -40 ��ʱ��ܶ������Ѿ��������ˣ�������Ҫʹ�� Log10 �����кܴ󻡶ȵĺ��������е��ڣ��û�����ֵ������ǲ��ֶ������Ŀ��Ƹ����У���ֵ��С���ǲ�����Ҫ�����С����������û�������о��ǡ����ȡ���
        //
        // �κ����� 0 �η��� 1������һ���������� 0��n �������� 1��������������Ϊ 20�����Ƿǳ�������ģ����Ի�����Сֵ��Ҫ����Ϊ Epsilon
        audioMixer.SetFloat(config.audioMixerParamName, Mathf.Log10(volume) * 20);

        // ���������ı仯��¼��Ӳ��
        PlayerPrefs.SetFloat(config.playerPrefsKey, volume);
        PlayerPrefs.Save();
    }
}
