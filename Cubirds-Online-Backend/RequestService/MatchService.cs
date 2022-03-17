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
    /// 匹配功能相关的请求的 Service
    /// </summary>
    public class MatchService
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取所有桌子
        /// </summary>
        /// <returns></returns>
        public static List<Table> GetAllTablesInfos()
        {
            log.Info("获取所有桌子信息");

            // 复制一份返回
            return new List<Table>(ServerModel.Instance.Tables);
        }

        /// <summary>
        /// 获取指定 ID 的桌子
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public static Table GetTableById(int tableId)
        {
            log.DebugFormat("获取 ID 为 {0} 的桌子", tableId);

            return ServerModel.Instance.Tables.Find(t => t.Id == tableId);
        }

        /// <summary>
        /// 新开一个桌子
        /// </summary>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Table CreateTable(SendParameters sendParameters, CubirdClientPeer clientPeer, string tableName)
        {
            log.InfoFormat("客户端({0})开新桌子 {1}", clientPeer.PlayerId, tableName);

            // TODO：这里需要修改，桌子 ID 应该是可以重复使用的，不然前端显示不开
            // 获取桌子 ID
            int tableId = ServerModel.Instance.TableIdCounter++;

            // 创建桌子
            Table newTable = new Table(tableId, tableName, clientPeer);

            // 把桌子加入到列表里
            ServerModel.Instance.Tables.Add(newTable);

            log.InfoFormat("新桌子 {0} {1} 开桌完毕，现有 {2} 个桌子", newTable.Name, newTable.Id, ServerModel.Instance.Tables.Count);

            // 准备有新桌子开出来的事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.TABLE_CREATED,
                // 参数
                Parameters = new Dictionary<byte, object>()
                {
                    // 新桌子的信息
                    { (byte)EventParamaterKey.TABLE_INFO, newTable.ToDTO().ToObjectArray() },
                },
            };
            // TODO：考虑到网络性能，这里最好改为只给不在桌子上的玩家发送
            // 给所有玩家发送这个消息
            ServerModel.Instance.ConnectingPeers.ForEach(p => p.SendEvent(eventData, sendParameters));

            // 返回
            return newTable;
        }

        /// <summary>
        /// 加入桌子
        /// </summary>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <param name="table"></param>
        /// <returns>如果玩家成功加入桌子则返回 true，否则返回 false</returns>
        public static bool JoinTable(SendParameters sendParameters, CubirdClientPeer clientPeer, Table table)
        {
            log.InfoFormat("客户端({0})加入桌子 {1}", clientPeer.PlayerId, table.Id);

            // 如果桌子已经有五个人或者已经开局了，玩家不能加入这个桌子
            if(table.Players.Count >= 5 || table.Playing)
            {
                return false;
            }

            // 包装一个玩家信息
            PlayerInfo playerInfo = new PlayerInfo(clientPeer);

            // 准备玩家加入桌子事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.PLAYER_JOIN_TABLE,
                // 参数
                Parameters = new Dictionary<byte, object>() {
                    // 玩家信息
                    { (byte)EventParamaterKey.PLAYER_INFO, playerInfo.ToDTO().ToObjectArray() },
                },
            };
            // 给桌子上现有的玩家发出这个玩家加入桌子的事件
            table.Players.ForEach(p => p.Peer.SendEvent(eventData, sendParameters));

            // 把玩家添加进桌子
            table.Players.Add(playerInfo);

            // 返回
            return true;
        }

        /// <summary>
        /// 获取一个桌子上所有的玩家的信息
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public static List<PlayerInfo> GetAllPlayerOnTable(int tableId)
        {
            log.InfoFormat("获取桌子 {0} 的玩家", tableId);

            // 找到桌子
            Table table = GetTableById(tableId);

            // 返回
            return table.Players;
        }

        /// <summary>
        /// 退出桌子
        /// </summary>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static void QuitTable(SendParameters sendParameters, CubirdClientPeer clientPeer, Table table)
        {
            log.InfoFormat("客户端({0})退出桌子 {1}", clientPeer.PlayerId, table.Id);

            // 找到这个玩家
            PlayerInfo quitPlayer = table.Players.Find(p => p.Peer.PlayerId == clientPeer.PlayerId);

            // 桌子上有这个玩家才处理
            if (quitPlayer != null)
            {
                // 准备事件
                EventData eventData = new EventData()
                {
                    // 事件码
                    Code = (byte)EventCode.PLAYER_QUIT_TABLE,
                    // 参数
                    Parameters = new Dictionary<byte, object>()
                    {
                        // 玩家 ID
                        { (byte)EventParamaterKey.PLAYER_ID, clientPeer.PlayerId },
                    }
                };

                // 把消息转发给桌子上的所有玩家
                table.Players.ForEach(p =>
                {
                    log.DebugFormat("转发玩家 {0} 退出桌子的消息给玩家 {1}", quitPlayer.Peer.PlayerId, p.Peer.PlayerId);
                    p.Peer.SendEvent(eventData, sendParameters);
                });

                // 从桌子上移除这个玩家
                table.Players.Remove(quitPlayer);
            }
        }

        /// <summary>
        /// 解散桌子
        /// </summary>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static void DisbandTable(SendParameters sendParameters, CubirdClientPeer clientPeer, Table table)
        {
            log.InfoFormat("客户端({0})解散桌子 {1}", clientPeer.PlayerId, table.Id);

            // 如果这个玩家不是桌主则不解散桌子
            if(table.Master.Peer.PlayerId != clientPeer.PlayerId)
            {
                return;
            }

            // 准备事件
            EventData eventData = new EventData()
            {
                // 事件码
                Code = (byte)EventCode.DISBAND_TABLE,
                // 解散桌子不需要参数
                Parameters = new Dictionary<byte, object>() { },
            };

            // 把消息转发给桌子上的所有玩家
            table.Players.ForEach(p =>
            {
                p.Peer.SendEvent(eventData, sendParameters);
            });

            // 从列表中移除桌子
            ServerModel.Instance.Tables.Remove(table);
        }
    }
}
