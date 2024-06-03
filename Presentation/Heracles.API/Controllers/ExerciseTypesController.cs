
using System.Text.Json;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Heracles.API.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ExerciseTypesController : ControllerBase
{
    private readonly IExerciseTypeService _service;

    public ExerciseTypesController(IExerciseTypeService service)
    {
        _service = service;
    }
    
    // GET: api/<ExerciseTypesController>
    [HttpGet]
    public async Task<IResult> Get([FromQuery] QueryRequestDto query)
    {
        var result = await _service.GetAsync(query);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    
    // GET api/<ExerciseTypesController>/5
    [HttpGet("{id}")]
    public async Task<IResult> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    // POST api/<ExerciseTypesController>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IResult> Post([FromBody] ExerciseType exerciseType)
    {
        var result = await _service.CreateAsync(exerciseType);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    // PUT api/<ExerciseTypesController>/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IResult> Put([FromBody] ExerciseType exerciseType)
    {
        var result = await _service.UpdateAsync(exerciseType);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    // DELETE api/<ExerciseTypesController>/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IResult> Delete([FromRoute] int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
}

