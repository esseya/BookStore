using Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _authService;
        public IAuthenticationService AuthService => _authService.Value;

        public ServiceManager(Lazy<IAuthenticationService> authService)
        {
            _authService = authService;
        }
    }
}
