using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubirdsOnline.Common;

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
        /// 这个桌子的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 这一桌已经开局了
        /// </summary>
        public bool Playing { get; set; } = false;

        /// <summary>
        /// 这一桌的玩家
        /// </summary>
        public List<PlayerInfo> Players { get; } = new List<PlayerInfo>();

        /// <summary>
        /// 这一桌的桌主
        /// </summary>
        public PlayerInfo Master { get; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="masterPeer"></param>
        public Table(int id, string name, CubirdClientPeer masterPeer)
        {
            // 保存数据
            Id = id;
            Name = name;

            // 新开的桌子当然没有开局
            Playing = false;

            // 把玩家客户端转为玩家信息对象
            PlayerInfo masterPlayerInfo = new PlayerInfo(masterPeer);

            // 开桌子的玩家就是桌主
            Master = masterPlayerInfo;

            // 把开桌子的玩家加入桌子
            Players.Add(masterPlayerInfo);
        }

        /// <summary>
        /// 转为 <see cref="TableInfoDTO"/>
        /// </summary>
        /// <returns></returns>
        public TableInfoDTO ToDTO()
        {
            return new TableInfoDTO()
            {
                Id = Id,
                Name = Name,
                Playing = Playing,
                PlayerIds = Players.Select(p => p.Peer.PlayerId).ToList().ToArray(),
                MasterId = Master.Peer.PlayerId,
            };
        }
    }
}
