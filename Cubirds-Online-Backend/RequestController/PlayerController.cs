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
    /// 游戏主流程之外的对玩家相关的请求进行处理的 Controller
    /// </summary>
    [RequestController]
    public class PlayerController
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

            // 交给 Service 处理
            int playerId = PlayerService.GetPlayerId(clientPeer);

            // 返回
            return new OperationResponse()
            {
                // 返回客户端的连接 ID
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.PLAYER_ID, playerId } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 设置玩家名字
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.SET_PLAYER_NAME)]
        public static OperationResponse SetPLayerName(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取参数
            string name = operationRequest.Parameters.Get<string>(RequestParamaterKey.PLAYER_NAME);

            log.InfoFormat("客户端({0})设置名称", clientPeer.ConnectionId);

            // 交给 Service 处理
            PlayerService.SetPLayerName(clientPeer, name);

            // 返回
            return new OperationResponse()
            {
                // 返回操作成功
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.SUCCESS, true } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }
    }
}
