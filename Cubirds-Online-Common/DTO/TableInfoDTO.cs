using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 桌子信息的数据传输对象
    /// </summary>
    public class TableInfoDTO
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
        /// 这一桌的玩家 ID
        /// </summary>
        public int[] PlayerIds { get; set; }

        /// <summary>
        /// 这一桌的桌主的 ID
        /// </summary>
        public int MasterId { get; set; }

        /// <summary>
        /// 预留的空构造
        /// </summary>
        public TableInfoDTO() { }

        /// <summary>
        /// 使用 <see cref="ToObjectArray"/> 方法转化出的对象数组的构造
        /// </summary>
        /// <param name="objectArray"></param>
        public TableInfoDTO(object[] objectArray)
        {
            Id = (int)objectArray[0];
            Name = (string)objectArray[1];
            PlayerIds = (int[])objectArray[2];
            MasterId = (int)objectArray[3];
        }

        /// <summary>
        /// 转为 Photon 可以转发的对象数组
        /// </summary>
        /// <returns></returns>
        public object[] ToObjectArray()
        {
            return new object[] { Id, Name, PlayerIds, MasterId };
        }
    }
}
