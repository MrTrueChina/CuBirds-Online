using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����������������
/// </summary>
public class CushionDisplayer : MonoBehaviour
{
    /// <summary>
    /// ����ͼƬ
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ")]
    private GameObject cushion;

    private void Start()
    {
        // ��ȡ���ã��������ÿ�������ͼƬ�����Ƿ񼤻�
        cushion.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("UseCushion", Convert.ToInt32(true))));
    }
}
