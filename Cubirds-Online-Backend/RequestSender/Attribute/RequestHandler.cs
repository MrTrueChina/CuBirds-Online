using System;
using CubirdsOnline.Common;
using Photon.SocketServer;

namespace CubirdsOnline.Backend.RequestSender
{
    /// <summary>
    /// 表示一个方法是对请求的处理方法的注解，修饰的方法必须是静态的<br/>
    /// 【注意】修饰的方法必须是静态的，动态方法不会被识别<br/>
    /// 【注意】修饰的方法必须有 <see cref="OperationRequest"/>, <see cref="SendParameters"/>, <see cref="CubirdClientPeer"/>，三个类型的参数<br/>
    /// 【注意】使用这个注解的方法如果需要对请求进行返回需要返回类型为 <see cref="OperationResponse"/> 的返回值，如果需要手动发送返回请通过引用找到发送返回的方法并与该方法进行相同的补充参数处理和其他处理<br/>
    /// 【注意】其他类型的返回值都会被视为无返回值忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestHandler : Attribute
    {
        /// <summary>
        /// 这个方法对哪个操作码的请求进行处理
        /// </summary>
        public RequestCode OperationCode { get; private set; }

        public RequestHandler(RequestCode operationCode)
        {
            OperationCode = operationCode;
        }
    }
}
