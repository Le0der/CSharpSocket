using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class DataConverter
    {
        /// <summary>
        /// 根据int值获取枚举
        /// </summary>
        public static TEnum GetEnumByInt<TEnum>(int value) where TEnum : struct, Enum
        {
            var type = typeof(TEnum);
            if (!Enum.IsDefined(type, value))
            {
                string errorStr = string.Format("数字转换枚举错误，枚举：{0} 未定义值：{1}", type, value);
                throw new ArgumentOutOfRangeException(errorStr);
            }
            return (TEnum)Enum.ToObject(type, value);
        }
    }
}
