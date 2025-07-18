namespace ET
{
    public static partial class TimerInvokeType
    {
        public const int SessionIdleChecker = PackageType.Login * 1000 + 1;
        public const int SessionAcceptTimeout = PackageType.Login * 1000 + 2;
        public const int AccountSessionCheckTimeOut = PackageType.Login * 1000 + 3;
        public const int VerificationCodeTimeOut = PackageType.Login * 1000 + 4;
        public const int PlayerOfflineTimeOut = PackageType.Login * 1000 + 5;
        public const int CacheRefresh = PackageType.Login * 1000 + 6;
    }
}