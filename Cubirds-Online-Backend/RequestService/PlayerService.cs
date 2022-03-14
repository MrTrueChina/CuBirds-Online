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
    /// 游戏主流程之外的对玩家相关的请求进行处理的 Service
    /// </summary>
    [RequestService]
    public class PlayerService
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取玩家 ID
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.GET_PLAYER_ID)]
        public static OperationResponse GetPlayerId(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            log.InfoFormat("客户端({0})获取 ID", clientPeer.ConnectionId);

            // 返回
            return new OperationResponse()
            {
                // 返回客户端的连接 ID
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.PLAYER_ID, clientPeer.PlayerId } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }
    }
}
