using ET.Helper;

namespace ET.Client
{
    //用结构体作为时间编号同时直接进行事件传参
    
    public struct SceneChangeStart
    {
    }
    
    public struct SceneChangeFinish
    {
    }
    
    public struct AfterCreateClientScene
    {
    }
    
    public struct AfterCreateCurrentScene
    {
    }

    public struct AppStartInitFinish
    {
    }

    public struct LoginFinish
    {
    }

    public struct EnterMapFinish
    {
    }

    public struct AfterUnitCreate
    {
        public Unit Unit;
    }

    public struct TestEventStruct
    {
        public int intArg;
    }
    
    //登录事件
    public struct LoginFailedEvent
    {
        public int errorCode;

        public override string ToString()
        {
            if (this.errorCode >= (int)EErrorCode.ERR_RequestRepeated)
            {
                var codeEnum = (EErrorCode)this.errorCode;
                return codeEnum.GetEnumDescription();
            }
            
            return base.ToString();
        }
    }
}