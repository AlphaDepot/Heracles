using Application.Features.UserExerciseHistories.Commands;
using Application.Features.UserExerciseHistories.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UserExerciseHistoriesController(IMediator mediator) : ControllerBase
{
	// GET: api/<UserExerciseHistoriesController>/by-user-exercise/5
	[HttpGet("by-user-exercise/{userExerciseId}")]
	public async Task<IResult> GetByUserExerciseId(int userExerciseId)
	{
		var result = await mediator.Send(new UserExerciseHistoriesByUserExerciseIdQuery(userExerciseId));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// GET api/<UserExerciseHistoriesController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id)
	{
		var result = await mediator.Send(new UserExerciseHistoryByIdQuery(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// POST api/<UserExerciseHistoriesController>
	[HttpPost]
	public async Task<IResult> Post([FromBody] CreateUserExerciseHistoryRequest request)
	{
		var result = await mediator.Send(new CreateUserExerciseHistoryCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// PUT api/<UserExerciseHistoriesController>/5
	[HttpPut("{id}")]
	public async Task<IResult> Put([FromBody] UpdateUserExerciseHistoryRequest request)
	{
		var result = await mediator.Send(new UpdateUserExerciseHistoryCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// DELETE api/<UserExerciseHistoriesController>/5
	[HttpDelete("{id}")]
	public async Task<IResult> Delete([FromRoute] int id)
	{
		var result = await mediator.Send(new RemoveUserExerciseHistoryCommand(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}
}
