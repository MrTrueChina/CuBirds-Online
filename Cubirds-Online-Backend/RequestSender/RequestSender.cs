using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Logging;
using Photon.SocketServer;
using CubirdsOnline.Common;
using CubirdsOnline.Backend.Service;

namespace CubirdsOnline.Backend.RequestSender
{
    /// <summary>
    /// 分发请求的类
    /// </summary>
    public static class RequestSender
    {
        /// <summary>
        /// log 对象
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 从操作码到对应的处理方法的映射
        /// </summary>
        private static Dictionary<byte, MethodInfo> operationCodeToMethod;

        /// <summary>
        /// 静态代码块
        /// </summary>
        static RequestSender()
        {
            // 创建映射字典
            operationCodeToMethod = new Dictionary<byte, MethodInfo>();

            // 获取所有有 Service 注解的类里的接收消息的方法
            List<MethodInfo> serviceMethod =
                // 获取用户的 Service 类所处的程序集
                Assembly.GetAssembly(typeof(LockstepService))
                // 获取程序集里的所有类
                .GetTypes()
                // 筛选出有 Service 注解的
                .Where(t => t.GetCustomAttribute<RequestService>() != null)
                // 获取这些类的所有公开静态方法
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                // 筛选出有接收消息注解的方法
                .Where(m => m.GetCustomAttribute<RequestHandler>() != null)
                // 转为列表
                .ToList();

            // 放入字典
            serviceMethod.ForEach(handler =>
            {
                // 获取操作码
                RequestCode code = handler.GetCustomAttribute<RequestHandler>().OperationCode;

                if (!operationCodeToMethod.ContainsKey((byte)code))
                {
                    // 映射表里没有这个操作码，把映射加进去
                    operationCodeToMethod.Add((byte)code, handler);
                }
                else
                {
                    // 映射表里已经有这个操作码了，说明这个操作码写了两个处理方法，立刻报错
                    log.ErrorFormat("对操作码 {0} 进行映射时发生错误：这个操作码有多个处理方法，无法确定使用哪个处理方法", code);
                    throw new ArgumentException("对操作码 " + code + " 进行映射时发生错误：这个操作码有多个处理方法，无法确定使用哪个处理方法");
                }
            });

            log.Info("请求映射表处理完毕");
        }

        /// <summary>
        /// 加载方法，本身没有什么作用，这个类的设计是加载时生成映射，可以在开机时调用这个类立即查看生成映射是否成功
        /// </summary>
        public static void Load()
        {
            log.Info("加载请求转发类");
        }

        /// <summary>
        /// 转发请求
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        public static void Send(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            if (operationCodeToMethod.ContainsKey(operationRequest.OperationCode))
            {
                // 获取处理方法
                MethodInfo handler = operationCodeToMethod[operationRequest.OperationCode];

                // 执行并尝试获得 OperationResponse 类型的返回值
                OperationResponse result = handler.Invoke(null, new object[] { operationRequest, sendParameters, clientPeer }) as OperationResponse;

                // 如果获取到 OperationResponse 类型的返回值，向前端发出返回
                if (result != null)
                {
                    clientPeer.SendOperationResponseWithOperationCodeAndId(operationRequest, result, sendParameters);
                }
            }
            else
            {
                // 映射表里没有这个操作码，立刻报错
                log.ErrorFormat("对操作进行分发时发生异常：没有提供对应操作码 {0} 的处理方法", operationRequest.OperationCode);

                // 创建返回消息
                OperationResponse response = new OperationResponse()
                {
                    // 错误消息
                    Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.ERROR_MESSAGE_STRING, "对操作进行分发时发生异常：没有提供对应操作码 " + operationRequest.OperationCode + " 的处理方法" } },
                    // 返回码设为服务器内部错误
                    ReturnCode = (short)ReturnCode.ERROR
                };

                // 发送返回信息
                clientPeer.SendOperationResponseWithOperationCodeAndId(operationRequest, response, sendParameters);
            }
        }
    }
}
