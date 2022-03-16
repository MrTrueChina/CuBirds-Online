using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 服务器向客户端发出事件时的事件码
    /// </summary>
    public enum EventCode : byte
    {
        /// <summary>
        /// 有新桌子开出的事件
        /// </summary>
        TABLE_CREATED = 0,
        /// <summary>
        /// 有桌子移除
        /// </summary>
        TABLE_REMOVED = 1,
        /// <summary>
        /// 玩家加入桌子
        /// </summary>
        PLAYER_JOIN_TABLE = 2,
        /// <summary>
        /// 玩家退出桌子
        /// </summary>
        PLAYER_QUIT_TABLE = 3,
        /// <summary>
        /// 桌子解散
        /// </summary>
        DISBAND_TABLE = 4,
    }
}
