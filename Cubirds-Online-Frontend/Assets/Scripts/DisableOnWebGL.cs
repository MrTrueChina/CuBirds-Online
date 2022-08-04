using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������ص������� WebGL ƽ̨�Ͻ�������
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
