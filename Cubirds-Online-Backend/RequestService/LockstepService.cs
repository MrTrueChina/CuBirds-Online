using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubirdsOnline.Backend.RequestSender;
using ExitGames.Logging;

namespace CubirdsOnline.Backend.Service
{
    /// <summary>
    /// 游戏过程中进行帧同步的 Service
    /// </summary>
    [RequestController]
    public class LockstepService
    {
        // 获取当前类的 log 实例
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
    }
}
