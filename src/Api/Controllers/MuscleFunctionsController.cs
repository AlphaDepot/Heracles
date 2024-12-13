using Application.Common.Requests;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleFunctions.Commands;
using Application.Features.MuscleFunctions.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class MuscleFunctionsController(IMediator mediator) : ControllerBase
{



	// GET: api/<MuscleFunctionsController>
	[HttpGet]
	public async Task<IResult> Get([FromQuery] QueryRequest query)
	{
		var result = await  mediator.Send(new GetPagedMuscleFunctionsQuery(query));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// GET api/<MuscleFunctionsController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id)
	{
		var result = await  mediator.Send(new GetMuscleFunctionByIdQuery(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// POST api/<MuscleFunctionsController>
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public async Task<IResult> Post([FromBody] CreateMuscleFunctionRequest request)
	{
		var result = await  mediator.Send(new CreateMuscleFunctionCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// PUT api/<MuscleFunctionsController>/5
	[Authorize(Roles = "Admin")]
	[HttpPut("{id}")]
	public async Task<IResult> Put([FromBody] UpdateMuscleFunctionRequest request)
	{
		var result = await mediator.Send(new UpdateMuscleFunctionCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// DELETE api/<MuscleFunctionsController>/5
	[Authorize(Roles = "Admin")]
	[HttpDelete("{id}")]
	public async Task<IResult> Delete([FromRoute] int id)
	{
		var result = await  mediator.Send(new RemoveMuscleFunctionCommand(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}
}
