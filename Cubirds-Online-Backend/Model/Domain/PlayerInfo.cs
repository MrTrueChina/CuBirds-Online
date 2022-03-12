using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Backend
{
    /// <summary>
    /// 记录玩家信息的类
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// 玩家的客户端
        /// </summary>
        public CubirdClientPeer Peer { get; set; }

        /// <summary>
        /// 这个玩家是否已经被移除出游戏
        /// </summary>
        public bool IsRemoved { get; set; } = false;
    }
}
