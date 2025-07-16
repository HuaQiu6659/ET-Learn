/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [MessageHandler(SceneType.Map)]
    public partial class G2M_ExitGameRequestHandler : MessageLocationHandler<Unit, G2M_ExitGameRequest, M2G_ExitGameResponse>
    {
        protected override async ETTask Run(Unit unit, G2M_ExitGameRequest request, M2G_ExitGameResponse response)
        {
            await ETTask.CompletedTask;

            //TODO: Unit角色下线逻辑, 保存至数据库, 释放Unit
            RemoveUnit(unit).NoContext();
        }

        private async ETTask RemoveUnit(Unit unit)
        {
            await unit.Fiber().WaitFrameFinish();

            await unit.RemoveLocation(LocationType.Unit);
            unit.Root().GetComponent<UnitComponent>().Remove(unit.Id);
        }
    }
}