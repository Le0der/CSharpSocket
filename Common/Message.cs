using System;
using System.Text;

namespace Common
{
    public class Message
    {
        private byte[] data = new byte[1024];                                   //接受数据缓存值
        private int totalDataCount = 0;                                         //总存储的数据长度

        public byte[] Data { get { return data; } }                             //缓存池索引器
        public int TotalCount { get { return totalDataCount; } }                //总数据强度索引器
        public int RemainSize { get { return data.Length - totalDataCount; } }  //剩余可用空间索引器

        /// <summary>
        /// 解析客户端给服务器的数据
        /// </summary>
        /// <param name="newDataAmount">新数据数量</param>
        public void ReadMessage(int newDataAmount, Action<RequestCode, ActionCode, string> processDataCallback)
        {
            //     _____________________________________________________________________
            //    | 数据长度 (4) | RequestCode (4) | ActionCode (4) | 数据              |
            totalDataCount += newDataAmount;
            while (true)
            {
                //如果数据不够解析数据长度，那么结束解析等待下次有数据再解析
                if (totalDataCount <= 4) return;

                //获取有效数据长度
                int count = BitConverter.ToInt32(data, 0);

                //如果剩余的有效数据不到完整的数据长度，那么结束解析等待下次有数据再解析
                if ((totalDataCount - 4) < count) return;


                //解析RequestCode
                int requestCodeVal = BitConverter.ToInt32(data, 4);
                RequestCode requestCode = DataConverter.GetEnumByInt<RequestCode>(requestCodeVal);

                //解析actionCode
                int actionCodeVal = BitConverter.ToInt32(data, 8);
                ActionCode actionCode = DataConverter.GetEnumByInt<ActionCode>(actionCodeVal);

                //解析有效数据
                string s = Encoding.UTF8.GetString(data, 12, count - 8);

                //执行回调
                processDataCallback.Invoke(requestCode, actionCode, s);

                //游标前移
                totalDataCount -= (count + 4);

                //删除已经解析的数据
                Array.Copy(data, count + 4, data, 0, totalDataCount);
            }
        }

        /// <summary>
        /// 解析服务端给客户端的数据
        /// </summary>
        /// <param name="newDataAmount">新数据数量</param>
        public void ReadMessage(int newDataAmount, Action<ActionCode, string> processDataCallback)
        {
            //     ___________________________________________________
            //    | 数据长度 (4) | ActionCode (4) | 数据              |
            totalDataCount += newDataAmount;
            while (true)
            {
                //如果数据不够解析数据长度，那么结束解析等待下次有数据再解析
                if (totalDataCount <= 4) return;

                //获取有效数据长度
                int count = BitConverter.ToInt32(data, 0);

                //如果剩余的有效数据不到完整的数据长度，那么结束解析等待下次有数据再解析
                if ((totalDataCount - 4) < count) return;


                //解析ActionCode
                int actionCodeVal = BitConverter.ToInt32(data, 4);
                ActionCode actionCode = DataConverter.GetEnumByInt<ActionCode>(actionCodeVal);

                //解析有效数据
                string s = Encoding.UTF8.GetString(data, 8, count - 4);

                //执行回调
                processDataCallback.Invoke(actionCode, s);

                //游标前移
                totalDataCount -= (count + 4);

                //删除已经解析的数据
                Array.Copy(data, count + 4, data, 0, totalDataCount);
            }
        }

        /// <summary>
        /// 打包发送响应信息
        /// </summary>
        /// <param name="action">请求代码</param>
        /// <param name="data">动作类型</param>
        /// <returns>用于传输的byte数组</returns>
        public static byte[] PackData(ActionCode action, string data)
        {
            //获取ActionCode的byte数组
            byte[] actionBytes = BitConverter.GetBytes((int)action);

            //获取返回信息的数组
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            //获取数据长度的byte数组
            int dataAmount = actionBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);

            //拼接信息：长度-code-数据
            byte[] result = Tools.ConcatBytes(dataAmountBytes, actionBytes);
            result = Tools.ConcatBytes(result, dataBytes);

            return result;
        }

        /// <summary>
        /// 打包发送请求信息
        /// </summary>
        /// <param name="request">请求类型</param>
        /// <param name="action">动作类型</param>
        /// <param name="data">返回数据</param>
        /// <returns>用于传输的byte数组</returns>
        public static byte[] PackData(RequestCode request, ActionCode action, string data)
        {
            //获取RequestCode的byte数组
            byte[] requestBytes = BitConverter.GetBytes((int)request);

            //获取ActionCode的byte数组
            byte[] actionBytes = BitConverter.GetBytes((int)action);

            //获取返回信息的数组
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            //获取数据长度的byte数组
            int dataAmount = requestBytes.Length + actionBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);

            //拼接信息：长度-code-数据
            byte[] result = Tools.ConcatBytes(dataAmountBytes, requestBytes);
            result = Tools.ConcatBytes(result, actionBytes);
            result = Tools.ConcatBytes(result, dataBytes);

            return result;
        }

    }
}
