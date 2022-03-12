using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Logging;

namespace CubirdsOnline.Backend
{
    /// <summary>
    /// 服务器的数据存储类
    /// </summary>
    public class ServerModel
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 实例
        /// </summary>
        public static ServerModel Instance { get; private set; }

        /// <summary>
        /// 所有已连接且没有断开连接的客户端
        /// </summary>
        public readonly List<CubirdClientPeer> ConnectingPeers = new List<CubirdClientPeer>();

        // 静态代码块，加载类时自动执行
        static ServerModel()
        {
            // 生成并保存单例
            Instance = new ServerModel();
        }
    }
}
