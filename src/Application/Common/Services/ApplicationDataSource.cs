using System;
using DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Services
{
    public class ApplicationDataSource : IGroupService
    {
        private readonly User _user;

        public ApplicationDataSource(IDataSourceProvider dataSourceProvider, IUserProvider userProvider)
        {

        }



        public GroupTreeNode GetGroupTree(Guid root)
        {
            throw new NotImplementedException();
        }

        public Group[] GetUserGroups(Guid user)
        {
            throw new NotImplementedException();
        }
    }
}