using System.Collections.Generic;
using DataSource;

namespace Electricity.Application.Groups.Queries.GetUserGroups
{
    public class UserGroupsDto
    {
        public IList<Group> Groups { get; set; } = new List<Group>();
    }
}