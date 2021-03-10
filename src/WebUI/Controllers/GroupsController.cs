using System;
using System.Threading.Tasks;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Groups.Queries.GetUserGroups;
using Electricity.Application.Groups.Queries.GetUserGroupTree;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.WebUI.Controllers
{
    public class GroupsController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<UserGroupsDto>> GetUserGroups()
        {
            return await Mediator.Send(new GetUserGroupsQuery());
        }

        [HttpGet("tree")]
        public async Task<ActionResult<GroupTreeNodeDto>> GetUserGroupTree()
        {
            return await Mediator.Send(new GetUserGroupTreeQuery());
        }
    }
}