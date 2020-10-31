using System;
using System.Threading.Tasks;
using Electricity.Application.Groups.Queries.GetUserGroups;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.WebUI.Controllers
{
    public class GroupsController : ApiController
    {
        // [HttpGet("user/{userId}")]
        [HttpGet]
        public async Task<ActionResult<UserGroupsDto>> GetUserGroups([FromQuery] Guid userId)
        {
            return await Mediator.Send(new GetUserGroupsQuery
            {
                UserId = userId,
            });
        }

        [HttpGet("user-tree/{userId}")]
        public Task GetUserGroupTree(Guid userId)
        {
            return Task.CompletedTask;
        }
    }
}