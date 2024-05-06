using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExerciseHistories.DTOs;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserExerciseHistoriesController : ControllerBase
    {
        private readonly IUserExerciseHistoryService _service;

        public UserExerciseHistoriesController(IUserExerciseHistoryService service)
        {
            _service = service;
        }
        
        
        // GET: api/<UserExerciseHistoriesController>
        [HttpGet]
        public async Task<IResult> Get([FromQuery] QueryRequest query)
        {
            var result = await _service.GetAsync(query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
     

        // GET api/<UserExerciseHistoriesController>/5
        [HttpGet("{id}")]
        public  async Task<IResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // POST api/<UserExerciseHistoriesController>
        [HttpPost]
        public  async Task<IResult> Post([FromBody] UserExerciseHistory entity)
        {
            var result = await _service.CreateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // PUT api/<UserExerciseHistoriesController>/5
        [HttpPut("{id}")]
        public  async Task<IResult> Put([FromBody] UpdateUserExerciseHistoryDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // DELETE api/<UserExerciseHistoriesController>/5
        [HttpDelete("{id}")]
        public  async Task<IResult> Delete([FromRoute] int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
    }
}
