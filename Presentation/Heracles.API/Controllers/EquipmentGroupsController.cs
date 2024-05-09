using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.EquipmentGroups.DTOs;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EquipmentGroupsController : ControllerBase
    {
        private readonly IEquipmentGroupService _service;

        public EquipmentGroupsController(IEquipmentGroupService service)
        {
            _service = service;
        }
        
        // GET: api/<EquipmentGroupsController>
        [HttpGet]
        public async Task<IResult> Get([FromQuery] QueryRequest query)
        {
            var result = await _service.GetAsync(query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // GET api/<EquipmentGroupsController>/5
        [HttpGet("{id}")]
        public async Task<IResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // POST api/<EquipmentGroupsController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IResult> Post([FromBody] EquipmentGroup entity)
        {
            var result = await _service.CreateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // PUT api/<EquipmentGroupsController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IResult> Put( [FromBody] EquipmentGroup entity)
        {
            var result = await _service.UpdateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
        
        // PATCH api/<EquipmentGroupsController>/5/add
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/add")]
        public async Task<IResult> AddEquipment([FromBody] AddRemoveEquipmentGroupDto dto)
        {
            var result = await _service.AddEquipmentAsync(dto);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
        
        // PATCH api/<EquipmentGroupsController>/5/remove
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/remove")]
        public async Task<IResult> RemoveEquipment([FromBody] AddRemoveEquipmentGroupDto dto)
        {
            var result = await _service.RemoveEquipmentAsync(dto);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
        
        
        // DELETE api/<EquipmentGroupsController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IResult> Delete([FromRoute] int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
        
    }
}
