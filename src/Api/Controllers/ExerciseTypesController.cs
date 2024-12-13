using Application.Common.Requests;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Commands;
using Application.Features.ExerciseTypes.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ExerciseTypesController(IMediator mediator) : ControllerBase
{


    // GET: api/<ExerciseTypesController>
    [HttpGet]
    public async Task<IResult> Get([FromQuery] QueryRequest query)
    {
        var result = await  mediator.Send(new GetPagedExerciseTypesQuery(query));
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }

    // GET api/<ExerciseTypesController>/5
    [HttpGet("{id}")]
    public async Task<IResult> Get(int id)
    {
        var result = await  mediator.Send(new GetExerciseTypeByIdQuery(id));
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    // POST api/<ExerciseTypesController>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IResult> Post([FromBody] CreateExerciseTypeRequest request)
    {
        var result = await  mediator.Send(new CreateExerciseTypeCommand(request));
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    // PUT api/<ExerciseTypesController>/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IResult> Put([FromBody] UpdateExerciseTypeRequest request)
	{
		var result = await  mediator.Send(new UpdateExerciseTypeCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


    //Patch api/<ExerciseTypesController>/5/add
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/add")]
    public async Task<IResult> AddMuscleGroup([FromBody] AttachExerciseMuscleGroupRequest request)
	{
		var result = await  mediator.Send(new AttachExerciseMuscleGroupCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

    // PATCH api/<ExerciseTypesController>/5/remove
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/remove")]
    public async Task<IResult> RemoveMuscleGroup([FromBody] DetachExerciseMuscleGroupRequest request)
    {
	    var result = await  mediator.Send(new DetachExerciseMuscleGroupCommand(request));
	    return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }


    // DELETE api/<ExerciseTypesController>/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IResult> Delete([FromRoute] int id)
    {
        var result = await  mediator.Send(new RemoveExerciseTypeCommand(id));
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
}

