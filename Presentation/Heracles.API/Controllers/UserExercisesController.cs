using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserExercisesController : ControllerBase
    {
        private readonly IUserExerciseService _service;

        public UserExercisesController(IUserExerciseService service)
        {
            _service = service;
        }
        
        // GET: api/<UserExercisesController>
        [HttpGet]
        public async Task<IResult> Get([FromQuery] QueryRequest query)
        {
            var result = await _service.GetAsync(query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
    

        // GET api/<UserExercisesController>/5
        [HttpGet("{id}")]
        public  async Task<IResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // POST api/<UserExercisesController>
        [HttpPost]
        public  async Task<IResult> Post([FromBody] CreateUserExerciseDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // PUT api/<UserExercisesController>/5
        [HttpPut("{id}")]
        public  async Task<IResult> Put( [FromBody] UpdateUserExerciseDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // DELETE api/<UserExercisesController>/5
        [HttpDelete("{id}")]
        public  async Task<IResult> Delete([FromRoute] int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
    }
}
