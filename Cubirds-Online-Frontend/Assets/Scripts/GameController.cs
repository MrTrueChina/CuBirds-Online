﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏控制器，就是发牌员和主持
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static GameController Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (typeof(GameController))
            {
                if(instance == null)
                {
                    instance = GameObject.FindWithTag("GameController").GetComponent<GameController>();
                }

                return instance;
            }
        }
    }
    private static GameController instance;

    /// <summary>
    /// 桌面画布
    /// </summary>
    public Canvas TableCanvas { get { return tableCanvas; } }
    [SerializeField]
    [Header("桌面画布")]
    private Canvas tableCanvas;
    /// <summary>
    /// 卡组控制器
    /// </summary>
    public DeckController DeckController { get { return deckController; } }
    [SerializeField]
    [Header("卡组控制器")]
    private DeckController deckController;
    /// <summary>
    /// 弃牌堆控制器
    /// </summary>
    public DiscardCardsController DiscardCardsController { get { return discardCardsController; } }
    [SerializeField]
    [Header("弃牌堆控制器")]
    private DiscardCardsController discardCardsController;
    /// <summary>
    /// 中央区行控制器
    /// </summary>
    public List<CenterAreaLineController> CenterAreaLineControllers { get { return centerAreaLineControllers; } }
    [SerializeField]
    [Header("中央区行控制器")]
    private List<CenterAreaLineController> centerAreaLineControllers;
    /// <summary>
    /// 玩家位置列表
    /// </summary>
    [SerializeField]
    [Header("玩家位置列表")]
    private List<Transform> playerPositions;

    /// <summary>
    /// 卡牌预制
    /// </summary>
    public GameObject CardPrefab { get { return cardPrefab; } }
    [SerializeField]
    [Header("卡牌预制")]
    private GameObject cardPrefab;
    /// <summary>
    /// 卡组数据
    /// </summary>
    public CardsData CardsData { get { return cardsData; } }
    [SerializeField]
    [Header("卡牌数据")]
    private CardsData cardsData;

    /// <summary>
    /// 所有玩家
    /// </summary>
    public List<PlayerController> players { get; private set; } = new List<PlayerController>();
    /// <summary>
    /// 当前回合操作的玩家
    /// </summary>
    public PlayerController CurrentTrunPlayre { get; private set; }

    private void Start()
    {
        // 开始游戏
        StartGame();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        // 生成所有玩家
        for (int i = 0; i < playerPositions.Count; i++)
        {
            // 创建玩家物体
            PlayerController player = new GameObject("Player " + i).AddComponent<PlayerController>();

            // 初始化
            player.Init(i, playerPositions[i], TableCanvas);

            // 保存这个玩家
            players.Add(player);
        }

        // 将第一个玩家设为现在回合的玩家
        CurrentTrunPlayre = players[0];

        // 通知牌组控制器初始化牌组
        DeckController.InitDeck(() =>
        {
            Debug.Log("初始化卡组回调执行");

            // 向中央区摆放 3*4 的卡牌矩阵作为开局基础
            DeckController.FillCenterArea(() =>
            {
                Debug.Log("填充中央区回调执行");

                // 把摆放矩阵期间弃掉的牌收回主卡组
                DiscardCardsController.BackToDeck(() =>
                {
                    Debug.Log("弃牌区返回卡组回调执行");

                    // 弃掉的牌收回主卡组后洗牌
                    DeckController.Shuffle(() =>
                    {
                        Debug.Log("洗牌回调执行");

                        // 给所有玩家发 8 张牌
                        DeckController.DealCards(players, 8, () =>
                        {
                            Debug.Log("发牌回调执行");

                            // 给每个玩家发一个鸟群卡
                            DeckController.GivePlayersStartGroup(() =>
                            {
                                Debug.Log("发出初始鸟群卡回调执行");

                                // 进入打牌阶段，这是游戏循环流程的入口，之后转为游戏内部进行循环
                                PlayBirdCards();
                            });
                        });
                    });
                });
            });
        });
    }

    /// <summary>
    /// 打牌阶段
    /// </summary>
    private void PlayBirdCards()
    {
        // 交给协程进行
        StartCoroutine(PlayBirdCardsCoroutine());
    }
    /// <summary>
    /// 打牌阶段的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayBirdCardsCoroutine()
    {
        Debug.Log("打牌阶段协程启动");

        // 打出牌的玩家的 ID
        int playerId = default;
        // 玩家打出的牌的类型
        CardType cardType = default;
        // 打到的行的索引
        int lineIndex = default;
        // 是否打在左边
        bool isLeft = default;

        // 玩家是否已经打出了牌
        bool playerPutedCards = false;

        // 监听玩家打出牌事件
        InputController.Instance.OnPlayerPlayCardsEvent.AddListener((eventPlayerId,eventCardType, eventLineIndex, eventIsLeft)=> {
            // 保存玩家操作的参数
            playerId = eventPlayerId;
            cardType = eventCardType;
            lineIndex = eventLineIndex;
            isLeft = eventIsLeft;
            // 记录玩家已经打出牌
            playerPutedCards = true;
        });

        // 显示玩家选择打牌面板
        PlayCardsController.Instance.StartPlayCards();

        // 等待玩家打出牌
        yield return new WaitUntil(() => playerPutedCards);

        Debug.LogFormat("游戏主控制器确认到玩家 {0} 打出牌 {1}，在第 {2} 行，是否在左边 {3}", playerId, cardType, lineIndex, isLeft);

        // 通知这个玩家打牌
        players.Find(p => p.Id == playerId).PlayCards(cardType, centerAreaLineControllers[lineIndex], isLeft, getCardsNumber =>
        {
            if(getCardsNumber > 0)
            {
                // 拿到了牌

                // 玩家打完牌后进入组群阶段
                MakeGroup();
            }
            else
            {
                // 没拿到牌

                // 选择是否抽牌阶段
                SelectDrawCard(() =>
                {
                    // 选择完后再组群
                    MakeGroup();
                });
            }
        });
    }

    /// <summary>
    /// 没收到牌时选择是否抽牌的阶段
    /// </summary>
    /// <param name="callback"></param>
    private void SelectDrawCard(Action callback)
    {
        // 交给协程进行
        StartCoroutine(SelectDrawCardCoroutine(callback));
    }
    /// <summary>
    /// 没收到牌时选择是否抽牌的阶段
    /// </summary>
    /// 没收到牌时选择是否抽牌的阶段
    /// <returns></returns>
    private IEnumerator SelectDrawCardCoroutine(Action callback)
    {
        Debug.Log("选择是否抽牌阶段协程启动");

        // 记录玩家是否进行了选择的标志变量
        bool selected = false;
        // 进行操作的玩家 id
        int playerId = default;
        // 是否抽牌
        bool drawCrads = default;

        // 监听输入控制器的玩家选择抽牌事件
        InputController.Instance.OnPlayerDrawCardsEvent.AddListener(eventPlayerId =>
        {
            // 记录玩家 id
            playerId = eventPlayerId;
            // 记录进行抽牌
            drawCrads = true;
            // 改为已经选择
            selected = true;
        });
        // 监听输入控制器的玩家选择不抽牌事件
        InputController.Instance.OnPlayerDontDrawCardsEvent.AddListener(eventPlayerId =>
        {
            // 记录玩家 id
            playerId = eventPlayerId;
            // 记录不进行抽牌
            drawCrads = false;
            // 改为已经选择
            selected = true;
        });

        // 通知选择抽不抽牌操作面板开始操作
        SelectDrawController.Instance.StartSelect();

        // 等待玩家选择
        yield return new WaitUntil(() => selected);

        if (drawCrads)
        {
            // 玩家选择抽牌

            // 让牌组发两张牌给玩家
            deckController.DealCards(players.Find(p => p.Id == playerId), 2, () =>
            {
                // 发牌完成后进入组群阶段
                MakeGroup();
            });
        }
        else
        {
            // 玩家选择不抽牌

            // 直接进入组群阶段
            MakeGroup();
        }
    }

    /// <summary>
    /// 组群阶段
    /// </summary>
    private void MakeGroup()
    {
        // 交给协程进行
        StartCoroutine(MakeGroupCoroutine());
    }
    /// <summary>
    /// 组群阶段的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator MakeGroupCoroutine()
    {
        Debug.Log("组群阶段协程启动");

        // 玩家完成进行组群操作的标志变量
        bool maked = false;
        // 进行操作的玩家 id
        int playerId = default;
        // 玩家选择进行组群
        bool makeGroup = default;
        // 玩家组群的鸟类
        CardType cardType = default;

        // 打开组群操作面板
        MakeGroupController.Instance.StartMakeGroup();

        // 订阅输入控制器的玩家进行组群事件
        InputController.Instance.OnPlayerMakeGroupEvent.AddListener((eventPlayerId, eventCardType) =>
        {
            // 记录玩家 id
            playerId = eventPlayerId;
            // 记录组群的鸟类
            cardType = eventCardType;
            // 记录进行组群
            makeGroup = true;
            // 记录为已进行组群操作
            maked = true;
        });
        // 订阅输入控制器的玩家不组群事件
        InputController.Instance.OnPlayerDontMakeGroupEvent.AddListener((eventPlayerId) =>
        {
            // 记录玩家 id
            playerId = eventPlayerId;
            // 记录不进行组群
            makeGroup = false;
            // 记录为已进行组群操作
            maked = true;
        });

        // 等待玩家完成组群操作
        yield return new WaitUntil(() => maked);

        if (makeGroup)
        {
            // 如果进行组群

            // 通知这个玩家进行组群
            players.Find(p => p.Id == playerId).MakeGroup(cardType, () =>
            {
                // 组群完成后执行空手判断阶段
                EmptyHandCheck();
            });
        }
        else
        {
            // 如果不进行组群

            // 立刻进行空手判断阶段
            EmptyHandCheck();
        }
    }

    /// <summary>
    /// 空手判断阶段
    /// </summary>
    private void EmptyHandCheck()
    {
        // 交给协程进行
        StartCoroutine(EmptyHandCheckCoroutine());
    }
    /// <summary>
    /// 空手判断阶段的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator EmptyHandCheckCoroutine()
    {
        Debug.Log("空手判断阶段协程启动");

        if(CurrentTrunPlayre.HandCardsCount() == 0)
        {
            // 玩家手里没牌了

            // 完成弃牌的玩家的计数器
            int discardCardsPlayersNumber = 0;

            // 遍历所有玩家
            players.ForEach(player =>
            {
                // 把手牌全都扔进弃牌堆
                player.DiscardAllHandCards(() =>
                {
                    // 完成弃牌的玩家计数器增加
                    discardCardsPlayersNumber++;
                });
            });

            // 等待所有玩家弃牌完成
            yield return new WaitUntil(() => discardCardsPlayersNumber == players.Count );

            // 记录发牌是否完成的计数器
            bool dealCarded = false;

            // 让牌堆发牌
            deckController.DealCards(players, 8, () =>
            {
                // 记录完成发牌
                dealCarded = true;
            });

            // 等待发牌完毕
            yield return new WaitUntil(() => dealCarded);

            // 回到打牌阶段，这个玩家再来一回合
            PlayBirdCards();
        }
        else
        {
            // 玩家手里还有牌

            // 回合交给下一个玩家
            CurrentTrunPlayre = players.IndexOf(CurrentTrunPlayre) != players.Count - 1 ? CurrentTrunPlayre = players[players.IndexOf(CurrentTrunPlayre) + 1] : players[0];

            Debug.LogFormat("回合交给玩家 {0}", CurrentTrunPlayre);

            // 回到打牌阶段
            PlayBirdCards();
        }
    }
}
