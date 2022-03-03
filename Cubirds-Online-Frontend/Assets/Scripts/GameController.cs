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
    public PlayerController currentTrunPlayre { get; private set; }

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

                        // 发牌给所有玩家
                        DeckController.DealCards(() =>
                        {
                            Debug.Log("发牌回调执行");

                            // 给每个玩家发一个鸟群卡
                            DeckController.GivePlayersStartGroup(() =>
                            {
                                Debug.Log("发出初始鸟群卡回调执行");
                            });
                        });
                    });
                });
            });
        });
    }

    /// <summary>
    /// 单个玩家的回合
    /// </summary>
    private void PlayerTurn()
    {
        // 打出牌

        // 组群
        
        // 对空手的判断

        // 下一回合
    }

    /// <summary>
    /// 打牌阶段
    /// </summary>
    private void PlayBirdCards()
    {

    }

    /// <summary>
    /// 组群阶段
    /// </summary>
    private void MakeGroup()
    {

    }

    /// <summary>
    /// 空手判断阶段
    /// </summary>
    private void EmptyHandCheck()
    {

    }
}
