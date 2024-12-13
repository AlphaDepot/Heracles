using Application.Common.Requests;
using Application.Features.MuscleGroups;
using Application.Features.MuscleGroups.Commands;
using Application.Features.MuscleGroups.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class MuscleGroupsController (IMediator mediator) : ControllerBase
{

	// GET: api/<MuscleGroupsController>
	[HttpGet]
	public async Task<IResult> Get([FromQuery] QueryRequest query)
	{
		var result = await  mediator.Send(new GetPagedMuscleGroupsQuery(query));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// GET api/<MuscleGroupsController>/5
	[HttpGet("{id}")]
	public  async Task<IResult> Get(int id)
	{
		var result = await mediator.Send(new GetMuscleGroupByIdQuery(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// POST api/<MuscleGroupsController>
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public  async Task<IResult> Post([FromBody] CreateMuscleGroupRequest request)
	{
		var result = await mediator.Send(new CreateMuscleGroupCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}



	// PUT api/<MuscleGroupsController>/5
	[Authorize(Roles = "Admin")]
	[HttpPut("{id}")]
	public  async Task<IResult> Put( [FromBody] UpdateMuscleGroupRequest request)
	{
		var result = await mediator.Send(new UpdateMuscleGroupCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// DELETE api/<MuscleGroupsController>/5
	[Authorize(Roles = "Admin")]
	[HttpDelete("{id}")]
	public  async Task<IResult> Delete([FromRoute] int id)
	{
		var result =  await mediator.Send(new RemoveMuscleGroupCommand(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}
}
