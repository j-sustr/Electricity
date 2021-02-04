using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Infrastructure.Identity;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class FakeIdentityService : IIdentityService
    {
        List<ApplicationUser> users = new List<ApplicationUser>();

        public Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            return Task.FromResult(true);
        }

        public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            users.Add(new ApplicationUser
            {
                UserName = userName,
                PasswordHash = password
            });

            var guid = Guid.NewGuid();
            return Task.FromResult((Result.Success(), guid.ToString()));
        }

        public Task<Result> DeleteUserAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserNameAsync(string userId)
        {
            return Task.FromResult("fake-user-name");
        }

        public Task<bool> IsInRoleAsync(string userId, string role)
        {
            return Task.FromResult(true);
        }
    }
}