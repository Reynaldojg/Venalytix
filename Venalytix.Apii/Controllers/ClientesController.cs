using Microsoft.AspNetCore.Mvc;
using Venalytix.Apication.DTOS.ClientesDTO;
using Venalytix.Apication.Interfaces.IBase;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IRepositoryBase<SaveClienteDTO, UpdateClienteDTO, ObtenerClienteDTO> _repository;

        public ClientesController(IRepositoryBase<SaveClienteDTO, UpdateClienteDTO, ObtenerClienteDTO> repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            OperationResult result = await _repository.ObtenerTodosAsync();

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

     
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorIdAsync(int id)
        {
            OperationResult result = await _repository.ObtenerPorAsync(id);

            if (result.IsSuccess)
                return Ok(result);
            else
                return NotFound(result);
        }

     
        [HttpPost]
        public async Task<IActionResult> AgregarAsync([FromBody] SaveClienteDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            OperationResult result = await _repository.AgregarAsync(dto);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

     
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarAsync(int id, [FromBody] UpdateClienteDTO dto)
        {
            if (id != dto.IdCliente)
                return BadRequest("El ID del cliente no coincide.");

            OperationResult result = await _repository.ActualizarAsync(dto);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

  
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAsync(int id)
        {
            OperationResult result = await _repository.EliminarAsync(id);

            if (result.IsSuccess)
                return Ok(result);
            else
                return NotFound(result);
        }
    }
}
