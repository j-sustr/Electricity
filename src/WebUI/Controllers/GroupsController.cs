using System.Threading.Tasks;
using AutoMapper;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Groups.Queries.GetUserGroupInfoTree;
using Electricity.Application.Groups.Queries.GetUserRecordGroupInfos;
using KMB.DataSource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Electricity.WebUI.Controllers
{
    public class GroupsController : ApiController
    {
        [HttpGet("user-group-info-tree")]
        public async Task<ActionResult<GroupInfoDto>> GetUserGroupInfoTree()
        {
            return await Mediator.Send(new GetUserGroupInfoTreeQuery());
        }

        [HttpGet("user-record-group-infos")]
        public async Task<ActionResult<GroupInfoDto[]>> GetUserRecordGroupInfos()
        {
            return await Mediator.Send(new GetUserRecordGroupInfosQuery());
        }

        [HttpGet("info")]
        public ActionResult<GroupInfoDto> GetGroupInfo(string id, [FromQuery] InfoFilterDto filter)
        {
            var gs = HttpContext.RequestServices.GetService<IGroupRepository>();
            var m = HttpContext.RequestServices.GetService<IMapper>();

            var f = m.Map<InfoFilter>(filter);

            var info = gs.GetGroupInfo(id);

            var dto = m.Map<GroupInfoDto>(info);

            return dto;
        }
    }
}