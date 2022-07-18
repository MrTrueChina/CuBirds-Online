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
        /// 要使用 AssetBundle 资源的图片组件
        /// </summary>
        [Header("要使用 AssetBundle 资源的图片组件")]
        public Image image;
        /// <summary>
        /// 资源包
        /// </summary>
        [Header("资源包")]
        public string assetBundle;
        /// <summary>
        /// 资源包里的文件名
        /// </summary>
        [Header("资源包里的文件名")]
        public string resourcesName;
        /// <summary>
        /// 自定义图片的文件名
        /// </summary>
        [Header("自定义图片的文件名")]
        public string customImageName;
    }

    /// <summary>
    /// 图片组件和使用的资源文件的映射表
    /// </summary>
    [SerializeField]
    [Header("图片组件和使用的资源文件")]
    private List<ImageToAssetBundleIndex> imageIndexList;

    private void Start()
    {
        // 遍历映射表
        imageIndexList.ForEach(imageIndex =>
        {
            // 如果没有存入图片组件则不处理，假设可能有设置手滑留空的话就给过滤掉。
            if(imageIndex.image == null)
            {
                return;
            }

            // 从 AssetBundle 加载图片设置给图片组件
            imageIndex.image.sprite = AssetBundleTools.Instance.LoadAsset<Sprite>(imageIndex.assetBundle, imageIndex.resourcesName);
        });
    }
}
