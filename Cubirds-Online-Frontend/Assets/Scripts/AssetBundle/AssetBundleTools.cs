using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源包工具类
/// </summary>
public class AssetBundleTools : MonoBehaviour
{
    /// <summary>
    /// 实例
    /// </summary>
    public static AssetBundleTools Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (typeof(AssetBundleTools))
            {
                if(instance == null)
                {
                    GameObject instanceGameObject = new GameObject("Asset Bundle Tools");
                    
                    DontDestroyOnLoad(instanceGameObject);

                    instance = instanceGameObject.AddComponent<AssetBundleTools>();
                }

                return instance;
            }
        }
    }
    private static AssetBundleTools instance;

    /// <summary>
    /// 资源包的基础路径
    /// </summary>
    private string AssetsPath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    
    /// <summary>
    /// 资源包主包的名称
    /// </summary>
    private string MainBundleName
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return "StandaloneWindows";
#elif UNITY_WEBGL
            // WebGL 没有热更新功能，不需要加载资源包，这里就留空了
            return "";
#else
            return "";
#endif
        }
    }

    /// <summary>
    /// 已加载的资源包的名字和包的映射表
    /// </summary>
    private readonly Dictionary<string, AssetBundle> loadedAssetBundle = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public T LoadAsset<T>(string assetBundleName, string assetName) where T : Object
    {
        // 加载资源包
        LoadAssetBundle(assetBundleName);

        // 加载资源并返回
        return loadedAssetBundle[assetBundleName].LoadAsset<T>(assetName);
    }

    /// <summary>
    /// 加载指定名称的资源包，会一并加载这个资源包依赖的其他资源包
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    private AssetBundle LoadAssetBundle(string assetBundleName)
    {
        // 如果这个资源包已经加载了，直接返回加载的包
        if (loadedAssetBundle.ContainsKey(assetBundleName))
        {
            return loadedAssetBundle[assetBundleName];
        }

        // 加载主包
        AssetBundle mainBundle = LoadAssetBundleWithOutDependency(MainBundleName);

        // 加载主包中的清单数据，这个数据和名字是官方提供的
        AssetBundleManifest mainBundleManifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        // 从清单数据里读取出要加载的这个包所依赖的包，这个是多层级的递归获取
        string[] dependenciesBundlesNames = mainBundleManifest.GetAllDependencies(assetBundleName);

        // 遍历这些包名
        foreach(string dependencyBundleName in dependenciesBundlesNames)
        {
            // 依次加载进来
            LoadAssetBundleWithOutDependency(dependencyBundleName);
        }

        // 加载这个包并返回。并不必须在加载包之前加载依赖的包，加载包放在最后仅为了书写方便
        return LoadAssetBundleWithOutDependency(assetBundleName);
    }

    /// <summary>
    /// 加载指定名称的资源包，不会加载这个资源包依赖的其他资源包<br/>如果需要一并加载依赖的资源包请使用 <see cref="LoadAssetBundle(string)"/>
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    private AssetBundle LoadAssetBundleWithOutDependency(string assetBundleName)
    {
        // 如果这个资源包已经加载了，直接返回加载的包
        if (loadedAssetBundle.ContainsKey(assetBundleName))
        {
            return loadedAssetBundle[assetBundleName];
        }

        // 加载这个资源包
        AssetBundle assetBundle = AssetBundle.LoadFromFile(AssetsPath + assetBundleName);

        // 记录到已加载资源包映射表里
        loadedAssetBundle.Add(assetBundleName, assetBundle);

        // 返回
        return assetBundle;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <param name="loadedHandler">加载完成后这个方法会被调用，参数是加载出的资源</param>
    public void LoadAssetAsync<T>(string assetBundleName, string assetName, UnityAction<T> loadedHandler) where T : Object
    {
        // 交给协程进行
        StartCoroutine(LoadAssetAsyncCoroutine(assetBundleName, assetName, loadedHandler));
    }
    /// <summary>
    /// 异步加载资源的协程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <param name="loadedHandler">加载完成后这个方法会被调用，参数是加载出的资源</param>
    /// <returns></returns>
    private IEnumerator LoadAssetAsyncCoroutine<T>(string assetBundleName, string assetName, UnityAction<T> loadedHandler) where T : Object
    {
        // 异步加载资源包并等待
        yield return LoadAssetBundleAsyncCoroutine(assetBundleName);

        // 异步加载资源
        AssetBundleRequest assetRequest = loadedAssetBundle[assetBundleName].LoadAssetAsync<T>(assetName);

        // 等待资源加载
        yield return assetRequest;

        loadedHandler.Invoke(assetRequest.asset as T);
    }

    /// <summary>
    /// 异步加载资源包的协程
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    private IEnumerator LoadAssetBundleAsyncCoroutine(string assetBundleName)
    {
        // 如果这个资源包已经加载了，直接返回
        if (loadedAssetBundle.ContainsKey(assetBundleName))
        {
            yield return null;
        }
        else
        {
            // 加载这个包并等待
            yield return LoadAssetBundleWithOutDependencyAsyncCoroutine(assetBundleName);

            // 异步加载主包并等待
            yield return LoadAssetBundleWithOutDependencyAsyncCoroutine(MainBundleName);

            // 加载主包中的清单数据，这个数据和名字是官方提供的
            AssetBundleManifest mainBundleManifest = loadedAssetBundle[MainBundleName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            // 从清单数据里读取出要加载的这个包所依赖的包，这个是多层级的递归获取
            string[] dependenciesBundlesNames = mainBundleManifest.GetAllDependencies(assetBundleName);

            // 遍历这些包名
            foreach (string dependencyBundleName in dependenciesBundlesNames)
            {
                // 依次加载并等待
                yield return LoadAssetBundleWithOutDependencyAsyncCoroutine(dependencyBundleName);
            }
        }
    }

    /// <summary>
    /// 异步加载资源包的协程，不会加载依赖的其他资源包，需要一并加载其他资源包请使用 <see cref="LoadAssetBundleAsyncCoroutine"/>
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    private IEnumerator LoadAssetBundleWithOutDependencyAsyncCoroutine(string assetBundleName)
    {
        // 如果这个资源包已经加载了，直接返回
        if (loadedAssetBundle.ContainsKey(assetBundleName))
        {
            yield return null;
        }
        else
        {
            // 异步加载资源包
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(AssetsPath + assetBundleName);

            // 等待这个资源包加载完毕
            yield return assetBundleCreateRequest;

            // 纪录进映射表里
            loadedAssetBundle.Add(assetBundleName, assetBundleCreateRequest.assetBundle);
        }
    }

    /// <summary>
    /// 卸载指定名称的资源包
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="unloadAllLoadedUbjects">是否一并卸载掉正在使用的所有已加载资源</param>
    public void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedUbjects = false)
    {
        // 如果这个包不在已加载映射表里，说明这个包并没有加载，不进行操作
        if (!loadedAssetBundle.ContainsKey(assetBundleName))
        {
            return;
        }

        // 卸载这个包
        loadedAssetBundle[assetBundleName].Unload(unloadAllLoadedUbjects);

        // 从映射表里移除这个包的映射
        loadedAssetBundle.Remove(assetBundleName);
    }

    /// <summary>
    /// 卸载所有资源包
    /// <param name="unloadAllLoadedUbjects">是否一并卸载掉正在使用的所有已加载资源</param>
    /// </summary>
    public void UnloadAllAssetBundle(bool unloadAllLoadedUbjects = false)
    {
        // 卸载所有加载的资源包
        AssetBundle.UnloadAllAssetBundles(unloadAllLoadedUbjects);

        // 清空已加载资源包的映射表
        loadedAssetBundle.Clear();
    }
}
