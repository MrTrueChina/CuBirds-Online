using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 服务器发出的事件的参数的 key
    /// </summary>
    public enum EventParamaterKey : byte
    {
        /// <summary>
        /// 玩家 ID
        /// </summary>
        PLAYER_ID,
        /// <summary>
        /// 玩家信息
        /// </summary>
        PLAYER_INFO,
        /// <summary>
        /// 桌子 ID
        /// </summary>
        TABLE_ID,
        /// <summary>
        /// 桌子信息
        /// </summary>
        TABLE_INFO,
    }
}
