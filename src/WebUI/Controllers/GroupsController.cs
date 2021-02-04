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
        // [HttpGet("user/{userId}")]
        [HttpGet]
        public async Task<ActionResult<UserGroupsDto>> GetUserGroups([FromQuery] Guid userId)
        {
            return await Mediator.Send(new GetUserGroupsQuery());
        }

        // [HttpGet("user-tree/{userId}")]
        [HttpGet("tree")]
        public async Task<ActionResult<GroupTreeNodeDto>> GetUserGroupTree([FromQuery] Guid userId)
        {
            return await Mediator.Send(new GetUserGroupTreeQuery());
        }
    }
}