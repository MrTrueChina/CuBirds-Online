using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 客户端向服务器发出请求时的参数的 key
    /// </summary>
    public enum RequestParamaterKey : byte
    {
        /// <summary>
        /// 操作 ID
        /// </summary>
        OPERATION_ID = 0,
        /// <summary>
        /// 桌子名
        /// </summary>
        TABLE_NAME = 1,
        /// <summary>
        /// 玩家 ID
        /// </summary>
        PLAYER_ID = 2,
        /// <summary>
        /// 桌子 ID
        /// </summary>
        TABLE_ID = 3,
    }
}
