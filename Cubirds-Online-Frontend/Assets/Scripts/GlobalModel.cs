using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储全局通用数据的类
/// </summary>
public class GlobalModel : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static GlobalModel Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (typeof(GlobalModel))
            {
                if (instance == null)
                {
                    // 生成挂载实例的物体
                    GameObject instanceObject = new GameObject("Global Model");

                    // 挂载组件
                    instance = instanceObject.AddComponent<GlobalModel>();

                    // 设为不随场景加载销毁
                    DontDestroyOnLoad(instanceObject);
                }

                return instance;
            }
        }
    }
    private static GlobalModel instance;

    /// <summary>
    /// 本机玩家 id
    /// </summary>
    public int LocalPLayerId { get; set; }
    /// <summary>
    /// 这一桌所有玩家的 id
    /// </summary>
    public List<int> TablePlayerIds { get; set; }
    /// <summary>
    /// 开局玩家的 id
    /// </summary>
    public int StartPlayerId { get; set; }
}
