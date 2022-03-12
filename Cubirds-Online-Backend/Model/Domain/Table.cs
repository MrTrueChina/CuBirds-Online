using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Backend
{
    /// <summary>
    /// 游戏的桌子
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 桌号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 这一桌的玩家
        /// </summary>
        public List<PlayerInfo> players { get; } = new List<PlayerInfo>();

        /// <summary>
        /// 这一桌的桌主
        /// </summary>
        public PlayerInfo Master { get; }
    }
}
