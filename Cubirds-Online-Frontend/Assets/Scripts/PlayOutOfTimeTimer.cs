using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

/// <summary>
/// 玩家超时计时器
/// </summary>
public class PlayOutOfTimeTimer : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static PlayOutOfTimeTimer Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(PlayOutOfTimeTimer))
            {
                if (instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("PlayOutOfTimeTimer").GetComponent<PlayOutOfTimeTimer>();
                }

                return instance;
            }
        }
    }
    private static PlayOutOfTimeTimer instance;

    /// <summary>
    /// 显示倒计时的画布
    /// </summary>
    [SerializeField]
    [Header("显示倒计时的画布")]
    private Canvas timeCanvas;
    /// <summary>
    /// 显示倒计时的文本组件
    /// </summary>
    [SerializeField]
    [Header("显示倒计时的文本组件")]
    private Text timeText;
    /// <summary>
    /// 显示移除玩家进度条的图片组件
    /// </summary>
    [SerializeField]
    [Header("显示移除玩家进度条的图片组件")]
    private Image removePlayerImage;

    /// <summary>
    /// 显示倒计时的文本到玩家的偏移量
    /// </summary>
    [SerializeField]
    [Header("显示倒计时的文本到玩家的偏移量")]
    private float timeCanvasToPlayerDistance = 150;

    /// <summary>
    /// 对这个玩家进行计时
    /// </summary>
    private PlayerController timerPlayer;

    /// <summary>
    /// 最大时间（初始时间）
    /// </summary>
    private float maxTime;

    /// <summary>
    /// 计时器
    /// </summary>
    private float timer;

    private void Update()
    {
        // 有玩家的时候才进行计时
        if(timerPlayer != null)
        {
            // 减少时间
            timer -= Time.deltaTime;

            // 时间到了
            if(timer <= 0)
            {
                // 发出玩家超时事件
                InputController.Instance.CallPlayerOutOfTime(timerPlayer.Id);

                // 移除计时的玩家，防止重复发出事件
                timerPlayer = null;
            }

            // 显示倒计时文字
            timeText.text = ((int)timer).ToString();
            // 根据计时调整倒计时文字的透明度，倒计时越接近超时文字越清晰
            timeText.color =  new Color(timeText.color.r, timeText.color.g, timeText.color.b, 1 - timer / maxTime);
        }
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    /// <param name="player">对这个玩家进行计时</param>
    /// <param name="time">计时时间</param>
    public void StartTiming(PlayerController player, float time)
    {
        // 保存玩家
        timerPlayer = player;

        // 把显示时间的画布移动到玩家前面
        timeCanvas.transform.position = player.transform.position + player.transform.up * timeCanvasToPlayerDistance;

        // 把移除玩家进度条的填充改为 0
        removePlayerImage.fillAmount = 0;

        // 把文字设置为完全透明
        timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, 0);

        // 保存时间作为最大时间
        maxTime = time;

        // 保存时间
        timer = time;
    }

    /// <summary>
    /// 显示移除玩家的进度条
    /// </summary>
    /// <param name="removePlayer"></param>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    public void ShowRemovePlayerBar(PlayerController removePlayer, float time, Action callback)
    {
        // 把显示时间的画布移动到玩家前面
        timeCanvas.transform.position = removePlayer.transform.position + removePlayer.transform.up * timeCanvasToPlayerDistance;

        // 播放一个填充到满的动画
        var doFill = removePlayerImage.DOFillAmount(1, time);
        // 添加播放完成后的回调
        doFill.onComplete = () => callback.Invoke();
        // 改为匀速动画
        doFill.SetEase(Ease.Linear);
    }
}
