/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/

namespace ET
{
    public partial class StartSceneConfigCategory
    {
        private StartSceneConfig loginCenterConfig;
        public StartSceneConfig LoginCenterConfig
        {
            get
            {
                if (loginCenterConfig is null)
                    loginCenterConfig = GetBySceneType(1000, SceneType.LoginCenter)[0];

                return loginCenterConfig;
            }
        }
    }
}