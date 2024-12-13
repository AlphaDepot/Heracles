using Application.Common.Requests;
using Application.Features.UserExercises.Commands;
using Application.Features.UserExercises.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UserExercisesController(IMediator mediator) : ControllerBase
{
	// GET: api/<UserExercisesController>
	[HttpGet]
	public async Task<IResult> Get([FromQuery] QueryRequest query)
	{
		var result = await mediator.Send(new UserPagedExercisesByUserIdQuery(query));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// GET api/<UserExercisesController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id)
	{
		var result = await mediator.Send(new UserExercisesByIdQuery(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// POST api/<UserExercisesController>
	[HttpPost]
	public async Task<IResult> Post([FromBody] CreateUserExerciseRequest request)
	{
		var result = await mediator.Send(new CreateUserExerciseCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// PUT api/<UserExercisesController>/5
	[HttpPut("{id}")]
	public async Task<IResult> Put([FromBody] UpdateUserExerciseRequest request)
	{
		var result = await mediator.Send(new UpdateUserExerciseCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// DELETE api/<UserExercisesController>/5
	[HttpDelete("{id}")]
	public async Task<IResult> Delete([FromRoute] int id)
	{
		var result = await mediator.Send(new RemoveUserExerciseCommand(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}
}
