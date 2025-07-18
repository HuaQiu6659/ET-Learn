using System;
using System.Net;
using System.Net.Sockets;

namespace ET.Client
{
    //A2B_MessageNameHandler
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_LoginHandler: MessageHandler<Scene, Main2NetClient_Login, NetClient2Main_Login>
    {
        //Client向RealmServer请求登录, 获取token
        //Client利用token向GateServer请求登录

        protected override async ETTask Run(Scene root, Main2NetClient_Login request, NetClient2Main_Login response)
        {
            string account = request.Account;
            long password = request.Password;
            // 创建一个ETModel层的Session
            root.RemoveComponent<RouterAddressComponent>();
            // 获取路由跟realmDispatcher地址
            RouterAddressComponent routerAddressComponent =
                    root.AddComponent<RouterAddressComponent, string>(request.Address);
            await routerAddressComponent.Init();
#if UNITY_WEBGL
            root.AddComponent<NetComponent, IKcpTransport>(new WebSocketTransport(routerAddressComponent.AddressFamily));
#else
            root.AddComponent<NetComponent, IKcpTransport>(new UdpTransport(routerAddressComponent.AddressFamily));
#endif
            root.GetComponent<FiberParentComponent>().ParentFiberId = request.OwnerFiberId;

            //负载均衡, 把账号均分到多个Realm服务器中
            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

            R2C_LoginRespose r2CLogin;
            NetComponent netComponent = root.GetComponent<NetComponent>();

            //通过路由转发给登陆服务器
            Session session = await netComponent.CreateRouterSession(realmAddress, account, password);
            C2R_LoginRequest c2RLogin = C2R_LoginRequest.Create();
            {
                c2RLogin.Account = account;
                c2RLogin.Password = password;
                r2CLogin = (R2C_LoginRespose)await session.Call(c2RLogin);
            }

            //登录失败
            if (r2CLogin.Error != ErrorCode.ERR_Success)
            {
                response.Error = r2CLogin.Error;
                response.Message = r2CLogin.Message;
                session?.Dispose();
                return;
            }

            //暂存连接
            root.AddComponent<SessionComponent>().Session = session;
            response.Token = r2CLogin.Token;
        }
    }
}