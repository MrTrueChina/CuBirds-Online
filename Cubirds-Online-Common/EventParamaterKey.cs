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
        PLAYER_ID = 0,
        /// <summary>
        /// 玩家信息
        /// </summary>
        PLAYER_INFO = 1,
        /// <summary>
        /// 桌子 ID
        /// </summary>
        TABLE_ID = 2,
        /// <summary>
        /// 桌子信息
        /// </summary>
        TABLE_INFO = 3,
        /// <summary>
        /// 玩家信息列表
        /// </summary>
        PLAYERS_INFOS = 4,
        /// <summary>
        /// 随机数种子
        /// </summary>
        RANDOM_SEED = 5,
        /// <summary>
        /// 卡牌种类
        /// </summary>
        CARD_TYPE = 6,
        /// <summary>
        /// 中央行索引
        /// </summary>
        CENTER_LINE_INDEX = 7,
        /// <summary>
        /// 是否放到中央行的左边
        /// </summary>
        PUT_ON_CENTER_LINE_LEFT = 8,
        /// <summary>
        /// 超时的玩家的 ID
        /// </summary>
        TIME_OUT_PLAYER_ID = 9,
    }
}
