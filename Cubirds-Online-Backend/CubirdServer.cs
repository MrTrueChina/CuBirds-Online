﻿using System.Collections.Generic;
using System.IO;
using CubirdsOnline.Backend.Service;
using CubirdsOnline.Common;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Photon.SocketServer;
using LogManager = ExitGames.Logging.LogManager;

namespace CubirdsOnline.Backend
{
    /// <summary>
    /// 后台服务器
    /// </summary>
    public class CubirdServer : ApplicationBase
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static CubirdServer ServerInstance { get; private set; }

        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Photon5 版本需要的带配置的构造
        /// </summary>
        public CubirdServer() : base(LoadConfiguration()) { }

        /// <summary>
        /// 加载 <see cref="ApplicationBase"/> 需要的配置
        /// </summary>
        /// <returns></returns>
        private static IConfiguration LoadConfiguration()
        {
            // 配置构建器
            ConfigurationBuilder cb = new ConfigurationBuilder();

            // 获取这个文件的位置，在编译后这个代码会编译到一个 dll 文件中，运行时就是获取编译出的那个 dll 文件的位置
            string cbpath = Path.GetDirectoryName(typeof(CubirdServer).Assembly.CodeBase).Remove(0, 6);

            // 拼接 dll 位置和配置文件名称，拼接出来的就是配置文件的准确位置，用配置构建器加载配置，之后构建出配置并返回
            return cb.AddXmlFile(Path.Combine(cbpath, "CubirdServer.xml.config")).Build();
        }

        /// <summary>
        /// 服务器初始化时调用这个方法
        /// </summary>
        protected override void Setup()
        {
            // 初始化 log
            InitLogging();

            // log 没有初始化调 log 也没用
            log.Info("log 完成初始化，开始记录 log");

            //log.Info("开始注册自定义序列化和反序列化");
            //Protocol.TryRegisterCustomType(typeof(TableInfoDTO), myCustomTypeCode, TableInfoDTO.Serialize, TableInfoDTO.Deserialize);
            //log.Info("自定义序列化和反序列化注册完毕");

            // 加载请求转发类，非必须，但可以在开机的时候就看到映射是不是正常
            log.Info("初始化请求转发功能");
            RequestSender.RequestSender.Load();
            log.Info("请求转发初始化完成");

            // 保存实例
            ServerInstance = this;

            log.Info("主类初始化完毕");
        }

        /// <summary>
        /// 初始化 log
        /// </summary>
        private void InitLogging()
        {
            // 通知 Photon 使用 Log4Net 的实例作为日志源
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);

            // 设置 log 文件的文件夹位置
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationRootPath, "log");

            // 设置 log 文件名称
            GlobalContext.Properties["LogFileName"] = ApplicationName;

            // 读取配置
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(BinaryPath, "log4net.config")));
        }

        /// <summary>
        /// 有客户端进行连接时调用这个方法创建客户端实例
        /// </summary>
        /// <param name="initRequest"></param>
        /// <returns></returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            log.InfoFormat("创建客户端");

            return new CubirdClientPeer(initRequest);
        }

        /// <summary>
        /// 停机时这个方法会被调用
        /// </summary>
        protected override void TearDown()
        {
            log.InfoFormat("拆毁");
        }

        /// <summary>
        /// 当客户端断开连接时这个方法会被调用
        /// </summary>
        /// <param name="peer"></param>
        public void OnPeerDisconnect(CubirdClientPeer peer)
        {
            // 从连接着的客户端列表中移除这个客户端
            ServerModel.Instance.ConnectingPeers.Remove(peer);

            // 遍历所有桌子
            ServerModel.Instance.Tables.ForEach(table =>
            {
                // 如果玩家在这个桌子里，把这个玩家从桌子里移除
                if(table.Players.Find(player=>player.Peer.PlayerId == peer.PlayerId) != null)
                {
                    if(table.Master.Peer.PlayerId == peer.PlayerId)
                    {
                        // 如果这个玩家是桌主，解散桌子
                        MatchService.DisbandTable(new SendParameters(), peer, table);
                    }
                    else
                    {
                        // 如果这个玩家不是桌主，这个玩家退出桌子
                        MatchService.QuitTable(new SendParameters(), peer, table);
                    }
                }
            });
        }
    }
}
