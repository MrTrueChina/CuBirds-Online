using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 服务器向客户端发出返回时的参数的 key
    /// </summary>
    public enum ResponseParamaterKey : byte
    {
        /// <summary>
        /// 操作 ID
        /// </summary>
        OPERATION_ID = 0,
        /// <summary>
        /// <see cref="string"/> 型的错误消息的返回参数的索引
        /// </summary>
        ERROR_MESSAGE_STRING = 1,
    }
}