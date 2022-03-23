using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubirdsOnline.Common
{
    /// <summary>
    /// <see cref="Dictionary{TKey, TValue}"/> 的扩展方法
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 获取值，获取不到会返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this Dictionary<byte, object> data, EventParamaterKey key)
        {
            return data.ContainsKey((byte)key) ? (T)data[(byte)key] : default;
        }

        /// <summary>
        /// 获取值，获取不到会返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this Dictionary<byte, object> data, RequestParamaterKey key)
        {
            return data.ContainsKey((byte)key) ? (T)data[(byte)key] : default;
        }

        /// <summary>
        /// 获取值，获取不到会返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this Dictionary<byte, object> data, ResponseParamaterKey key)
        {
            return data.ContainsKey((byte)key) ? (T)data[(byte)key] : default;
        }
    }
}
