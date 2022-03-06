using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Text;

public class RandomTest
{
    /// <summary>
    /// 这个方法用于测试 Unity 的随机数生成器设置种子效果
    /// </summary>
    [Test]
    public void TestSeed()
    {
        Debug.Log("如果多次运行这个测试输出全部相同，说明 Unity 的随机数生成器可以通过设置随机种子获得固定效果");

        Random.InitState(259674);

        StringBuilder stringBuilder = new StringBuilder();

        for(int i = 0;i < 100; i++)
        {
            stringBuilder.Append(Random.Range(0, 100) + ", ");
        }

        Debug.Log(stringBuilder);
    }
}
