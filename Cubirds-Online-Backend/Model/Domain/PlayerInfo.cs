using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubirdsOnline.Common;

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

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="peer"></param>
        public PlayerInfo(CubirdClientPeer peer)
        {
            // 保存玩家客户端
            Peer = peer;

            // 初始化为没有被移除
            IsRemoved = false;
        }

        /// <summary>
        /// 转为数据传输对象
        /// </summary>
        /// <returns></returns>
        public PlayerInfoDTO ToDTO()
        {
            return new PlayerInfoDTO()
            {
                Id = Peer.PlayerId,
                Name = Peer.Name,
                IsRemoved = IsRemoved,
            };
        }
    }
}
