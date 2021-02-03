using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class FakeIdentityService : IIdentityService
    {
        public Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            return Task.FromResult(true);
        }

        public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> DeleteUserAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserNameAsync(string userId)
        {
            return Task.FromResult("fake-user");
        }

        public Task<bool> IsInRoleAsync(string userId, string role)
        {
            return Task.FromResult(true);
        }
    }
}