using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;
using Heracles.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Heracles.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentTypesController : ControllerBase
    {
        private readonly IEquipmentService _service;

        public EquipmentTypesController(IEquipmentService service)
        {
            _service = service;
        }
        
        // GET: api/<EquipmentTypesController>
        [HttpGet]
        public async Task<IResult> Get([FromQuery] QueryRequest query)
        {
            var result = await _service.GetAsync(query);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // GET api/<EquipmentTypesController>/5
        [HttpGet("{id}")]
        public  async Task<IResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }

        // POST api/<EquipmentTypesController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public  async Task<IResult> Post([FromBody] Equipment entity)
        {
            var result = await _service.CreateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }


        // PUT api/<EquipmentTypesController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public  async Task<IResult> Put([FromBody] Equipment entity)
        {
            var result = await _service.UpdateAsync(entity);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }


        // DELETE api/<EquipmentTypesController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public  async Task<IResult> Delete([FromRoute] int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Results.Ok( result.Value) : result.ToProblemDetails();
        }
    }
}
