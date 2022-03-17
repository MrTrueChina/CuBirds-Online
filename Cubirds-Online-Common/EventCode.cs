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
        /// <summary>
        /// 开始游戏
        /// </summary>
        START_GAME = 5,
        /// <summary>
        /// 同步玩家打出鸟牌操作
        /// </summary>
        LOCK_STEP_PLAYER_PLAY_CARDS = 6,
        /// <summary>
        /// 同步玩家组成鸟群操作
        /// </summary>
        LOCK_STEP_PLAYER_MAKE_GROUP = 7,
        /// <summary>
        /// 同步玩家不组成鸟群操作
        /// </summary>
        LOCK_STEP_PLAYER_DONT_MAKE_GROUP = 8,
        /// <summary>
        /// 同步玩家选择抽牌操作
        /// </summary>
        LOCK_STEP_PLAYER_DRAW_CARDS = 9,
        /// <summary>
        /// 同步玩家选择不抽牌操作
        /// </summary>
        LOCK_STEP_PLAYER_DONT_DRAW_CARDS = 10,
        /// <summary>
        /// 同步玩家超时操作
        /// </summary>
        LOCK_STEP_PLAYER_OUT_OF_TIME = 11,
    }
}
