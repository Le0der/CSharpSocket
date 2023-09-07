using Common;
using CSharpServer.Servers;
using CSharpServer.ServerTools;

namespace CSharpServer.Contoller
{
    public class ControllerManager
    {
        private Server _server;
        private Dictionary<RequestCode, ControllerBase> _ctrlDict = new Dictionary<RequestCode, ControllerBase>();

        public ControllerManager(Server server)
        {
            this._server = server;
            InitController();
        }

        private void InitController()
        {
            var defaultController = new ContollerDefault();
            this._ctrlDict.Add(defaultController.Request, defaultController);
        }

        public void HandleRequest(RequestCode request, ActionCode action, string data, Client client)
        {
            //根据requestCode获得执行的controller
            if (this._ctrlDict.TryGetValue(request, out var controller))
            {
                //建立反射
                var type = controller.GetType();
                var methodInfo = Tools.GetMethodInfo(action, type);

                //没找到反射方法，报错退出
                if (methodInfo == null)
                {
                    LogManager.LogError(string.Format("在Controller:[{0}]中没有对应的处理方法：[{1}]", type, action));
                    return;
                }

                //构造反射方法参数，执行反射
                var parameters = new object[] { data, client, _server };
                var methodResult = methodInfo.Invoke(controller, parameters);

                //查看是否有给客户端的返回值
                if (methodResult == null) return;
                var resultData = methodResult as string;

                //给客户端返回数据
                if (string.IsNullOrEmpty(resultData)) return;
                _server.SendResponse(client, action, resultData);
            }
            else
            {
                LogManager.LogError(string.Format("无法得到：[{0}] 所对应的Contoller，无法处理请求", request));
            }
        }
    }
}
