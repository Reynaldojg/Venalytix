using Microsoft.AspNetCore.Mvc;
using Venalytix.Apication.DTOS.ProductosDTO;
using Venalytix.Apication.Interfaces.IBase;

namespace Venalytix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IRepositoryBase<SaveProductoDTO, UpdateProductoDTO, ObtenerProductoDTO> _productoRepo;

        public ProductosController(IRepositoryBase<SaveProductoDTO, UpdateProductoDTO, ObtenerProductoDTO> productoRepo)
        {
            _productoRepo = productoRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productoRepo.ObtenerTodosAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productoRepo.ObtenerPorAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveProductoDTO dto)
        {
            var result = await _productoRepo.AgregarAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateProductoDTO dto)
        {
            var result = await _productoRepo.ActualizarAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productoRepo.EliminarAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
