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
    [SerializeField]
    [Header("卡组控制器")]
    private DeckController deckController;
    /// <summary>
    /// 弃牌堆控制器
    /// </summary>
    [SerializeField]
    [Header("弃牌堆控制器")]
    private DiscardCardsController discardCardsController;
    /// <summary>
    /// 中央区行控制器
    /// </summary>
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
        // 生成所有玩家
        for(int i = 0;i < playerPositions.Count; i++)
        {
            // 创建玩家物体
            PlayerController player = new GameObject("Player " + i).AddComponent<PlayerController>();

            // 初始化
            player.Init(i, playerPositions[i], tableCanvas);

            // 保存这个玩家
            players.Add(player);
        }

        // 通知牌组控制器初始化牌组
        deckController.InitDeck(() =>
        {
            Debug.Log("初始化卡组回调执行");

            deckController.DealCards(() =>
            {
                Debug.Log("发牌回调执行");
            });
        });
    }
}
