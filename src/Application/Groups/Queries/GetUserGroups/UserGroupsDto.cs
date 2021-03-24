using System.Collections.Generic;
using KMB.DataSource;
using Electricity.Application.Common.Models.Dtos;

namespace Electricity.Application.Groups.Queries.GetUserGroups
{
    public class UserGroupsDto
    {
        public IList<GroupDto> Groups { get; set; } = new List<GroupDto>();
    }
}