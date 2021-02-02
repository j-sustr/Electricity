using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IAuthenticationService
    {
        Guid Login(string username, string password);
    }
}