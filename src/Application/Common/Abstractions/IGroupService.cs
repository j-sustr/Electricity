using Electricity.Application.Common.Models;
using KMB.DataSource;
using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IGroupService
    {
        public GroupInfo GetUserGroupInfoTree();

        public GroupInfo[] GetUserRecordGroupInfos();
        
        public Group[] GetUserGroups();

        public GroupTreeNode GetUserGroupTree();

        public GroupInfo GetGroupInfo(string id, InfoFilter infoFilter);

        public Group GetGroupById(string id);
    }
}