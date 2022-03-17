using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubirdsOnline.Backend.RequestSender;
using CubirdsOnline.Backend.Service;
using CubirdsOnline.Common;
using ExitGames.Logging;
using Photon.SocketServer;

namespace CubirdsOnline.Backend.Controller
{
    /// <summary>
    /// 游戏过程中进行帧同步的 Controller
    /// </summary>
    [RequestController]
    public class LockstepController
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 同步玩家打出牌操作
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.LOCK_STEP_PLAYER_PLAY_CARDS)]
        public static OperationResponse LockStepPlayerPlayCards(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取参数
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);
            int cardType = operationRequest.Parameters.Get<int>(RequestParamaterKey.CARD_TYPE);
            int lineIndex = operationRequest.Parameters.Get<int>(RequestParamaterKey.CENTER_LINE_INDEX);
            bool putOnLeft = operationRequest.Parameters.Get<bool>(RequestParamaterKey.PUT_ON_CENTER_LINE_LEFT);

            log.InfoFormat("客户端({0})在 {1} 桌打出 {2} 牌到第 {3} 行 {4}", clientPeer.PlayerId, tableId, cardType, lineIndex, putOnLeft ? "左边" : "右边");

            // 转给 Service 处理
            LockstepService.LockStepPlayerPlayCards(clientPeer, tableId, cardType, lineIndex, putOnLeft, sendParameters);

            // 返回
            return new OperationResponse()
            {
                // 操作成功
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.SUCCESS, true } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 同步玩家组成鸟群操作
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.LOCK_STEP_PLAYER_MAKE_GROUP)]
        public static OperationResponse LockStepPlayerMakeGroup(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取参数
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);
            int cardType = operationRequest.Parameters.Get<int>(RequestParamaterKey.CARD_TYPE);

            log.InfoFormat("客户端({0})在 {1} 桌打组成 {2} 类型的鸟牌", clientPeer.PlayerId, tableId, cardType);

            // 转给 Service 处理
            LockstepService.LockStepPlayerMakeGroup(clientPeer, tableId, cardType, sendParameters);

            // 返回
            return new OperationResponse()
            {
                // 操作成功
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.SUCCESS, true } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 同步玩家不组成鸟群操作
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.LOCK_STEP_PLAYER_DONT_MAKE_GROUP)]
        public static OperationResponse LockStepPlayerDontMakeGroup(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取参数
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})在 {1} 桌打选择不组成鸟群", clientPeer.PlayerId, tableId);

            // 转给 Service 处理
            LockstepService.LockStepPlayerDontMakeGroup(clientPeer, tableId, sendParameters);

            // 返回
            return new OperationResponse()
            {
                // 操作成功
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.SUCCESS, true } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 同步玩家抽卡操作
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.LOCK_STEP_PLAYER_DRAW_CARDS)]
        public static OperationResponse LockStepPlayerDrawCards(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取参数
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})在 {1} 桌打选择抽牌", clientPeer.PlayerId, tableId);

            // 转给 Service 处理
            LockstepService.LockStepPlayerDrawCards(clientPeer, tableId, sendParameters);

            // 返回
            return new OperationResponse()
            {
                // 操作成功
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.SUCCESS, true } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 同步玩家选择不抽卡操作
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.LOCK_STEP_PLAYER_DONT_DRAW_CARDS)]
        public static OperationResponse LockStepPlayerDontDrawCards(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取参数
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})在 {1} 桌打选择不抽牌", clientPeer.PlayerId, tableId);

            // 转给 Service 处理
            LockstepService.LockStepPlayerDontDrawCards(clientPeer, tableId, sendParameters);

            // 返回
            return new OperationResponse()
            {
                // 操作成功
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.SUCCESS, true } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }

        /// <summary>
        /// 同步玩家超时
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        [RequestHandler(RequestCode.LOCK_STEP_PLAYER_OUT_OF_TIME)]
        public static OperationResponse LockStepPlayerOutOfTime(OperationRequest operationRequest, SendParameters sendParameters, CubirdClientPeer clientPeer)
        {
            // 获取参数
            int tableId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);
            int timeOutPlayerId = operationRequest.Parameters.Get<int>(RequestParamaterKey.TABLE_ID);

            log.InfoFormat("客户端({0})在 {1} 桌同步玩家 {2} 超时", clientPeer.PlayerId, tableId, timeOutPlayerId);

            // 转给 Service 处理
            LockstepService.LockStepPlayerOutOfTime(clientPeer, tableId, timeOutPlayerId, sendParameters);

            // 返回
            return new OperationResponse()
            {
                // 操作成功
                Parameters = new Dictionary<byte, object>() { { (byte)ResponseParamaterKey.SUCCESS, true } },
                // 设为请求成功
                ReturnCode = (short)ReturnCode.OK
            };
        }
    }
}
