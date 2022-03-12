using System.Collections.Generic;
using ExitGames.Logging;
using CubirdsOnline.Common;
using Photon.SocketServer;
using LogManager = ExitGames.Logging.LogManager;

namespace CubirdsOnline.Backend
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class CubirdClientPeer : ClientPeer
    {
        // 获取当前类的 log 实例，这里没有再进行设置，可能是设置只需要进行一次，在主类里进行过了客户端就不要再设置了
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 这个客户端的玩家 ID
        /// </summary>
        public int playerId = -1;

        public CubirdClientPeer(InitRequest initRequest) : base(initRequest)
        {
            log.InfoFormat("客户端建立连接，ConnectionId = {0}", ConnectionId);

            // 添加到连接着的客户端列表中
            ServerModel.Instance.ConnectingPeers.Add(this);

            // 将连接 ID 作为玩家 ID 保存
            playerId = ConnectionId;
        }

        /// <summary>
        /// 接收到这个客户端发送的请求时这个方法会被调用
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            log.DebugFormat("客户端({0})收到请求，OperationCode = {1}，Parameters = {2}", ConnectionId, operationRequest.OperationCode, operationRequest.Parameters);

            // 如果请求没有携带操作 ID，忽略该请求
            if (!operationRequest.Parameters.ContainsKey((byte)RequestParamaterKey.OPERATION_ID))
            {
                log.DebugFormat("客户端({0})收到缺少操作 ID 的请求，该请求将被忽略，请检查前端是否有没有添加操作 ID 发出请求的代码，OperationCode = {1}，Parameters = {2}", ConnectionId, operationRequest.OperationCode, operationRequest.Parameters);
                return;
            }

            // 让分发请求的类分发请求
            RequestSender.RequestSender.Send(operationRequest, sendParameters, this);
        }

        /// <summary>
        /// 这个客户端断开连接时这个方法会被调用
        /// </summary>
        /// <param name="reasonCode"></param>
        /// <param name="reasonDetail"></param>
        protected override void OnDisconnect(int reasonCode, string reasonDetail)
        {
            log.InfoFormat("客户端断开连接，ConnectionId = {0}", ConnectionId);

            // 从连接着的客户端列表中移除
            ServerModel.Instance.ConnectingPeers.Remove(this);
        }

        /// <summary>
        /// 根据前端发送的请求向前端发出返回，自动添加操作码和操作 ID<br/>
        /// 【该方法取代 SendOperationResponse 方法，请不要使用 SendOperationResponse 方法向前端发送返回】
        /// </summary>
        /// <param name="operationRequest">前端发送的请求</param>
        /// <param name="operationResponse">要发送的返回</param>
        /// <param name="sendParameters">发送参数</param>
        public void SendOperationResponseWithOperationCodeAndId(OperationRequest operationRequest, OperationResponse operationResponse, SendParameters sendParameters)
        {
            log.DebugFormat("向客户端({0})发出返回，OperationCode = {1}，Parameters = {2}", ConnectionId, operationRequest.OperationCode, operationResponse.Parameters);

            // 设置返回的操作码为请求的操作码
            operationResponse.OperationCode = operationRequest.OperationCode;

            // 将请求的操作 ID 赋值给返回
            if (operationResponse.Parameters.ContainsKey((byte)ResponseParamaterKey.OPERATION_ID))
            {
                log.WarnFormat("客户端({0})发现一个请求的返回有不应有的操作 ID，请检查是否有代码在返回参数中对 Key 为 {1} 的位置赋值", ConnectionId, (byte)ResponseParamaterKey.OPERATION_ID);

                operationResponse.Parameters[(byte)ResponseParamaterKey.OPERATION_ID] = operationRequest.Parameters[(byte)RequestParamaterKey.OPERATION_ID];
            }
            else
            {
                operationResponse.Parameters.Add((byte)ResponseParamaterKey.OPERATION_ID, operationRequest.Parameters[(byte)RequestParamaterKey.OPERATION_ID]);
            }

            // 发送返回信息，第二个参数就是这个方法的 sendParameters 参数
            SendOperationResponse(operationResponse, sendParameters);
        }
    }
}
