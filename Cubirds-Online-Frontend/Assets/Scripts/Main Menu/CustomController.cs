using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.UI;
using System;

public class CustomController : MonoBehaviour
{
    /// <summary>
    /// 本地存放自定义图片的路径
    /// </summary>
    public static readonly string CUSTOM_IMAGES_PATH = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Custom" + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar;

    /// <summary>
    /// 桌布图片组件
    /// </summary>
    [SerializeField]
    [Header("桌布图片组件")]
    private Image tableClothImage;
    /// <summary>
    /// 桌布图片的本地文件名
    /// </summary>
    [SerializeField]
    [Header("桌布图片的本地文件名")]
    private string tableClothImageName;
    /// <summary>
    /// 桌布图片的 AssetBundle 包名
    /// </summary>
    [SerializeField]
    [Header("桌布图片的 AssetBundle 包名")]
    private string tableClothAssetBundleName;
    /// <summary>
    /// 桌布图片的 AssetBundle 资源名
    /// </summary>
    [SerializeField]
    [Header("桌布图片的 AssetBundle 资源名")]
    private string tableClothResourcesName;
    /// <summary>
    /// 桌垫图片组件
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("桌垫图片组件")]
    private Image cushionImage;
    /// <summary>
    /// 桌垫图片的本地文件名
    /// </summary>
    [SerializeField]
    [Header("桌垫图片的本地文件名")]
    private string cushionImageName;
    /// <summary>
    /// 桌垫图片的 AssetBundle 包名
    /// </summary>
    [SerializeField]
    [Header("桌垫图片的 AssetBundle 包名")]
    private string cushionAssetBundleName;
    /// <summary>
    /// 桌垫图片的 AssetBundle 资源名
    /// </summary>
    [SerializeField]
    [Header("桌垫图片的 AssetBundle 资源名")]
    private string cushionResourcesName;
    /// <summary>
    /// 卡背图片组件
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("卡背图片组件")]
    private List<Image> cardBackImages;
    /// <summary>
    /// 卡背图片的本地文件名
    /// </summary>
    [SerializeField]
    [Header("卡背图片的本地文件名")]
    private string cardBackImageName;
    /// <summary>
    /// 卡背图片的 AssetBundle 包名
    /// </summary>
    [SerializeField]
    [Header("卡背图片的 AssetBundle 包名")]
    private string cardBackAssetBundleName;
    /// <summary>
    /// 卡背图片的 AssetBundle 资源名
    /// </summary>
    [SerializeField]
    [Header("卡背图片的 AssetBundle 资源名")]
    private string cardBackResourcesName;

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        // WebGL 没有自定义功能，直接 return
        return;
#endif

        // 初始化卡背，桌布和桌垫用通用的自定义+热更新显示就可以了
        InitCardBack();

        // 读取是否使用桌垫的设置，控制桌面显隐
        cushionImage.gameObject.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("UseCushion", Convert.ToInt32(true))));
    }

    /// <summary>
    /// 初始化卡背
    /// </summary>
    private void InitCardBack()
    {
        Sprite cardBackSprite = null;

        if (File.Exists(CUSTOM_IMAGES_PATH + cardBackImageName))
        {
            // 卡背的自定义图片存在，使用自定义图片作为卡背
            cardBackSprite = LoadSpriteFromImageFilePath(CUSTOM_IMAGES_PATH + cardBackImageName);
        }
        else
        {
            // 卡背的自定义图片不存在，加载 AssetBundle 中的图片作为卡背
            cardBackSprite = AssetBundleTools.Instance.LoadAsset<Sprite>(cardBackAssetBundleName, cardBackResourcesName);
        }

        cardBackImages.ForEach(image => image.sprite = cardBackSprite);
    }

    /// <summary>
    /// 选择自定义桌布
    /// </summary>
    public void SelectCustomTabelCloth()
    {
        SelectCustomImage(new List<Image>() { tableClothImage }, tableClothImageName);
    }

    /// <summary>
    /// 选择自定义桌垫
    /// </summary>
    public void SelectCustomCushion()
    {
        // 激活桌垫物体
        cushionImage.gameObject.SetActive(true);

        SelectCustomImage(new List<Image>() { cushionImage }, cushionImageName);

        // 保存使用桌垫的设置
        PlayerPrefs.SetInt("UseCushion", Convert.ToInt32(true));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 选择自定义卡背
    /// </summary>
    public void SelectCustomCardBack()
    {
        SelectCustomImage(cardBackImages, cardBackImageName);
    }

    /// <summary>
    /// 选择自定义图片
    /// </summary>
    /// <param name="imageComponents">用于预览效果的图片组件</param>
    /// <param name="localImageName">自定义图片在本地存储的文件名</param>
    private void SelectCustomImage(List<Image> imageComponents, string localImageName)
    {
        // Runtime File Browser 的打开选择框方法
        FileBrowser.ShowLoadDialog((selectedFiles) =>
        {
            // 选择成功的回调内部

            // 获取选择的文件的路径
            string path = selectedFiles[0];

            // 准备一个 Sprite 字段来接收读取出来的图片
            Sprite customSprite = null;
            try
            {
                // 从文件读取图片
                customSprite = LoadSpriteFromImageFilePath(path);
            }
            catch (FileNotFoundException)
            {
                InfoDialogController.Show("文件不存在", 1.5f);
                return;
            }
            catch (FormatException)
            {
                InfoDialogController.Show("无法解析文件", 1.5f);
                return;
            }

            // 没有读取到图片，不管什么原因都不能进行下去
            if (customSprite == null)
            {
                InfoDialogController.Show("没能读取到文件", 1.5f);
                return;
            }

            // 遍历预览效果的图片组件，给他们设置图片
            imageComponents.ForEach(image => image.sprite = customSprite);

            // 创建本地存放自定义图片的目录，仅在没有的时候创建
            Directory.CreateDirectory(CUSTOM_IMAGES_PATH);

            // 拼接目录和文件名，作为保存的文件
            string localPath = CUSTOM_IMAGES_PATH + localImageName;

            // 写入本地文件
            File.WriteAllBytes(localPath, File.ReadAllBytes(path));

        }, () => { Debug.Log("取消选择自定义图片"); }, FileBrowser.PickMode.Files, false, null, null, "选择自定义图片", "选择");
    }

    /// <summary>
    /// 将指定路径的图片读取为为 <see cref="Sprite"/>
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="FileNotFoundException">当文件不存在时抛出这个异常</exception>
    /// <exception cref="FormatException">当文件不能转为 Texture2D 时抛出这个异常</exception>
    /// <returns></returns>
    public static Sprite LoadSpriteFromImageFilePath(string path)
    {
        Debug.Log("将文件 " + path + " 转为 Sprite");

        // 如果文件不存在，直接抛异常
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("文件不存在");
        }

        // 准备一个 Texture2D 接收文件数据
        Texture2D texture = new Texture2D(1, 1);

        // 读取文件数据并在读取失败的情况下抛出异常
        if (!(texture.LoadImage(File.ReadAllBytes(path))))
        {
            throw new FormatException("此文件无法读取为 Texture2D 数据");
        }

        // 用 Texture2D 创建 Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);

        // 返回
        return sprite;
    }

    /// <summary>
    /// 取消自定义桌布
    /// </summary>
    public void ClearCustomTableCloth()
    {
        RemoveCustomImage(new List<Image>() { tableClothImage }, tableClothAssetBundleName, tableClothResourcesName, tableClothImageName);
    }

    /// <summary>
    /// 取消自定义桌垫
    /// </summary>
    public void ClearCustomCushion()
    {
        // 激活桌垫物体
        cushionImage.gameObject.SetActive(true);

        RemoveCustomImage(new List<Image>() { cushionImage }, cushionAssetBundleName, cushionResourcesName, cushionImageName);

        // 保存使用桌垫的设置
        PlayerPrefs.SetInt("UseCushion", Convert.ToInt32(true));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 取消自定义卡背
    /// </summary>
    public void ClearCustomCardBack()
    {
        RemoveCustomImage(cardBackImages, cardBackAssetBundleName, cardBackResourcesName, cardBackImageName);
    }

    /// <summary>
    /// 移除自定义图片
    /// </summary>
    /// <param name="imageComponents">用于预览效果的图片组件</param>
    /// <param name="assetBundleName">使用这个图片的组件在没有自定义图片的情况下使用的图片所处的资源包的名称</param>
    /// <param name="resourcesName">使用这个图片的组件在没有自定义图片的情况下使用的图片的名称</param>
    /// <param name="customImageFileName">这个自定义图片在本地保存的名称</param>
    private void RemoveCustomImage(List<Image> imageComponents, string assetBundleName, string resourcesName, string customImageFileName)
    {
        // 把预览图片组件的图改回去
        imageComponents.ForEach(image => image.sprite = AssetBundleTools.Instance.LoadAsset<Sprite>(assetBundleName, resourcesName));

        // 如果本地有自定义图片，把这个文件删除掉
        if (File.Exists(CUSTOM_IMAGES_PATH + customImageFileName))
        {
            File.Delete(CUSTOM_IMAGES_PATH + customImageFileName);
        }
    }

    /// <summary>
    /// 移除桌垫
    /// </summary>
    public void RemoveCushion()
    {
        // 禁用桌垫物体
        cushionImage.gameObject.SetActive(false);

        // 保存不使用桌垫的设置
        PlayerPrefs.SetInt("UseCushion", Convert.ToInt32(false));
        PlayerPrefs.Save();

        // 移除自定义的桌垫图片
        RemoveCustomImage(new List<Image>() { cushionImage }, cushionAssetBundleName, cushionResourcesName, cushionImageName);
    }
}
