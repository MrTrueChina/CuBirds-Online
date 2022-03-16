using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 玩家信息的数据传输对象
    /// </summary>
    public class PlayerInfoDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 这个玩家的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 这个玩家是否已经被移除出游戏
        /// </summary>
        public bool IsRemoved { get; set; }

        /// <summary>
        /// 预留的空构造
        /// </summary>
        public PlayerInfoDTO() { }

        /// <summary>
        /// 使用 <see cref="ToObjectArray"/> 方法转化出的对象数组的构造
        /// </summary>
        /// <param name="objectArray"></param>
        public PlayerInfoDTO(object[] objectArray)
        {
            Id = (int)objectArray[0];
            Name = (string)objectArray[1];
            IsRemoved = (bool)objectArray[2];
        }

        /// <summary>
        /// 转为 Photon 可以转发的对象数组
        /// </summary>
        /// <returns></returns>
        public object[] ToObjectArray()
        {
            return new object[] { Id, Name, IsRemoved };
        }
    }
}
