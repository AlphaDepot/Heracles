using Application.Common.Requests;
using Application.Features.Equipments.Commands;
using Application.Features.Equipments.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EquipmentsController(IMediator mediator) : ControllerBase
{
	// GET: api/<EquipmentsController>
	[HttpGet]
	public async Task<IResult> Get([FromQuery] QueryRequest query)
	{
		var result = await mediator.Send(new GetPagedEquipmentsQuery(query));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// GET api/<EquipmentsController>/5
	[HttpGet("{id}")]
	public  async Task<IResult> Get(int id)
	{
		var result = await mediator.Send(new GetEquipmentByIdQuery(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// POST api/<EquipmentsController>
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public  async Task<IResult> Post([FromBody] CreateEquipmentRequest request)
	{
		var result = await mediator.Send(new CreateEquipmentCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// PUT api/<EquipmentsController>/5
	[Authorize(Roles = "Admin")]
	[HttpPut("{id}")]
	public  async Task<IResult> Put([FromBody] UpdateEquipmentRequest request)
	{
		var result = await mediator.Send(new UpdateEquipmentCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// DELETE api/<EquipmentsController>/5
	[Authorize(Roles = "Admin")]
	[HttpDelete("{id}")]
	public  async Task<IResult> Delete([FromRoute] int id)
	{
		var result = await mediator.Send(new RemoveEquipmentCommand(id));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}
}
