using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MuscleGroupsController : ControllerBase
    {
        private readonly IMuscleGroupService _service;

        public MuscleGroupsController(IMuscleGroupService service)
        {
            _service = service;
        }
        
        
        // GET: api/<MuscleGroupsController>
        [HttpGet]
        public async Task<IResult> Get([FromQuery] QueryRequest query)
        {
            var result = await _service.GetAsync(query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }


        // GET api/<MuscleGroupsController>/5
        [HttpGet("{id}")]
        public  async Task<IResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // POST api/<MuscleGroupsController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public  async Task<IResult> Post([FromBody] MuscleGroup entity)
        {
            var result = await _service.CreateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }


        // PUT api/<MuscleGroupsController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public  async Task<IResult> Put( [FromBody] MuscleGroup entity)
        {
            var result = await _service.UpdateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // DELETE api/<MuscleGroupsController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public  async Task<IResult> Delete([FromRoute] int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
    }
}
