using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        // EDITOR ��Ҫ������ WEBGL����Ϊû�� EDITOR ��������ǰ��շ������ô����
#elif UNITY_WEBGL
        // ����� WebGL �ˣ�WebGL ��û���Զ��幦�ܣ����Ҳ���Ҫ�ȸ��¹��ܣ�ֱ��ʹ��Ĭ��ͼƬ
        return;
#endif
        // ����ӳ���
        imageIndexList.ForEach(imageIndex =>
        {
            // ���û�д���ͼƬ����򲻴�����������������ֻ����յĻ��͸����˵���
            if(imageIndex.image == null)
            {
                return;
            }

            // ���д���Զ���ͼƬ���ƣ���������ͼƬȷʵ����
            if(imageIndex.customImageName != null && imageIndex.customImageName != "" && File.Exists(CustomController.CUSTOM_IMAGES_PATH + imageIndex.customImageName))
            {
                // ��������ļ���Ϊ��ʾ��ͼ��
                imageIndex.image.sprite = CustomController.LoadSpriteFromImageFilePath(CustomController.CUSTOM_IMAGES_PATH + imageIndex.customImageName);

                // ʹ�����Զ���ͼƬ��Ͳ���Ҫ����������
                return;
            }

            // �� AssetBundle ����ͼƬ���ø�ͼƬ���
            imageIndex.image.sprite = AssetBundleTools.Instance.LoadAsset<Sprite>(imageIndex.assetBundle, imageIndex.resourcesName);
        });
    }
}
