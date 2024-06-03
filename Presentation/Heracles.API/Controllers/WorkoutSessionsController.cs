using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.WorkoutSessions.DTOs;
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly IWorkoutSessionService _service;

        public WorkoutSessionsController(IWorkoutSessionService service)
        {
            _service = service;
        }
        
        // GET: api/<WorkoutSessionsController>
        [HttpGet]
        public async Task<IResult> Get([FromQuery] QueryRequestDto query)
        {
            var result = await _service.GetAsync(query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
  
        // GET api/<WorkoutSessionsController>/5
        [HttpGet("{id}")]
        public async Task<IResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // POST api/<WorkoutSessionsController>
        [HttpPost]
        public async Task<IResult> Post([FromBody] WorkoutSession entity)
        {
            var result = await _service.CreateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // PUT api/<WorkoutSessionsController>/5
        [HttpPut("{id}")]
        public async Task<IResult> Put( [FromBody] WorkoutSession entity)
        {
            var result = await _service.UpdateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
        
        // PATCH api/<WorkoutSessionsController>/5/add
        [HttpPatch("{id}/add")]
        public async Task<IResult> AddExercise( [FromBody] WorkoutSessionExerciseDto entity)
        {
            var result = await _service.AddUserExerciseAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
        
        // PATCH api/<WorkoutSessionsController>/5/remove
        [HttpPatch("{id}/remove")]
        public async Task<IResult> RemoveExercise( [FromBody] WorkoutSessionExerciseDto entity)
        {
            var result = await _service.RemoveUserExerciseAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // DELETE api/<WorkoutSessionsController>/5
        [HttpDelete("{id}")]
        public async Task<IResult> Delete([FromRoute] int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
    }
}
