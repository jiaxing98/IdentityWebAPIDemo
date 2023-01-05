using ProductApi.Dtos;
using ProductApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                ProductDto productDto = await _productRepository.GetProductById(productId);
                return Ok(new ResponseDto
                {
                    Result = productDto,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessage = new List<string> { ex.Message },
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _productRepository.GetProducts();
                return Ok(new ResponseDto
                {
                    Result = productDtos,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessage = new List<string> { ex.Message },
                });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto model = await _productRepository.CreateProduct(productDto);
                return Ok(new ResponseDto
                {
                    Result = model,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessage = new List<string> { ex.Message },
                });
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto model = await _productRepository.UpdateProduct(productDto);
                return Ok(new ResponseDto
                {
                    Result = model,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessage = new List<string> { ex.Message },
                });
            }
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromBody] int productId)
        {
            try
            {
                bool isDeleted = await _productRepository.DeleteProduct(productId);
                return Ok(new ResponseDto
                {
                    Result = isDeleted,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessage = new List<string> { ex.Message },
                });
            }
        }
    }
}
