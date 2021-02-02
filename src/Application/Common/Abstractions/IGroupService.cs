using DataSource;
using Electricity.Application.Common.Models;
using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IGroupService
    {
        public Group[] GetUserGroups(Guid userId);

        public GroupTreeNode GetUserGroupTree(Guid userId);
    }
}