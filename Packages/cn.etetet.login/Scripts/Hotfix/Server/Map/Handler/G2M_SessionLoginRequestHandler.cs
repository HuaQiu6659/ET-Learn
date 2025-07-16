/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    public class G2M_SessionLoginRequestHandler : MessageLocationHandler<Unit, G2M_SessionLoginRequest, M2G_SessionLoginResponse>
    {
        protected override ETTask Run(Unit unit, G2M_SessionLoginRequest request, M2G_SessionLoginResponse response)
        {
            throw new System.NotImplementedException("TODO: 二次登录逻辑");
        }
    }
}