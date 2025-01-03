using System;
using System.Net;
using System.Net.Sockets;

namespace ET.Client
{
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_LoginHandler: MessageHandler<Scene, Main2NetClient_Login, NetClient2Main_Login>
    {
        protected override async ETTask Run(Scene root, Main2NetClient_Login request, NetClient2Main_Login response)
        {
            string account = request.Account;
            string password = request.Password;
            // 创建一个ETModel层的Session
            root.RemoveComponent<RouterAddressComponent>();
            // 获取路由跟realmDispatcher地址
            RouterAddressComponent routerAddressComponent =
                    root.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
            await routerAddressComponent.Init();
            root.AddComponent<NetComponent, AddressFamily, NetworkProtocol>(routerAddressComponent.RouterManagerIPAddress.AddressFamily, NetworkProtocol.UDP);
            root.GetComponent<FiberParentComponent>().ParentFiberId = request.OwnerFiberId;

            NetComponent netComponent = root.GetComponent<NetComponent>();
            
            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

            Session session = await netComponent.CreateRouterSession(realmAddress, account, password);
            var c2rLoginReq = C2R_LoginAccountRequest.Create();
            c2rLoginReq.Account = account;
            c2rLoginReq.Password = password;
            R2C_LoginAccountResponse r2cLoginRsp = (R2C_LoginAccountResponse)await session.Call(c2rLoginReq);
            if (r2cLoginRsp.Error == ErrorCode.ERR_Success)
            {
                //直接保持连接 
                root.AddComponent<SessionComponent>().Session = session;
            }
            else
            {
                session?.Dispose();
            }

            response.token = r2cLoginRsp.token;
            response.Error = r2cLoginRsp.Error;
        }
    }
}