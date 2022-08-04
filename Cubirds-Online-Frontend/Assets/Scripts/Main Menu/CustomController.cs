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
    /// ���ش���Զ���ͼƬ��·��
    /// </summary>
    public static readonly string CUSTOM_IMAGES_PATH = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Custom" + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar;

    /// <summary>
    /// ����ͼƬ���
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ���")]
    private Image tableClothImage;
    /// <summary>
    /// ����ͼƬ�ı����ļ���
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�ı����ļ���")]
    private string tableClothImageName;
    /// <summary>
    /// ����ͼƬ�� AssetBundle ����
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�� AssetBundle ����")]
    private string tableClothAssetBundleName;
    /// <summary>
    /// ����ͼƬ�� AssetBundle ��Դ��
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�� AssetBundle ��Դ��")]
    private string tableClothResourcesName;
    /// <summary>
    /// ����ͼƬ���
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("����ͼƬ���")]
    private Image cushionImage;
    /// <summary>
    /// ����ͼƬ�ı����ļ���
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�ı����ļ���")]
    private string cushionImageName;
    /// <summary>
    /// ����ͼƬ�� AssetBundle ����
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�� AssetBundle ����")]
    private string cushionAssetBundleName;
    /// <summary>
    /// ����ͼƬ�� AssetBundle ��Դ��
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�� AssetBundle ��Դ��")]
    private string cushionResourcesName;
    /// <summary>
    /// ����ͼƬ���
    /// </summary>
    [SerializeField]
    [Space(20)]
    [Header("����ͼƬ���")]
    private List<Image> cardBackImages;
    /// <summary>
    /// ����ͼƬ�ı����ļ���
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�ı����ļ���")]
    private string cardBackImageName;
    /// <summary>
    /// ����ͼƬ�� AssetBundle ����
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�� AssetBundle ����")]
    private string cardBackAssetBundleName;
    /// <summary>
    /// ����ͼƬ�� AssetBundle ��Դ��
    /// </summary>
    [SerializeField]
    [Header("����ͼƬ�� AssetBundle ��Դ��")]
    private string cardBackResourcesName;

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        // WebGL û���Զ��幦�ܣ�ֱ�� return
        return;
#endif

        // ��ʼ��������������������ͨ�õ��Զ���+�ȸ�����ʾ�Ϳ�����
        InitCardBack();

        // ��ȡ�Ƿ�ʹ����������ã�������������
        cushionImage.gameObject.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("UseCushion", Convert.ToInt32(true))));
    }

    /// <summary>
    /// ��ʼ������
    /// </summary>
    private void InitCardBack()
    {
        Sprite cardBackSprite = null;

        if (File.Exists(CUSTOM_IMAGES_PATH + cardBackImageName))
        {
            // �������Զ���ͼƬ���ڣ�ʹ���Զ���ͼƬ��Ϊ����
            cardBackSprite = LoadSpriteFromImageFilePath(CUSTOM_IMAGES_PATH + cardBackImageName);
        }
        else
        {
            // �������Զ���ͼƬ�����ڣ����� AssetBundle �е�ͼƬ��Ϊ����
            cardBackSprite = AssetBundleTools.Instance.LoadAsset<Sprite>(cardBackAssetBundleName, cardBackResourcesName);
        }

        cardBackImages.ForEach(image => image.sprite = cardBackSprite);
    }

    /// <summary>
    /// ѡ���Զ�������
    /// </summary>
    public void SelectCustomTabelCloth()
    {
        SelectCustomImage(new List<Image>() { tableClothImage }, tableClothImageName);
    }

    /// <summary>
    /// ѡ���Զ�������
    /// </summary>
    public void SelectCustomCushion()
    {
        // ������������
        cushionImage.gameObject.SetActive(true);

        SelectCustomImage(new List<Image>() { cushionImage }, cushionImageName);

        // ����ʹ�����������
        PlayerPrefs.SetInt("UseCushion", Convert.ToInt32(true));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ѡ���Զ��忨��
    /// </summary>
    public void SelectCustomCardBack()
    {
        SelectCustomImage(cardBackImages, cardBackImageName);
    }

    /// <summary>
    /// ѡ���Զ���ͼƬ
    /// </summary>
    /// <param name="imageComponents">����Ԥ��Ч����ͼƬ���</param>
    /// <param name="localImageName">�Զ���ͼƬ�ڱ��ش洢���ļ���</param>
    private void SelectCustomImage(List<Image> imageComponents, string localImageName)
    {
        // Runtime File Browser �Ĵ�ѡ��򷽷�
        FileBrowser.ShowLoadDialog((selectedFiles) =>
        {
            // ѡ��ɹ��Ļص��ڲ�

            // ��ȡѡ����ļ���·��
            string path = selectedFiles[0];

            // ׼��һ�� Sprite �ֶ������ն�ȡ������ͼƬ
            Sprite customSprite = null;
            try
            {
                // ���ļ���ȡͼƬ
                customSprite = LoadSpriteFromImageFilePath(path);
            }
            catch (FileNotFoundException)
            {
                InfoDialogController.Show("�ļ�������", 1.5f);
                return;
            }
            catch (FormatException)
            {
                InfoDialogController.Show("�޷������ļ�", 1.5f);
                return;
            }

            // û�ж�ȡ��ͼƬ������ʲôԭ�򶼲��ܽ�����ȥ
            if (customSprite == null)
            {
                InfoDialogController.Show("û�ܶ�ȡ���ļ�", 1.5f);
                return;
            }

            // ����Ԥ��Ч����ͼƬ���������������ͼƬ
            imageComponents.ForEach(image => image.sprite = customSprite);

            // �������ش���Զ���ͼƬ��Ŀ¼������û�е�ʱ�򴴽�
            Directory.CreateDirectory(CUSTOM_IMAGES_PATH);

            // ƴ��Ŀ¼���ļ�������Ϊ������ļ�
            string localPath = CUSTOM_IMAGES_PATH + localImageName;

            // д�뱾���ļ�
            File.WriteAllBytes(localPath, File.ReadAllBytes(path));

        }, () => { Debug.Log("ȡ��ѡ���Զ���ͼƬ"); }, FileBrowser.PickMode.Files, false, null, null, "ѡ���Զ���ͼƬ", "ѡ��");
    }

    /// <summary>
    /// ��ָ��·����ͼƬ��ȡΪΪ <see cref="Sprite"/>
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="FileNotFoundException">���ļ�������ʱ�׳�����쳣</exception>
    /// <exception cref="FormatException">���ļ�����תΪ Texture2D ʱ�׳�����쳣</exception>
    /// <returns></returns>
    public static Sprite LoadSpriteFromImageFilePath(string path)
    {
        Debug.Log("���ļ� " + path + " תΪ Sprite");

        // ����ļ������ڣ�ֱ�����쳣
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("�ļ�������");
        }

        // ׼��һ�� Texture2D �����ļ�����
        Texture2D texture = new Texture2D(1, 1);

        // ��ȡ�ļ����ݲ��ڶ�ȡʧ�ܵ�������׳��쳣
        if (!(texture.LoadImage(File.ReadAllBytes(path))))
        {
            throw new FormatException("���ļ��޷���ȡΪ Texture2D ����");
        }

        // �� Texture2D ���� Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);

        // ����
        return sprite;
    }

    /// <summary>
    /// ȡ���Զ�������
    /// </summary>
    public void ClearCustomTableCloth()
    {
        RemoveCustomImage(new List<Image>() { tableClothImage }, tableClothAssetBundleName, tableClothResourcesName, tableClothImageName);
    }

    /// <summary>
    /// ȡ���Զ�������
    /// </summary>
    public void ClearCustomCushion()
    {
        // ������������
        cushionImage.gameObject.SetActive(true);

        RemoveCustomImage(new List<Image>() { cushionImage }, cushionAssetBundleName, cushionResourcesName, cushionImageName);

        // ����ʹ�����������
        PlayerPrefs.SetInt("UseCushion", Convert.ToInt32(true));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ȡ���Զ��忨��
    /// </summary>
    public void ClearCustomCardBack()
    {
        RemoveCustomImage(cardBackImages, cardBackAssetBundleName, cardBackResourcesName, cardBackImageName);
    }

    /// <summary>
    /// �Ƴ��Զ���ͼƬ
    /// </summary>
    /// <param name="imageComponents">����Ԥ��Ч����ͼƬ���</param>
    /// <param name="assetBundleName">ʹ�����ͼƬ�������û���Զ���ͼƬ�������ʹ�õ�ͼƬ��������Դ��������</param>
    /// <param name="resourcesName">ʹ�����ͼƬ�������û���Զ���ͼƬ�������ʹ�õ�ͼƬ������</param>
    /// <param name="customImageFileName">����Զ���ͼƬ�ڱ��ر��������</param>
    private void RemoveCustomImage(List<Image> imageComponents, string assetBundleName, string resourcesName, string customImageFileName)
    {
        // ��Ԥ��ͼƬ�����ͼ�Ļ�ȥ
        imageComponents.ForEach(image => image.sprite = AssetBundleTools.Instance.LoadAsset<Sprite>(assetBundleName, resourcesName));

        // ����������Զ���ͼƬ��������ļ�ɾ����
        if (File.Exists(CUSTOM_IMAGES_PATH + customImageFileName))
        {
            File.Delete(CUSTOM_IMAGES_PATH + customImageFileName);
        }
    }

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    public void RemoveCushion()
    {
        // ������������
        cushionImage.gameObject.SetActive(false);

        // ���治ʹ�����������
        PlayerPrefs.SetInt("UseCushion", Convert.ToInt32(false));
        PlayerPrefs.Save();

        // �Ƴ��Զ��������ͼƬ
        RemoveCustomImage(new List<Image>() { cushionImage }, cushionAssetBundleName, cushionResourcesName, cushionImageName);
    }
}
