using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ExerciseMuscleGroupsController : ControllerBase
    {
        private readonly IExerciseMuscleGroupService _service;

        public ExerciseMuscleGroupsController(IExerciseMuscleGroupService service)
        {
            _service = service;
        }
        
        // GET: api/<ExerciseMuscleGroupsController>
        [HttpGet]
        public async Task<IResult> Get([FromQuery] QueryRequestDto query)
        {
            var result = await _service.GetAsync(query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
      

        // GET api/<ExerciseMuscleGroupsController>/5
        [HttpGet("{id:int}")]
        public async Task<IResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
        
        // GET api/<ExerciseMuscleGroupsController>/exerciseId
        [HttpGet("exercise/{exerciseId:int}")]
        public async Task<IResult> GetByExerciseId([FromRoute] int exerciseId, [FromQuery] QueryRequestDto query)
        {
            var result = await _service.GetByExerciseIdAsync(exerciseId, query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }


        // POST api/<ExerciseMuscleGroupsController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IResult> Post([FromBody] CreateExerciseMuscleGroupDto entity)
        {
            var result = await _service.CreateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // PUT api/<ExerciseMuscleGroupsController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IResult> Put([FromBody] UpdateExerciseMuscleGroupDto entity)
        {
            var result = await _service.UpdateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // DELETE api/<ExerciseMuscleGroupsController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IResult> Delete([FromRoute] int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
    }
}
