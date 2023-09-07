using System;
using System.Reflection;

namespace Common
{
    public static class Tools
    {
        /// <summary>
        /// 组合数组
        /// </summary>
        /// <param name="arr1">数组一</param>
        /// <param name="arr2">数组二</param>
        /// <returns>组合后的数组</returns>
        public static byte[] ConcatBytes(byte[] arr1, byte[] arr2)
        {
            byte[] result = new byte[arr1.Length + arr2.Length];

            Array.Copy(arr1, result, arr1.Length);
            Array.Copy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        /// <summary>
        /// 根据枚举和GetType获取反射方法
        /// </summary>
        public static MethodInfo GetMethodInfo<TEnum>(TEnum value, Type type) where TEnum : struct, Enum
        {
            MethodInfo methodInfo = null;
            var methodName = Enum.GetName(typeof(TEnum), value);
            if (!string.IsNullOrEmpty(methodName))
            {
                methodInfo = type.GetMethod(methodName);
            }
            return methodInfo;
        }
    }
}
