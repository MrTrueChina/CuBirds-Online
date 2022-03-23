using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// 操作返回码
    /// </summary>
    public enum ReturnCode : short
    {
        /// <summary>
        /// 操作正常完成
        /// </summary>
        OK = 200,
        /// <summary>
        /// 请求错误，例如请求头错误、参数错误
        /// </summary>
        REQUEST_WRONG = 400,
        /// <summary>
        /// 服务器内部错误
        /// </summary>
        ERROR = 500
    }
}
