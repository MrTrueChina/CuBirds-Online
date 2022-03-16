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
    public class PlayerService
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取玩家 ID
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        public static int GetPlayerId(CubirdClientPeer clientPeer)
        {
            log.InfoFormat("客户端({0})获取 ID", clientPeer.ConnectionId);

            // 直接返回客户端记录的玩家 id
            return clientPeer.PlayerId;
        }
    }
}
