using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{

    //GET: api/<UsersController>/userId-string
    [HttpGet("{userId}")]
    public async Task<IResult> GetByUserId(string userId)
    {
        var result = await mediator.Send(new GetUserByUserIdQuery(userId));
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }


    /// <summary>
    ///   Creates a new <see cref="User" />
    /// Only Admin users can create a new user manually.
    /// </summary>
    /// <param name="request"> The <see cref="CreateUserRequest" /> to create.</param>
    /// <returns> The  <see cref="Result{TValue}" /> created.</returns>
    //POST: api/<UsersController>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IResult> Post([FromBody] CreateUserRequest request)
	{
		var result = await mediator.Send(new CreateUserCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


    /// <summary>
    ///  Updates a <see cref="User" />
    ///  The user can update their own user information.
    /// </summary>
    /// <param name="request"> The <see cref="UpdateUserRequest" /> to update.</param>
    /// <returns>  The <see cref="Result{TValue}" /> updated.</returns>
    //PUT: api/<UsersController>/5
    [HttpPut("{id}")]
    public async Task<IResult> Put([FromBody] UpdateUserRequest request)
	{
		var result = await mediator.Send(new UpdateUserCommand(request));
		return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
	}


	//
	/// <summary>
	///   Creates or updates a <see cref="User" />
	/// This method is used to create or update a user when the user authenticates with an external provider.
	///  It would be used to update the user's email address if the user authenticates with a different email address.
	/// </summary>
	/// <param name="request">The <see cref="CreateOrUpdateRequest" /> to create or update.</param>
	/// <returns> The <see cref="Result" /> created or updated.</returns>
	// PATCH: api/<UsersController>
	[HttpPatch]
	public async Task<IResult> CreateOrUpdate([FromBody] CreateOrUpdateRequest request)
	{
		var result = await mediator.Send(new CreateOrUpdateCommand(request));
		return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
	}

	/// <summary>
	///   Deletes a <see cref="User" />
	///  Only Admin users can delete a user.
	/// </summary>
	/// <param name="id"> The user id.</param>
	/// <returns> The  <see cref="Result{TValue}" /> deleted.</returns>
    //DELETE: api/<UsersController>/userId-string
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int  id)
    {
        var result = await mediator.Send(new RemoveUserCommand(id));
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }




}
