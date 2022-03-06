using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 弃牌区控制器
/// </summary>
public class DiscardCardsController : MonoBehaviour
{
    /// <summary>
    /// 弃牌堆位置
    /// </summary>
    [SerializeField]
    [Header("弃牌堆位置")]
    private Transform discardCardsPosition;
    /// <summary>
    /// 弃牌位置偏移范围
    /// </summary>
    [SerializeField]
    [Header("弃牌位置偏移范围（正负）")]
    private Vector3 discardOffsetRange;

    /// <summary>
    /// 弃牌区中的卡牌
    /// </summary>
    public List<Card> Cards { get; private set; } = new List<Card>();

    /// <summary>
    /// 将牌弃入这个弃牌堆
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动的时间</param>
    public void TakeCard(Card card, Action callback = null, float duration = 0.5f)
    {
        // 交给协程进行
        StartCoroutine(TakeCardCoroutine(card, callback, duration));
    }
    /// <summary>
    /// 将牌弃入这个弃牌堆的协程
    /// </summary>
    /// <param name="card"></param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动的时间</param>
    public IEnumerator TakeCardCoroutine(Card card, Action callback, float duration)
    {
        // 移动卡牌到弃牌堆位置并等待卡牌移动到位
        bool moved = false;
        card.MoveToAndRotateTo(GetDiscardPosition(), discardCardsPosition.rotation, duration, () => { moved = true; });
        yield return new WaitUntil(() => moved);

        // 加入到列表里
        Cards.Add(card);

        // 设置显示层级
        card.SetDisplaySort(Cards.Count);

        // 执行回调
        if (callback != null)
        {
            callback.Invoke();
        }
    }

    /// <summary>
    /// 将弃牌区的牌返回主卡组，注意这个操作不会导致主卡组洗牌
    /// </summary>
    /// <param name="callback"></param>
    public void BackToDeck(Action callback = null)
    {
        // 让协程处理
        StartCoroutine(BackToDeckCoroutine(callback));
    }
    /// <summary>
    /// 将弃牌区的牌返回主卡组的协程，注意这个操作不会导致主卡组洗牌
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator BackToDeckCoroutine(Action callback = null)
    {
        // 记录要发出的牌的总数
        int sendedCards = Cards.Count;
        // 记录对方收到的卡的总数的计数器
        int takedCards = 0;

        // 遍历所有的牌
        Cards.ForEach(card =>
        {
            // 让牌移动到卡组的位置
            card.MoveToAndRotateTo(GameController.Instance.DeckController.DeckPosition.position, GameController.Instance.DeckController.DeckPosition.rotation, () =>
            {
                // 牌移动到卡组位置后把牌给卡组
                GameController.Instance.DeckController.TakeCard(card, () =>
                {
                    // 增加收到的卡的计数器
                    takedCards++;
                });
            });
        });

        // 等所有的牌都被卡组接收
        yield return new WaitUntil(() => takedCards == sendedCards);

        // 执行回调
        callback.Invoke();
    }

    /// <summary>
    /// 获取弃牌位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetDiscardPosition()
    {
        // 在弃牌堆随机范围内返回一个位置，这样弃牌堆看起来会比较乱，比较有弃牌的感觉
        return discardCardsPosition.position + new Vector3(Random.Range(discardOffsetRange.x, -discardOffsetRange.x),
            Random.Range(discardOffsetRange.y, -discardOffsetRange.y),
            Random.Range(discardOffsetRange.z, -discardOffsetRange.z));
    }
}
