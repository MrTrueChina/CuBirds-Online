using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Text = UnityEngine.UI.Text;
using System.Text;
using CubirdsOnline.Common;

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
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(GameController))
            {
                if (instance == null)
                {
                    instance = GameObject.FindWithTag("GameController").GetComponent<GameController>();
                }

                return instance;
            }
        }
    }
    private static GameController instance;

    /// <summary>
    /// 游戏范围画布
    /// </summary>
    public Canvas PlayGameCanvas { get { return playGameCanvas; } }
    [SerializeField]
    [Header("游戏范围画布")]
    private Canvas playGameCanvas;
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
    /// 提示文字文本组件
    /// </summary>
    [SerializeField]
    [Header("提示文字文本组件")]
    private Text tipsText;
    /// <summary>
    /// 胜利面板画布
    /// </summary>
    [SerializeField]
    [Header("胜利面板画布")]
    private GameObject winCanvas;
    /// <summary>
    /// 胜利面板文本组件
    /// </summary>
    [SerializeField]
    [Header("胜利面板文本组件")]
    private Text winText;

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
    /// 一个回合的最长时间
    /// </summary>
    public float MaxTurnTime { get { return maxTurnTime; } }
    [SerializeField]
    [Header("一个回合的最长时间（秒）")]
    private float maxTurnTime = 60;

    /// <summary>
    /// 所有玩家
    /// </summary>
    public List<PlayerController> players { get; private set; } = new List<PlayerController>();
    /// <summary>
    /// 当前回合操作的玩家
    /// </summary>
    public PlayerController CurrentTrunPlayre { get; private set; }
    /// <summary>
    /// 本机玩家
    /// </summary>
    public PlayerController LocalPlayer { get; private set; }

    /// <summary>
    /// 打牌阶段的协程
    /// </summary>
    private Coroutine playBirdCardsCoroutine;
    /// <summary>
    /// 选择是否抽牌阶段的协程
    /// </summary>
    private Coroutine selectDrawCardCoroutine;
    /// <summary>
    /// 组群阶段的协程
    /// </summary>
    private Coroutine makeGroupCoroutine;
    /// <summary>
    /// 空手判断阶段的协程
    /// </summary>
    private Coroutine emptyHandCheckCoroutine;

    private void Start()
    {
        // 订阅玩家超时事件
        InputController.Instance.OnPlayerOutOfTimeEvent.AddListener(OnPlayerTimeOutEvent);

        // 开始游戏
        StartGame();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        // FIXME：这里需要调整位置，本机玩家在最大的位置
        // 生成所有玩家
        for(int i = 0;i < GlobalModel.Instance.TablePlayers.Count; i++)
        {
            PlayerInfoDTO playerInfo = GlobalModel.Instance.TablePlayers[i];

            // 创建玩家物体
            PlayerController playerController = new GameObject("Player " + playerInfo.Id).AddComponent<PlayerController>();

            // 初始化
            playerController.Init(playerInfo.Id, playerPositions[i], PlayGameCanvas);

            // 保存这个玩家
            players.Add(playerController);
        }

        // 将第一个玩家设为现在回合的玩家
        CurrentTrunPlayre = players[0];

        // 设置本机玩家
        LocalPlayer = players.Find(p => p.Id == GlobalModel.Instance.LocalPLayerId);

        // 通知牌组控制器初始化牌组
        DeckController.InitDeck(() =>
        {
            Debug.Log("初始化卡组回调执行");

            ShowTip("摆放中央区初始卡牌……");

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

                        ShowTip("给所有玩家发牌……");

                        // 给所有玩家发 8 张牌
                        DeckController.DealCards(players, 8, () =>
                        {
                            Debug.Log("发牌回调执行");

                            ShowTip("给每个玩家初始鸟群卡……");

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
        // 发出提示
        ShowTip(CurrentPlayerIsLocalPlayer() ? "你需要打出一种鸟类卡，这一操作是必须的。" : string.Format("等待玩家 {0} 打出鸟类卡……", CurrentTrunPlayre.Id));

        // 开始计时
        PlayOutOfTimeTimer.Instance.StartTiming(CurrentTrunPlayre, 60);

        // 交给协程进行
        playBirdCardsCoroutine = StartCoroutine(PlayBirdCardsCoroutine());
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

        // 如果现在是本机玩家的回合则显示玩家选择打牌面板
        if (CurrentPlayerIsLocalPlayer())
        {
            PlayCardsController.Instance.StartPlayCards();
        }

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
                SelectDrawCard();
            }
        });
    }

    /// <summary>
    /// 没收到牌时选择是否抽牌的阶段
    /// </summary>
    private void SelectDrawCard()
    {
        // 发出提示
        ShowTip(CurrentPlayerIsLocalPlayer() ? "因为你没有收到牌，你可以选择从卡组抽两张牌。" : string.Format("等待玩家 {0} 选择是否抽牌……", CurrentTrunPlayre.Id));

        // 交给协程进行
        selectDrawCardCoroutine = StartCoroutine(SelectDrawCardCoroutine());
    }
    /// <summary>
    /// 没收到牌时选择是否抽牌的阶段的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelectDrawCardCoroutine()
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

        // 如果当前玩家是本机玩家，通知选择抽不抽牌操作面板开始操作
        if (CurrentPlayerIsLocalPlayer())
        {
            SelectDrawController.Instance.StartSelect();
        }

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

            if(CurrentTrunPlayre.HandCardsCount() > 0)
            {
                // 玩家还有手牌，进入组群阶段
                MakeGroup();
            }
            else
            {
                // 玩家没有手牌了，直接进入空手判断阶段
                EmptyHandCheck();
            }
        }
    }

    /// <summary>
    /// 组群阶段
    /// </summary>
    private void MakeGroup()
    {
        // 如果现在是其他玩家的回合，显示这个提示
        string otherPlayerTip = string.Format("等待玩家 {0} 组成鸟群……", CurrentTrunPlayre.Id);

        // 如果现在是本机玩家的回合，显示这个提示
        string localPlayerTip = CurrentTrunPlayre.CanMakeGroup() ? "你可以选择将手中的牌组成鸟群，也可以选择不组成鸟群。" : "你手中的牌不能组成鸟群，但你可以等一会再选择不组成鸟群让其他玩家以为你在思考是否组群。";

        // 发出提示
        ShowTip(CurrentPlayerIsLocalPlayer() ? localPlayerTip : otherPlayerTip);

        // 交给协程进行
        makeGroupCoroutine = StartCoroutine(MakeGroupCoroutine());
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

        // 如果当前回合玩家是本机玩家，打开组群操作面板
        if (CurrentPlayerIsLocalPlayer())
        {
            MakeGroupController.Instance.StartMakeGroup();
        }

        // 等待玩家完成组群操作
        yield return new WaitUntil(() => maked);

        if (makeGroup)
        {
            // 如果进行组群

            // 通知这个玩家进行组群
            players.Find(p => p.Id == playerId).MakeGroup(cardType, () =>
            {
                // 组群完成后执行胜利判断阶段
                WinCheck();
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
    /// 胜利检测阶段
    /// </summary>
    private void WinCheck()
    {
        // 记录当前玩家是否胜利的标志变量
        bool currentPlayerWin = false;

        // 如果玩家手里的鸟群牌的种类达到 7 种，这名玩家获胜
        if(CurrentTrunPlayre.GroupCards.Select(c => c.CardType).Distinct().ToList().Count >= 7)
        {
            currentPlayerWin = true;
        }

        // 集齐至少三张卡的鸟类的计数器
        int threeCardsTypeNumber = 0;
        // 遍历所有鸟类
        foreach (CardType cardType in (CardType[])Enum.GetValues(typeof(CardType)))
        {
            // 如果鸟群卡中这种鸟的种类达到了三张则计数器增加
            if(CurrentTrunPlayre.GroupCards.Count(c=>c.CardType == cardType) >= 3)
            {
                threeCardsTypeNumber++;
            }
        }
        // 如果有两种鸟的卡达到了三张，这名玩家获胜
        if(threeCardsTypeNumber >= 2)
        {
            currentPlayerWin = true;
        }

        if (currentPlayerWin)
        {
            // 如果当前玩家胜利

            // 显示胜利信息
            ShowWinInfo(new List<PlayerController>() { CurrentTrunPlayre });

            // 之后不进行其他操作了，流程结束
        }
        else
        {
            // 当前玩家没有胜利

            // 进入空手判断阶段
            EmptyHandCheck();
        }
    }

    /// <summary>
    /// 显示胜利信息
    /// </summary>
    /// <param name="winPlayers"></param>
    private void ShowWinInfo(List<PlayerController> winPlayers)
    {
        // 停止计时
        PlayOutOfTimeTimer.Instance.StopTiming();

        // 玩家胜利文本构造器
        StringBuilder winTextBuilder = new StringBuilder();

        // 添加前缀
        winTextBuilder.Append("玩家 ");

        // 把并列第一的玩家们加进去
        winPlayers.ForEach(player =>
        {
            winTextBuilder.Append(player.Id + "、");
        });

        // 移除最后面的顿号
        winTextBuilder.Remove(winTextBuilder.Length - 1, 1);

        // 添加后缀
        winTextBuilder.Append(" 获胜！");

        // 打开胜利面板
        winCanvas.SetActive(true);

        // 设置胜利文本
        winText.text = winTextBuilder.ToString();
    }

    /// <summary>
    /// 空手判断阶段
    /// </summary>
    private void EmptyHandCheck()
    {
        // 到这个阶段后就停止计时，因为空手后弃牌抽满有大概十秒，这段时间玩家实际上已经没有操作了，防止玩家无辜被超时
        PlayOutOfTimeTimer.Instance.StopTiming();

        // 交给协程进行
        emptyHandCheckCoroutine = StartCoroutine(EmptyHandCheckCoroutine());
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

            // 发出提示
            ShowTip(string.Format("由于{0}将手牌打空，所有玩家弃牌并重新抽牌，{0}将再次获得一个回合。", (CurrentPlayerIsLocalPlayer() ? "你" : "玩家\u00A0" + CurrentTrunPlayre.Id + "\u00A0")));

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

    /// <summary>
    /// 卡牌耗尽
    /// </summary>
    public void RunOutCards()
    {
        // 复制一份玩家列表
        List<PlayerController> sortedPlayers = new List<PlayerController>(players);

        // 按照玩家手里的鸟群卡数量排序
        sortedPlayers.Sort((a, b) => b.GroupCards.Count - a.GroupCards.Count);

        // 筛选出鸟群卡数量和第一名一样的玩家，即并列第一的玩家们
        List<PlayerController> winPlayers = sortedPlayers.Where(player => player.GroupCards.Count == sortedPlayers[0].GroupCards.Count).ToList();

        // 显示胜利信息
        ShowWinInfo(winPlayers);
    }

    /// <summary>
    /// 当有玩家超时时这个方法会被调用
    /// </summary>
    /// <param name="playerId"></param>
    private void OnPlayerTimeOutEvent(int playerId)
    {
        Debug.LogFormat("收到玩家 {0} 超时事件", playerId);

        StartCoroutine(RemovePlayerCoroutine(playerId));
    }
    /// <summary>
    /// 把玩家移除出游戏的协程
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    private IEnumerator RemovePlayerCoroutine(int playerId)
    {
        Debug.LogFormat("将玩家 {0} 移除出游戏", playerId);

        // 立刻停止所有主要的游戏协程，防止被踢出的玩家进一步操作
        StopCoroutine();

        // 找到超时的玩家
        PlayerController timeOutPlayer = players.Find(p => p.Id == playerId);

        // 播放玩家超时的进度条动画并等待动画播放完毕
        bool removePlayerBarComplete = false;
        PlayOutOfTimeTimer.Instance.ShowRemovePlayerBar(timeOutPlayer,3,()=> removePlayerBarComplete = true);
        yield return new WaitUntil(() => removePlayerBarComplete);

        // 让超时的玩家丢弃所有的手牌和鸟群牌
        bool handCardsDiscarded = false;
        bool groupCardsDiscarded = false;
        timeOutPlayer.DiscardAllHandCards(() => handCardsDiscarded = true);
        timeOutPlayer.DiscardAllGroupCards(() => groupCardsDiscarded = true);

        // 等待玩家丢完牌
        yield return new WaitUntil(() => handCardsDiscarded && groupCardsDiscarded);

        // 如果当前回合是超时玩家的回合，把回合交给下一位玩家
        if(CurrentTrunPlayre == timeOutPlayer)
        {
            CurrentTrunPlayre = players.IndexOf(CurrentTrunPlayre) != players.Count - 1 ? CurrentTrunPlayre = players[players.IndexOf(CurrentTrunPlayre) + 1] : players[0];
        }

        // 从玩家列表里移除这个玩家
        players.Remove(timeOutPlayer);

        if(players.Count == 1)
        {
            // 只剩下一个玩家了，这个玩家直接获胜
            ShowWinInfo(new List<PlayerController>() { players[0] });
        }
        else
        {
            // 剩下不止一个玩家，进入打牌阶段
            PlayBirdCards();
        }
    }

    /// <summary>
    /// 停止所有主要的游戏流程的协程
    /// </summary>
    private void StopCoroutine()
    {
        // 打牌阶段的协程
        if(playBirdCardsCoroutine != null)
        {
            StopCoroutine(playBirdCardsCoroutine);
        }
        // 选择是否抽牌阶段的协程
        if(selectDrawCardCoroutine != null)
        {
            StopCoroutine(selectDrawCardCoroutine);
        }
        // 组群阶段的协程
        if(makeGroupCoroutine != null)
        {
            StopCoroutine(makeGroupCoroutine);
        }
        // 空手判断阶段的协程
        if(emptyHandCheckCoroutine != null)
        {
            StopCoroutine(emptyHandCheckCoroutine);
        }
}

    /// <summary>
    /// 检测指定的玩家是不是本机玩家
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsLocalPlayer(PlayerController player)
    {
        // 因为就是对比对象直接用等于号就行
        return player == LocalPlayer;
    }

    /// <summary>
    /// 检测当前回合玩家是不是本机玩家
    /// </summary>
    /// <returns></returns>
    public bool CurrentPlayerIsLocalPlayer()
    {
        return IsLocalPlayer(CurrentTrunPlayre);
    }

    /// <summary>
    /// 显示提示文本
    /// </summary>
    /// <param name="tip"></param>
    public void ShowTip(string tip)
    {
        tipsText.text = tip;
    }
}
