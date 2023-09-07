using Common;
using CSharpServer.Servers;

namespace CSharpServer.Contoller
{
    public abstract class ControllerBase
    {
        protected RequestCode _request = RequestCode.None;

        public RequestCode Request { get { return _request; } }

        /// <summary>
        /// 未指定ActionCode的默认方法
        /// </summary>
        /// <param name="data">接收到客户端的数据</param>
        /// <returns>给客户端的响应数据</returns>
        public virtual string? DefaultHandle(string data, Client client, Server server) { return null; }
    }
}