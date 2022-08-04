using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个组件挂载的物体在 WebGL 平台上将被禁用
/// </summary>
public class DisableOnWebGL : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_EDITOR
#elif UNITY_WEBGL
        gameObject.SetActive(false);
#endif
    }
}
