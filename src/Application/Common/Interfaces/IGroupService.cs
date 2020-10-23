using DataSource;
using Electricity.Application.Common.Models;
using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IGroupService
    {
        public Group[] GetUserGroups(Guid user);

        public GroupTreeNode GetUserGroupTree(Guid root);
    }
}