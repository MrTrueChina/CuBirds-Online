using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 客户端向服务器发出请求的操作码
    /// </summary>
    public enum RequestCode : byte
    {
        /// <summary>
        /// 获取玩家 ID
        /// </summary>
        GET_PLAYER_ID = 0,
        /// <summary>
        /// 获取所有的桌子信息
        /// </summary>
        GET_ALL_TABLES_INFOS = 1,
        /// <summary>
        /// 开新桌子
        /// </summary>
        CREATE_TABLE = 2,
        /// <summary>
        /// 加入桌子
        /// </summary>
        JOIN_TABLE = 3,
        /// <summary>
        /// 获取指定桌子上所有的玩家
        /// </summary>
        GET_ALL_PLAYER_ON_TABLE = 4,
        /// <summary>
        /// 退出桌子
        /// </summary>
        QUIT_TABLE = 5,
        /// <summary>
        /// 解散桌子
        /// </summary>
        DISBAND_TABLE = 6,
    }
}
