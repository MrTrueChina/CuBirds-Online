﻿using System;
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
        /// 获取所有没开局的桌子的信息
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.GET_ALL_TABLES_INFOS)]
        public static OperationResponse GetAllNotPlayingTablesInfos(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            log.InfoFormat("客户端({0})获取所有没开局的桌子的信息", clientPeer.PlayerId);

            // 转给 Service 处理
            List<Table> tables = MatchService.GetAllNotPlayingTablesInfos();

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
            // 获取参数
            string tableName = operationRequest.Parameters.Get<string>(RequestParamaterKey.TABLE_NAME);
            string tablePassword = operationRequest.Parameters.Get<string>(RequestParamaterKey.TABLE_PASSWORD);

            log.InfoFormat("客户端({0})开新桌子 {1}", clientPeer.PlayerId, tableName);

            // 交给 Service 处理
            Table newTable = MatchService.CreateTable(sendParameters, clientPeer, tableName, tablePassword);

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

            // 如果没找到桌子，说明桌子解散了或者根本没开出来
            if(table == null)
            {
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                        // 加入失败信息
                        { (byte)ResponseParamaterKey.SUCCESS, false },
                        // 提示文本
                        { (byte)ResponseParamaterKey.ERROR_MESSAGE_STRING, "桌子已解散" },
                    },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }
            
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

        /// <summary>
        /// 解散桌子
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.DISBAND_TABLE)]
        public static OperationResponse DisbandTable(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取桌子 ID
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})解散桌子 {1}", clientPeer.PlayerId, tableId);

            // 获取桌子
            Table table = MatchService.GetTableById(tableId);

            // 如果这个玩家不是桌主则不处理
            if (table.Master.Peer.PlayerId != clientPeer.PlayerId)
            {
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                    // 返回解散失败
                    { (byte)ResponseParamaterKey.SUCCESS, false }
                },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }

            // 交给 Service 处理
            MatchService.DisbandTable(sendParameters, clientPeer, table);

            return new OperationResponse()
            {
                Parameters = new Dictionary<byte, object>() {
                    // 返回解散成功
                    { (byte)ResponseParamaterKey.SUCCESS, true }
                },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.START_GAME)]
        public static OperationResponse StartGame(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取桌子 ID
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})在桌子 {1} 开局", clientPeer.PlayerId, tableId);

            // 获取桌子
            Table table = MatchService.GetTableById(tableId);

            // 如果这个玩家不是桌主或桌上人数不够开局则不处理
            if (table.Master.Peer.PlayerId != clientPeer.PlayerId || table.Players.Count < 2)
            {
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                    // 返回开局失败
                    { (byte)ResponseParamaterKey.SUCCESS, false }
                },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }

            // 交给 Service 处理
            MatchService.StartGame(sendParameters, clientPeer, table);

            return new OperationResponse()
            {
                Parameters = new Dictionary<byte, object>() {
                    // 返回开局成功
                    { (byte)ResponseParamaterKey.SUCCESS, true }
                },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.GAME_END)]
        public static OperationResponse GameEnd(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取桌子 ID
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})在桌子 {1} 同步游戏结束事件", clientPeer.PlayerId, tableId);

            // 获取桌子
            Table table = MatchService.GetTableById(tableId);

            // 如果没找到桌子则不处理，此时桌子可能已经在其他玩家的请求下关闭了
            if(table == null)
            {
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                    // 返回结束失败
                    { (byte)ResponseParamaterKey.SUCCESS, false }
                },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }

            // 如果这个玩家不是桌子上的玩家则不处理
            if (!table.Players.Any(p=>p.Peer.PlayerId == clientPeer.PlayerId))
            {
                return new OperationResponse()
                {
                    Parameters = new Dictionary<byte, object>() {
                    // 返回结束失败
                    { (byte)ResponseParamaterKey.SUCCESS, false }
                },
                    // 设为请求成功
                    ReturnCode = (short)ReturnCode.OK
                };
            }

            // 交给 Service 处理
            MatchService.GameEnd(clientPeer, table);

            return new OperationResponse()
            {
                Parameters = new Dictionary<byte, object>() {
                    // 返回结束成功
                    { (byte)ResponseParamaterKey.SUCCESS, true }
                },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }
    }
}
