using Application.Common.Requests;
using Application.Features.ExerciseMuscleGroups.Commands;
using Application.Features.ExerciseMuscleGroups.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ExerciseMuscleGroupsController(IMediator mediator) : ControllerBase
{


	// GET: api/<ExerciseMuscleGroupsController>
	[HttpGet]
	public async Task<IResult> Get([FromQuery] QueryRequest query)
	{
		var result = await  mediator.Send(new GetPagedExerciseMuscleGroupQuery(query));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// GET api/<ExerciseMuscleGroupsController>/5
	[HttpGet("{id:int}")]
	public async Task<IResult> Get(int id)
	{
		var result = await  mediator.Send(new GetExerciseMuscleGroupByIdQuery(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// POST api/<ExerciseMuscleGroupsController>
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public async Task<IResult> Post([FromBody] CreateExerciseMuscleGroupRequest request)
	{
		var result = await  mediator.Send(new CreateExerciseMuscleGroupCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// PUT api/<ExerciseMuscleGroupsController>/5
	[Authorize(Roles = "Admin")]
	[HttpPut("{id}")]
	public async Task<IResult> Put([FromBody] UpdateExerciseMuscleGroupRequest request)
	{
		var result = await mediator.Send(new UpdateExerciseMuscleGroupCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// DELETE api/<ExerciseMuscleGroupsController>/5
	[Authorize(Roles = "Admin")]
	[HttpDelete("{id:int}")]
	public async Task<IResult> Delete([FromRoute] int id)
	{
		var result = await mediator.Send(new RemoveExerciseMuscleGroupCommand(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}
}
