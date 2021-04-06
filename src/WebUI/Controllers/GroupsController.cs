using System;
using System.Threading.Tasks;
using AutoMapper;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Groups.Queries.GetUserGroups;
using Electricity.Application.Groups.Queries.GetUserGroupTree;
using KMB.DataSource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Electricity.WebUI.Controllers
{
    public class GroupsController : ApiController
    {
        [HttpGet("user")]
        public async Task<ActionResult<UserGroupsDto>> GetUserGroups()
        {
            return await Mediator.Send(new GetUserGroupsQuery());
        }

        [HttpGet("user-tree")]
        public async Task<ActionResult<GroupTreeNodeDto>> GetUserGroupTree()
        {
            return await Mediator.Send(new GetUserGroupTreeQuery());
        }

        [HttpGet("info")]
        public ActionResult<GroupInfo> GetGroupInfo(string id, [FromQuery] InfoFilterDto filter)
        {
            var gs = HttpContext.RequestServices.GetService<IGroupService>();
            var m = HttpContext.RequestServices.GetService<IMapper>();

            var f = m.Map<InfoFilter>(filter);

            var info = gs.GetGroupInfo(id, f);

            return info;
        }
    }
}