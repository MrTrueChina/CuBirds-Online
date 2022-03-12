using System;

namespace CubirdsOnline.Backend.RequestSender
{
    /// <summary>
    /// 表示一个类是需要接收请求的注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequestService : Attribute
    {

    }
}
