using Application.Features.WorkoutSessions.Commands;
using Application.Features.WorkoutSessions.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class WorkoutSessionsController(IMediator mediator) : ControllerBase
{
	// GET: api/<WorkoutSessionsController>
	[HttpGet]
	public async Task<IResult> Get()
	{
		var result = await mediator.Send(new WorkoutSessionsByUserIdQuery());
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// GET api/<WorkoutSessionsController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id)
	{
		var result = await mediator.Send(new WorkoutSessionByIdAndUserIdQuery(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// POST api/<WorkoutSessionsController>
	[HttpPost]
	public async Task<IResult> Post([FromBody] CreateWorkoutSessionRequest request)
	{
		var result = await mediator.Send(new CreateWorkoutSessionCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// PUT api/<WorkoutSessionsController>/5
	[HttpPut("{id}")]
	public async Task<IResult> Put([FromBody] UpdateWorkoutSessionRequest request)
	{
		var result = await mediator.Send(new UpdateWorkoutSessionCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// PATCH api/<WorkoutSessionsController>/5/add
	[HttpPatch("{id}/add")]
	public async Task<IResult> AddExercise([FromBody] AttachUserExerciseToWorkoutSessionRequest request)
	{
		var result = await mediator.Send(new AttachUserExerciseToWorkoutSessionCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// PATCH api/<WorkoutSessionsController>/5/remove
	[HttpPatch("{id}/remove")]
	public async Task<IResult> RemoveExercise([FromBody] DetachUserExerciseToWorkoutSessionRequest request)
	{
		var result = await mediator.Send(new DetachUserExerciseToWorkoutSessionCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// DELETE api/<WorkoutSessionsController>/5
	[HttpDelete("{id}")]
	public async Task<IResult> Delete([FromRoute] int id)
	{
		var result = await mediator.Send(new RemoveWorkoutSessionCommand(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}
}
