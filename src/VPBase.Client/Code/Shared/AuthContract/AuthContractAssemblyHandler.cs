using System;
using System.Reflection;
using VPBase.Auth.Contract.SharedInterfaces;

namespace VPBase.Client.Code.Shared.AuthContract
{
    public class AuthContractAssemblyHandler : IAuthContractAssemblyHandler
    {
        private AuthContractAssemblyType _selectedAssemblyType = AuthContractAssemblyType.AuthContract;
        private Type _customAssemblyType = null;

        public AuthContractAssemblyHandler() { }


        public string GetVersion()
        {
            return GetContractAssemblyVersion().ToString();
        }

        public int Revision
        {
            get
            {
                return GetContractAssemblyVersion().Revision;
            }
        }

        public int Build
        {
            get
            {
                return GetContractAssemblyVersion().Build;
            }
        }

        public int Minor
        {
            get
            {
                return GetContractAssemblyVersion().Minor;
            }
        }

        public int Major
        {
            get
            {
                return GetContractAssemblyVersion().Major;
            }
        }

        public string GetName()
        {
            var contractAssembly = GetSelectedAssembly();
            var contractAssemblyName = contractAssembly.GetName();
            return contractAssemblyName.Name;
        }

        public string GetFullName()
        {
            var contractAssembly = GetSelectedAssembly();
            var contractAssemblyName = contractAssembly.GetName();
            return contractAssemblyName.FullName;
        }

        #region AssemblyType

        public AuthContractAssemblyType GetSelectedAssemblyType()
        {
            return _selectedAssemblyType;
        }

        public void SetSelectedAssemblyType(AuthContractAssemblyType selectedAssemblyType)
        {
            _selectedAssemblyType = selectedAssemblyType;
        }

        public void SetSelectedCustomAssembly(Type type)
        {
            _selectedAssemblyType = AuthContractAssemblyType.Custom;
            _customAssemblyType = type;
        }

        #endregion

        #region Private

        private Version GetContractAssemblyVersion()
        {
            var contractAssembly = GetSelectedAssembly();
            var contractAssemblyVersion = contractAssembly.GetName().Version;
            return contractAssemblyVersion;
        }

        private Assembly GetSelectedAssembly()
        {
            switch (_selectedAssemblyType)
            {
                case AuthContractAssemblyType.AuthContract:
                    {
                        return Assembly.GetAssembly(typeof(IAuthContractAssemblyHandler));
                    }
                case AuthContractAssemblyType.Custom:
                    {
                        return Assembly.GetAssembly(_customAssemblyType);
                    }
                default:
                    {
                        return Assembly.GetAssembly(typeof(IAuthContractAssemblyHandler));
                    }
            }
        }

        #endregion
    }
}