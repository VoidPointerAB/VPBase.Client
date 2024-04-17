using System;
using VPBase.Auth.Contract.SharedInterfaces;

namespace VPBase.Client.Code.Shared.AuthContract
{
    public class AuthContractDateTimeProvider : IAuthContractDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public DateTime Today()
        {
            return DateTime.Today;
        }
    }

    public class TestAuthContractDateTimeProvider : IAuthContractDateTimeProvider
    {
        private DateTime _date;

        public TestAuthContractDateTimeProvider()
        {
            _date = DateTime.Now;
        }

        public TestAuthContractDateTimeProvider(DateTime dateTime)
        {
            _date = dateTime;
        }

        public void SetDate(DateTime dateTime)
        {
            _date = dateTime;
        }

        public DateTime Now()
        {
            return _date;
        }

        public DateTime Today()
        {
            var dateWithOutTime = new DateTime(_date.Year, _date.Month, _date.Day);
            return dateWithOutTime;
        }
    }
}
