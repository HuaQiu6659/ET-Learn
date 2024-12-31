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
            switch (this.errorCode)
            {
                case ErrorCode.ERR_LoginInfoEmpty:
                    return "账号或密码为空";
                case ErrorCode.ERR_LoginWrongPassword:
                    return "密码错误";
            }
            
            return base.ToString();
        }
    }
}