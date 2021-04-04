using Electricity.Application.Common.Models;
using KMB.DataSource;
using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IGroupService
    {
        public Group GetGroupById(string id);

        public GroupInfo GetGroupInfo(string id, InfoFilter infoFilter);

        public Group[] GetUserGroups();

        public GroupTreeNode GetUserGroupTree();
    }
}