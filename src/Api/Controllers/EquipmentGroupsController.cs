using Application.Common.Requests;
using Application.Features.EquipmentGroups.Commands;
using Application.Features.EquipmentGroups.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class EquipmentGroupsController(IMediator mediator) : ControllerBase
{
	// GET: api/<EquipmentGroupsController>
	[HttpGet]
	public async Task<IResult> Get([FromQuery] QueryRequest query)
	{
		var result = await mediator.Send(new GetPagedEquipmentGroupsQuery(query));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// GET api/<EquipmentGroupsController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id)
	{
		var result = await mediator.Send(new GetEquipmentGroupByIdQuery(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// POST api/<EquipmentGroupsController>
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public async Task<IResult> Post([FromBody] CreateEquipmentGroupRequest createEquipmentGroup)
	{
		var result = await mediator.Send(new CreateEquipmentGroupCommand(createEquipmentGroup));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	// PUT api/<EquipmentGroupsController>/5
	[Authorize(Roles = "Admin")]
	[HttpPut("{id}")]
	public async Task<IResult> Put( [FromBody] UpdateEquipmentGroupRequest updateEquipmentGroup)
	{
		var result = await mediator.Send(new UpdateEquipmentGroupCommand(updateEquipmentGroup));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}


	// PATCH api/<EquipmentGroupsController>/5/add
	[Authorize(Roles = "Admin")]
	[HttpPatch("{id}/add")]
	public async Task<IResult> AddEquipment([FromBody] AttachEquipmentRequest request)
	{
		var result = await mediator.Send(new AttachEquipmentCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}

	// PATCH api/<EquipmentGroupsController>/5/remove
	[Authorize(Roles = "Admin")]
	[HttpPatch("{id}/remove")]
	public async Task<IResult> RemoveEquipment([FromBody] DetachEquipmentRequest request)
	{
		var result = await mediator.Send(new DetachEquipmentCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	// DELETE api/<EquipmentGroupsController>/5
	[Authorize(Roles = "Admin")]
	[HttpDelete("{id}")]
	public async Task<IResult> Delete([FromRoute] int id)
	{
		var result = await mediator.Send(new RemoveEquipmentGroupCommand(id));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

}
