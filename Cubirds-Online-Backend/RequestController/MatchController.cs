using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubirdsOnline.Backend.RequestSender;
using CubirdsOnline.Backend.Service;
using CubirdsOnline.Common;
using ExitGames.Logging;
using Photon.SocketServer;

namespace CubirdsOnline.Backend.Controller
{
    /// <summary>
    /// 匹配功能相关的请求的 Controller
    /// </summary>
    [RequestController]
    public class MatchController
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

            // 转给 Service 处理
            List<Table> tables = MatchService.GetAllTablesInfos();

            // 返回
            return new OperationResponse()
            {
                // 把桌子信息转为 DTO 返回
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.TABLES_INFOS, tables.Select(t => t.ToDTO().ToObjectArray()).ToList().ToArray() } },
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

            // 交给 Service 处理
            Table newTable = MatchService.CreateTable(sendParameters, clientPeer, tableName);

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
            Table table = MatchService.GetTableById(tableId);
            
            // 如果桌子上已经有五个玩家了，则桌子已经满员，不能再加入
            if (table.Players.Count >= 5)
            {
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

            // 如果桌子上已经开局，不能加入
            if (table.Playing)
            {
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

            // 交给 Service 处理
            MatchService.JoinTable(sendParameters, clientPeer, table);

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

            // 从 Service 获取桌子上所有玩家的信息
            List<PlayerInfo> players = MatchService.GetAllPlayerOnTable(tableId);

            // 返回
            return new OperationResponse()
            {
                Parameters = new Dictionary<byte, object>() {
                    // 把玩家信息转为 DTO 数组返回
                    { (byte)ResponseParamaterKey.PLAYERS_INFOS, players.Select(p=>p.ToDTO().ToObjectArray()).ToArray() }
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

            // 获取桌子
            Table table = MatchService.GetTableById(tableId);

            // 交给 Service 处理
            MatchService.QuitTable(sendParameters, clientPeer, table);

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
