using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubirdsOnline.Backend.RequestSender;
using CubirdsOnline.Common;
using ExitGames.Logging;
using Photon.SocketServer;

namespace CubirdsOnline.Backend.Service
{
    /// <summary>
    /// 游戏过程中进行帧同步的 Service
    /// </summary>
    public class LockstepService
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 同步玩家打出牌操作
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="tableId"></param>
        /// <param name="cardType"></param>
        /// <param name="lineIndex"></param>
        /// <param name="putOnLeft"></param>
        /// <param name="sendParameters"></param>
        public static void LockStepPlayerPlayCards(CubirdClientPeer clientPeer, int tableId, int cardType, int lineIndex, bool putOnLeft, SendParameters sendParameters)
        {
            log.InfoFormat("客户端({0})在 {1} 桌打出 {2} 牌到第 {3} 行 {4}", clientPeer.PlayerId, tableId, cardType, lineIndex, putOnLeft ? "左边" : "右边");

            // 获取桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 没有桌子或者还没开局，不转发
            if (table == null || !table.Playing)
            {
                return;
            }

            // 如果这个桌子上没有这个玩家，不转发
            if(!table.Players.Any(p=>p.Peer.PlayerId == clientPeer.PlayerId))
            {
                return;
            }

            // 准备事件
            EventData eventData = new EventData() { 
                // 事件码
                Code = (byte)EventCode.LOCK_STEP_PLAYER_PLAY_CARDS,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 打牌的玩家的 ID
                    { (byte)EventParamaterKey.PLAYER_ID, clientPeer.PlayerId },
                    // 打出的牌的种类
                    { (byte)EventParamaterKey.CARD_TYPE, cardType },
                    // 打到的中央行的索引
                    { (byte)EventParamaterKey.CENTER_LINE_INDEX, lineIndex },
                    // 是否打在左边
                    { (byte)EventParamaterKey.PUT_ON_CENTER_LINE_LEFT, putOnLeft },
                },
            };

            // 给桌子上的所有玩家转发消息
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });
        }

        /// <summary>
        /// 同步玩家组成鸟群操作
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="tableId"></param>
        /// <param name="cardType"></param>
        /// <param name="sendParameters"></param>
        public static void LockStepPlayerMakeGroup(CubirdClientPeer clientPeer, int tableId, int cardType, SendParameters sendParameters)
        {
            log.InfoFormat("客户端({0})在 {1} 桌打组成 {2} 类型的鸟牌", clientPeer.PlayerId, tableId, cardType);

            // 获取桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 没有桌子或者还没开局，不转发
            if (table == null || !table.Playing)
            {
                return;
            }

            // 如果这个桌子上没有这个玩家，不转发
            if (!table.Players.Any(p => p.Peer.PlayerId == clientPeer.PlayerId))
            {
                return;
            }

            // 准备事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.LOCK_STEP_PLAYER_MAKE_GROUP,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 组群玩家的 ID
                    { (byte)EventParamaterKey.PLAYER_ID, clientPeer.PlayerId },
                    // 组群的牌的种类
                    { (byte)EventParamaterKey.CARD_TYPE, cardType },
                },
            };

            // 给桌子上的所有玩家转发消息
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });
        }

        /// <summary>
        /// 同步玩家不组成鸟群操作
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="tableId"></param>
        /// <param name="sendParameters"></param>
        public static void LockStepPlayerDontMakeGroup(CubirdClientPeer clientPeer, int tableId, SendParameters sendParameters)
        {
            log.InfoFormat("客户端({0})在 {1} 桌打选择不组成鸟群", clientPeer.PlayerId, tableId);

            // 获取桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 没有桌子或者还没开局，不转发
            if (table == null || !table.Playing)
            {
                return;
            }

            // 如果这个桌子上没有这个玩家，不转发
            if (!table.Players.Any(p => p.Peer.PlayerId == clientPeer.PlayerId))
            {
                return;
            }

            // 准备事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.LOCK_STEP_PLAYER_DONT_MAKE_GROUP,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 操作玩家的 ID
                    { (byte)EventParamaterKey.PLAYER_ID, clientPeer.PlayerId },
                },
            };

            // 给桌子上的所有玩家转发消息
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });
        }

        /// <summary>
        /// 同步玩家抽卡操作
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="tableId"></param>
        /// <param name="sendParameters"></param>
        public static void LockStepPlayerDrawCards(CubirdClientPeer clientPeer, int tableId, SendParameters sendParameters)
        {
            log.InfoFormat("客户端({0})在 {1} 桌打选择抽牌", clientPeer.PlayerId, tableId);

            // 获取桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 没有桌子或者还没开局，不转发
            if (table == null || !table.Playing)
            {
                return;
            }

            // 如果这个桌子上没有这个玩家，不转发
            if (!table.Players.Any(p => p.Peer.PlayerId == clientPeer.PlayerId))
            {
                return;
            }

            // 准备事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.LOCK_STEP_PLAYER_DRAW_CARDS,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 操作玩家的 ID
                    { (byte)EventParamaterKey.PLAYER_ID, clientPeer.PlayerId },
                },
            };

            // 给桌子上的所有玩家转发消息
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });
        }

        /// <summary>
        /// 同步玩家选择不抽卡操作
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="tableId"></param>
        /// <param name="sendParameters"></param>
        public static void LockStepPlayerDontDrawCards(CubirdClientPeer clientPeer, int tableId, SendParameters sendParameters)
        {
            log.InfoFormat("客户端({0})在 {1} 桌打选择不抽牌", clientPeer.PlayerId, tableId);

            // 获取桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 没有桌子或者还没开局，不转发
            if (table == null || !table.Playing)
            {
                return;
            }

            // 如果这个桌子上没有这个玩家，不转发
            if (!table.Players.Any(p => p.Peer.PlayerId == clientPeer.PlayerId))
            {
                return;
            }

            // 准备事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.LOCK_STEP_PLAYER_DONT_DRAW_CARDS,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 操作玩家的 ID
                    { (byte)EventParamaterKey.PLAYER_ID, clientPeer.PlayerId },
                },
            };

            // 给桌子上的所有玩家转发消息
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });
        }

        /// <summary>
        /// 同步玩家超时
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="tableId"></param>
        /// <param name="timeOutPlayerId"></param>
        /// <param name="sendParameters"></param>
        public static void LockStepPlayerOutOfTime(CubirdClientPeer clientPeer, int tableId, int timeOutPlayerId, SendParameters sendParameters)
        {
            log.InfoFormat("客户端({0})在 {1} 桌同步玩家 {2} 超时", clientPeer.PlayerId, tableId, timeOutPlayerId);

            // 获取桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 没有桌子或者还没开局，不转发
            if (table == null || !table.Playing)
            {
                log.WarnFormat("桌子 {0} 还没开局", table.Id);
                return;
            }

            // 如果这个桌子上没有发出请求的玩家，不转发
            if (!table.Players.Any(p => p.Peer.PlayerId == clientPeer.PlayerId))
            {
                log.WarnFormat("桌子 {0} 上没有发出请求的玩家 {1}", table.Id, clientPeer.PlayerId);
                return;
            }

            // 找到掉线的玩家，如果这个玩家已经从游戏中移除了也算作找不到
            PlayerInfo timeOutPlayer = table.Players.Find(p => p.Peer.PlayerId == timeOutPlayerId && !p.IsRemoved);

            // 如果这个桌子上没有这个超时的玩家，不转发
            if (timeOutPlayer == null)
            {
                log.WarnFormat("桌子 {0} 上没有超时的玩家 {1}", table.Id, timeOutPlayerId);


                // Debug 功能，输出桌子上所有的玩家
                StringBuilder stringBuilder = new StringBuilder(string.Format("桌子 {0} 上的玩家有：", table.Id));
                table.Players.ForEach(p => stringBuilder.Append(p.Peer.PlayerId + ", "));
                log.Debug(stringBuilder.ToString());

                return;
            }

            // 准备事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.LOCK_STEP_PLAYER_OUT_OF_TIME,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 超时玩家的 ID
                    { (byte)EventParamaterKey.TIME_OUT_PLAYER_ID, timeOutPlayer.Peer.PlayerId },
                },
            };

            // 给桌子上的所有玩家转发消息
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });

            // 这个玩家改为已经从游戏中移除
            timeOutPlayer.IsRemoved = true;
        }

        /// <summary>
        /// 同步玩家放弃游戏操作
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="tableId"></param>
        /// <param name="sendParameters"></param>
        public static void LockStepPlayerGiveUp(CubirdClientPeer clientPeer, int tableId, SendParameters sendParameters)
        {
            log.InfoFormat("客户端({0})在 {1} 桌放弃游戏", clientPeer.PlayerId, tableId);

            // 获取桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 没有桌子或者还没开局，不转发
            if (table == null || !table.Playing)
            {
                log.WarnFormat("桌子 {0} 还没开局", table.Id);
                return;
            }

            // 找出这个玩家，同时这个玩家不能是已被移除的状态
            PlayerInfo playerInfo = table.Players.Find(p => p.Peer.PlayerId == clientPeer.PlayerId && !p.IsRemoved);

            // 如果这个桌子上没有发出请求的玩家，不转发
            if (playerInfo == null)
            {
                log.WarnFormat("桌子 {0} 上没有发出请求的玩家 {1}", table.Id, clientPeer.PlayerId);
                return;
            }

            // 准备事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.LOCK_STEP_PLAYER_GIVE_UP,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 玩家 ID
                    { (byte)EventParamaterKey.PLAYER_ID, clientPeer.PlayerId },
                },
            };

            // 给桌子上的所有玩家转发消息
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });

            // 这个玩家改为已经从游戏中移除
            playerInfo.IsRemoved = true;
        }
    }
}
