using Common;
using CSharpServer.Servers;
using CSharpServer.ServerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpServer.Contoller
{
    public class ContollerDefault : ControllerBase
    {
        public ContollerDefault()
        {
            this._request = RequestCode.None;
        }

        public string None(string data, Client client, Server server)
        {
            var info = string.Format("服务器接收到：{0}", data);
            LogManager.LogInfo(info);
            return info;
        }
    }
}
