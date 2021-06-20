using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Commands;

namespace WebApi.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("products")]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            return Ok(await _productRepository.GetAllAsync());
        }

        [HttpGet("products/{id:guid}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            return Ok(await _productRepository.GetByIdAsync(id));
        }

        [HttpPost("products")]
        public async Task<ActionResult<Product>> Create([FromBody] CreateProductCommand command)
        {
            var product = new Product(command.Description, command.Price);
            return Ok(await _productRepository.AddAsync(product));
        }

        [HttpPut("products")]
        public async Task<ActionResult<Product>> Update([FromBody] UpdateProductCommand command)
        {
            var product = await _productRepository.GetByIdAsync(command.Id);
            if (product is null) return BadRequest("Prodcut not found");

            product.Update(command.Description, command.Price);
            await _productRepository.UpdateAsync(product);
            return Ok(product);
        }

        [HttpDelete("products/{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return BadRequest("Prodcut not found");

            await _productRepository.DeleteAsync(product);
            return Ok();
        }
    }
}
