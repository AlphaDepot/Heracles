using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IResult> Get([FromQuery] QueryRequest query)
    {
        var result = await _service.GetAsync(query);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    
    
    //GET: api/<UsersController>/userId-string
    [HttpGet("{userId}")]
    public async Task<IResult> Get(string userId)
    {
        var result = await _service.GetUserByUserIdAsync(userId);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    
    //POST: api/<UsersController>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IResult> Post([FromBody] User entity)
    {
        var result = await _service.CreateUserAsync(entity);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    
    //PUT: api/<UsersController>/5
    [HttpPut("{id}")]
    public async Task<IResult> Put([FromBody] User entity)
    {
        var result = await _service.UpdateUserAsync(entity);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    
    //DELETE: api/<UsersController>/userId-string
    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    public async Task<IResult> Delete(string  userId)
    {
        var result = await _service.DeleteUserAsync(userId);
        return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
    }
    
    
}