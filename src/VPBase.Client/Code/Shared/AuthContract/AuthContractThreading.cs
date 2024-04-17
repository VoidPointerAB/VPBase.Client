using System.Threading;
using VPBase.Auth.Contract.SharedInterfaces;

namespace VPBase.Client.Code.Shared.AuthContract
{
    public class AuthContractThreading : IAuthContractThreading
    {
        public void ThreadSleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}
