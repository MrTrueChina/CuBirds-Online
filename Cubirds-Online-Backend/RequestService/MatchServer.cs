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
            log.InfoFormat("客户端({0})获取所有桌子信息", clientPeer.ConnectionId);

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

            log.InfoFormat("客户端({0})开新桌子 {1}", clientPeer.ConnectionId, tableName);

            // 获取桌子 ID
            int tableId = ServerModel.Instance.TableIdCounter++;

            // 创建桌子
            Table newTable = new Table(tableId, tableName, clientPeer);

            // 把桌子加入到列表里
            ServerModel.Instance.Tables.Add(newTable);

            log.InfoFormat("新桌子 {0} {1} 开桌完毕，现有 {2} 个桌子", newTable.Name, newTable.Id, ServerModel.Instance.Tables.Count);

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
    }
}
