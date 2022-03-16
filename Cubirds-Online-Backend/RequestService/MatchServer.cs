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
    [RequestService]
    public class MatchServer
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取所有桌子
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.GET_ALL_TABLES_INFOS)]
        public static OperationResponse GetAllTablesInfos(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            log.InfoFormat("客户端({0})获取所有桌子信息", clientPeer.PlayerId);

            // 返回
            return new OperationResponse()
            {
                // 把桌子信息转为 DTO 返回
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.TABLES_INFOS, ServerModel.Instance.Tables.Select(t => t.ToDTO().ToObjectArray()).ToList().ToArray() } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 新开一个桌子
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.CREATE_TABLE)]
        public static OperationResponse CreateTable(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取桌子名称
            string tableName = operationRequest.Parameters.Get<string>(RequestParamaterKey.TABLE_NAME);

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
            return new OperationResponse()
            {
                Parameters = new Dictionary<byte, object>() {
                    // 把桌子信息转为 DTO 返回
                    { (byte)ResponseParamaterKey.TABLE_INFO, newTable.ToDTO().ToObjectArray() }
                },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 加入桌子
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.JOIN_TABLE)]
        public static OperationResponse JoinTable(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取桌子 ID
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})加入桌子 {1}", clientPeer.PlayerId, tableId);

            // 找到桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            if(table.Players.Count < 5 && !table.Playing)
            {
                // 桌子还有空位而且没有开局

                // 把玩家添加进桌子
                table.Players.Add(new PlayerInfo(clientPeer));

                // 返回
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                        // 加入成功信息
                        { (byte)ResponseParamaterKey.SUCCESS, true },
                        // 桌子信息
                        { (byte)ResponseParamaterKey.TABLE_INFO, table.ToDTO().ToObjectArray() },
                    },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }
            else if(table.Players.Count >= 5)
            {
                // 桌子没有空位

                // 返回
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                        // 加入失败信息
                        { (byte)ResponseParamaterKey.SUCCESS, false },
                        // 提示文本
                        { (byte)ResponseParamaterKey.ERROR_MESSAGE_STRING, "桌子已满员" },
                    },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }
            else
            {
                // 桌子已经开局

                // 返回
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                        // 加入失败信息
                        { (byte)ResponseParamaterKey.SUCCESS, false },
                        // 提示文本
                        { (byte)ResponseParamaterKey.ERROR_MESSAGE_STRING, "桌子已开局" },
                    },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }
        }

        /// <summary>
        /// 获取一个桌子上所有的玩家的信息
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.GET_ALL_PLAYER_ON_TABLE)]
        public static OperationResponse GetAllPlayerOnTable(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取桌子 ID
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})获取桌子 {1} 的玩家", clientPeer.PlayerId, tableId);

            // 找到桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 返回
            return new OperationResponse()
            {
                Parameters = new Dictionary<byte, object>() {
                    // 把玩家信息转为 DTO 数组返回
                    { (byte)ResponseParamaterKey.PLAYERS_INFOS, table.Players.Select(p=>p.ToDTO().ToObjectArray()).ToArray() }
                },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 退出桌子
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.QUIT_TABLE)]
        public static OperationResponse QuitTable(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取桌子 ID
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})退出桌子 {1}", clientPeer.PlayerId, tableId);

            // 找到桌子
            Table table = ServerModel.Instance.Tables.Find(t => t.Id == tableId);

            // 桌子上有这个玩家才处理
            if (table.Players.Find(p => p.Peer.PlayerId == tableId) != null)
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
                    p.Peer.SendEvent(eventData, sendParameters);
                });
            }

            return new OperationResponse()
            {
                Parameters = new Dictionary<byte, object>() {
                    // 返回退出成功
                    { (byte)ResponseParamaterKey.SUCCESS, true }
                },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }
    }
}
