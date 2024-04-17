namespace VPBase.Client.Code.Log4Net
{
    public interface IHeartbeatClient
    {
        void Heartbeat();
        void Heartbeat(string additonalInfo, string identifier = null, int? intervalInSeconds = null);
    }
}