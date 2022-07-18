using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetBundleGetter : MonoBehaviour
{
    [Serializable]
    private class ImageToAssetBundleIndex
    {
        /// <summary>
        /// Ҫʹ�� AssetBundle ��Դ��ͼƬ���
        /// </summary>
        [Header("Ҫʹ�� AssetBundle ��Դ��ͼƬ���")]
        public Image image;
        /// <summary>
        /// ��Դ��
        /// </summary>
        [Header("��Դ��")]
        public string assetBundle;
        /// <summary>
        /// ��Դ������ļ���
        /// </summary>
        [Header("��Դ������ļ���")]
        public string resourcesName;
        /// <summary>
        /// �Զ���ͼƬ���ļ���
        /// </summary>
        [Header("�Զ���ͼƬ���ļ���")]
        public string customImageName;
    }

    /// <summary>
    /// ͼƬ�����ʹ�õ���Դ�ļ���ӳ���
    /// </summary>
    [SerializeField]
    [Header("ͼƬ�����ʹ�õ���Դ�ļ�")]
    private List<ImageToAssetBundleIndex> imageIndexList;

    private void Start()
    {
        // ����ӳ���
        imageIndexList.ForEach(imageIndex =>
        {
            // ���û�д���ͼƬ����򲻴�����������������ֻ����յĻ��͸����˵���
            if(imageIndex.image == null)
            {
                return;
            }

            // �� AssetBundle ����ͼƬ���ø�ͼƬ���
            imageIndex.image.sprite = AssetBundleTools.Instance.LoadAsset<Sprite>(imageIndex.assetBundle, imageIndex.resourcesName);
        });
    }
}
