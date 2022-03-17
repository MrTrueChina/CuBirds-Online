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
        /// <summary>
        /// 设置玩家名字
        /// </summary>
        SET_PLAYER_NAME = 7,
        /// <summary>
        /// 开始游戏
        /// </summary>
        START_GAME = 8,
        /// <summary>
        /// 同步玩家打出鸟牌操作
        /// </summary>
        LOCK_STEP_PLAYER_PLAY_CARDS = 9,
        /// <summary>
        /// 同步玩家组成鸟群操作
        /// </summary>
        LOCK_STEP_PLAYER_MAKE_GROUP = 10,
        /// <summary>
        /// 同步玩家不组成鸟群操作
        /// </summary>
        LOCK_STEP_PLAYER_DONT_MAKE_GROUP = 11,
        /// <summary>
        /// 同步玩家选择抽牌操作
        /// </summary>
        LOCK_STEP_PLAYER_DRAW_CARDS = 12,
        /// <summary>
        /// 同步玩家选择不抽牌操作
        /// </summary>
        LOCK_STEP_PLAYER_DONT_DRAW_CARDS = 13,
        /// <summary>
        /// 同步玩家超时操作
        /// </summary>
        LOCK_STEP_PLAYER_OUT_OF_TIME = 14,
    }
}
