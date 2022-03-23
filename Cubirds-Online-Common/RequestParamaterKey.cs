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
        /// <summary>
        /// 玩家名称
        /// </summary>
        PLAYER_NAME = 4,
        /// <summary>
        /// 卡牌种类
        /// </summary>
        CARD_TYPE = 5,
        /// <summary>
        /// 中央行索引
        /// </summary>
        CENTER_LINE_INDEX = 6,
        /// <summary>
        /// 是否放到中央行的左边
        /// </summary>
        PUT_ON_CENTER_LINE_LEFT = 7,
        /// <summary>
        /// 超时的玩家的 ID
        /// </summary>
        TIME_OUT_PLAYER_ID = 8,
        /// <summary>
        /// 桌子密码
        /// </summary>
        TABLE_PASSWORD = 9,
    }
}
