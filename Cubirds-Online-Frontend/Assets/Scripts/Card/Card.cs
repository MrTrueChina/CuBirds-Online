using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.EventSystems;

/// <summary>
/// 卡牌组件
/// </summary>
public class Card : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// 卡牌主画布
    /// </summary>
    [SerializeField]
    [Header("卡牌主画布")]
    private Canvas mainCanvas;
    /// <summary>
    /// 卡图组件
    /// </summary>
    [SerializeField]
    [Header("卡图组件")]
    private Image cardPictureComponent;
    /// <summary>
    /// 鸟群数量角标图片组件
    /// </summary>
    [SerializeField]
    [Header("鸟群数量角标图片组件")]
    private Image groupNumberImageComponent;
    /// <summary>
    /// 鸟类图片组件
    /// </summary>
    [SerializeField]
    [Header("鸟类图片组件")]
    private Image typeImageComponent;
    /// <summary>
    /// 卡背图片
    /// </summary>
    [SerializeField]
    [Header("卡背图片")]
    private Sprite cardBack;
    /// <summary>
    /// 卡图图片
    /// </summary>
    [SerializeField]
    [Header("卡图图片")]
    private Sprite cardPicture;
    /// <summary>
    /// 鸟群数量角标图片
    /// </summary>
    [SerializeField]
    [Header("鸟群数量角标图片")]
    private Sprite groupNumberImage;

    /// <summary>
    /// 鸟类角标图片
    /// </summary>
    [SerializeField]
    [Header("鸟类角标图片")]
    private Sprite typeImage;

    /// <summary>
    /// 这张牌的 ID
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 鸟类
    /// </summary>
    public CardType CardType { get; private set; }
    /// <summary>
    /// 组成小鸟群所需卡牌数量
    /// </summary>
    public int SmallGroupNumber { get; private set; }
    /// <summary>
    /// 组成大鸟群所需卡牌数量
    /// </summary>
    public int BigGroupNumber { get; private set; }

    /// <summary>
    /// 卡牌移动的补间动画
    /// </summary>
    private TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
    /// <summary>
    /// 卡牌旋转的补间动画
    /// </summary>
    private TweenerCore<Quaternion, Quaternion, NoOptions> rotateTween;
    /// <summary>
    /// 保存显示顺序的字段
    /// </summary>
    private int displaySort;

    private void Awake()
    {
        // 设置卡面
        cardPictureComponent.sprite = cardPicture;
        groupNumberImageComponent.sprite = groupNumberImage;
        typeImageComponent.sprite = typeImage;
    }

    private void OnValidate()
    {
        // 卡图
        if (cardPictureComponent != null)
        {
            cardPictureComponent.sprite = cardPicture;
        }
        // 鸟群数量角标
        if (groupNumberImageComponent != null)
        {
            groupNumberImageComponent.sprite = groupNumberImage;
        }
        // 鸟类角标
        if (typeImageComponent != null)
        {
            typeImageComponent.sprite = typeImage;
        }
    }

    /// <summary>
    /// 初始化卡牌
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cardData"></param>
    public void InitData(int id, CardData cardData)
    {
        // ID
        Id = id;

        // 卡图
        cardPicture = cardData.cardPicture;
        cardPictureComponent.sprite = cardPicture;
        // 组群数量角标图片
        groupNumberImage = cardData.groupNumberImage;
        groupNumberImageComponent.sprite = groupNumberImage;
        // 鸟类角标图片
        typeImage = cardData.typeImage;
        typeImageComponent.sprite = typeImage;

        // 鸟类
        CardType = cardData.cardType;
        // 组成小鸟群需要的数量
        SmallGroupNumber = cardData.smallGroupNumber;
        // 组成大鸟群需要的数量
        BigGroupNumber = cardData.bigGroupNumber;
    }

    /// <summary>
    /// 设置卡牌显示次序
    /// </summary>
    /// <param name="sort"></param>
    public void SetDisplaySort(int sort)
    {
        // 记录下来
        displaySort = sort;

        // 设置显示次序
        SetTempDisplaySort(displaySort);
    }

    /// <summary>
    /// 设置卡牌显示次序，但是不保存显示次序
    /// </summary>
    /// <param name="sort"></param>
    private void SetTempDisplaySort(int sort)
    {
        // 画布改为覆盖显示顺序
        mainCanvas.overrideSorting = true;
        // 设置显示顺序
        mainCanvas.sortingOrder = sort;
    }

    /// <summary>
    /// 设置是否明牌，即翻开还是扣下
    /// </summary>
    /// <param name="isOpen"></param>
    public void SetOpen(bool isOpen)
    {
        if (isOpen)
        {
            // 显示卡面
            cardPictureComponent.sprite = cardPicture;
            // 显示组群数量角标
            groupNumberImageComponent.enabled = true;
            // 显示种类角标
            typeImageComponent.enabled = true;
        }
        else
        {
            // 显示卡背
            cardPictureComponent.sprite = cardBack;
            // 隐藏组群数量角标
            groupNumberImageComponent.enabled = false;
            // 隐藏种类角标
            typeImageComponent.enabled = false;
        }
    }

    /// <summary>
    /// 将这张卡移动和旋转到指定位置
    /// </summary>
    /// <param name="targetPosition">移动到的目标位置</param>
    /// <param name="targetRoattion">旋转到的目标旋转值</param>
    /// <param name="duration">移动和旋转的时间</param>
    /// <param name="callback">完成后执行的回调，以移动完成为准</param>
    public void MoveToAndRotateTo(Vector3 targetPosition, Quaternion targetRoattion, float duration = 0.5f, Action callback = null)
    {
        // 取消正在进行的移动补间动画
        if (moveTween != null)
        {
            moveTween.Kill();
        }
        // 取消正在进行的旋转补间动画
        if (rotateTween != null)
        {
            rotateTween.Kill();
        }

        // 设置一个极高的显示顺序，防止卡牌移动的时候被放在桌子上的卡挡住
        SetTempDisplaySort(1000);

        // 使用 DOTween 进行移动并记录这个补间
        moveTween = transform.DOMove(targetPosition, duration);
        // 使用 DOTween 进行移动并记录这个补间
        rotateTween = transform.DORotateQuaternion(targetRoattion, duration);

        // 添加回调
        moveTween.onComplete += () =>
        {
            // 恢复原来的显示顺序
            SetDisplaySort(displaySort);

            // 如果有传入的回调则执行
            if (callback != null)
            {
                callback.Invoke();
            }
        };
    }
    /// <summary>
    /// 将这张卡移动和旋转到指定位置
    /// </summary>
    /// <param name="targetPosition">移动到的目标位置</param>
    /// <param name="targetRoattion">旋转到的目标旋转值</param>
    /// <param name="callback">完成后执行的回调，以移动完成为准</param>
    public void MoveToAndRotateTo(Vector3 targetPosition, Quaternion targetRoattion, Action callback = null)
    {
        MoveToAndRotateTo(targetPosition, targetRoattion, 0.5f, callback);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogFormat("点击卡牌 {0} {1}", CardType, Id);

        // 转发点击事件给输入控制器
        InputController.Instance.CallCardPointClick(this, eventData);
    }
}
