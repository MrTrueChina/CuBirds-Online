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
    /// <param name="discardCards"></param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动的时间</param>
    public void TakeCard(List<Card> discardCards, Action callback = null, float duration = 0.5f)
    {
        // 交给协程进行
        StartCoroutine(TakeCardCoroutine(discardCards, callback, duration));
    }
    /// <summary>
    /// 将牌弃入这个弃牌堆的协程
    /// </summary>
    /// <param name="discardCards"></param>
    /// <param name="callback"></param>
    /// <param name="duration">卡牌移动的时间</param>
    public IEnumerator TakeCardCoroutine(List<Card> discardCards, Action callback, float duration)
    {
        // 移动卡牌到弃牌堆位置并等待卡牌移动到位
        int movedCardsNumber = 0;
        discardCards.ForEach(c => c.MoveToAndRotateTo(GetDiscardPosition(), discardCardsPosition.rotation, duration, () => movedCardsNumber++));
        yield return new WaitUntil(() => movedCardsNumber == discardCards.Count);

        // 播放放下卡牌音效
        CardSoundController.Instance.PlayPutCardSound(0.1f);

        // 加入到列表里
        Cards.AddRange(discardCards);

        // 设置显示层级
        for(int i = 0;i < Cards.Count; i++)
        {
            Cards[i].SetDisplaySort(i);
        }

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
        Debug.Log("将弃牌区的牌返回主卡组");

        // 复制一份牌列表作为要返回的牌的列表
        List<Card> needReturnCards = new List<Card>(Cards);
        // 清空卡牌列表
        Cards.Clear();

        // 把牌发给卡组并等待卡组完成接收
        bool deckGeted = false;
        GameController.Instance.DeckController.TakeCard(needReturnCards, () => deckGeted = true, 0.3f);
        yield return new WaitUntil(() => deckGeted);

        Debug.Log("将弃牌区的牌返回主卡组完成");

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
        return discardCardsPosition.position
            // 随机偏移一点位置，让弃牌堆看起来像是散乱丢弃的
            + new Vector3(Random.Range(discardOffsetRange.x, -discardOffsetRange.x), Random.Range(discardOffsetRange.y, -discardOffsetRange.y), Random.Range(discardOffsetRange.z, -discardOffsetRange.z))
            // 调整高度，让弃牌堆的牌看起来也有厚度
            + Vector3.up * Cards.Count;
    }
}
