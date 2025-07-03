using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController(IProductRepository repo) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        return Ok(await repo.GetProductsAsync());

    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);
        if (product == null)
            return NotFound("Product not found with the given ID.");
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.AddProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        return BadRequest("Failed to create product.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id || !ProductExists(id))
            return BadRequest("Cannot update product with mismatched ID or non-existing product.");

        repo.UpdateProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest("Failed to update product.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (!ProductExists(id))
            return NotFound("Cannot delete product with non-existing ID.");

        var product = await repo.GetProductByIdAsync(id);
        
        if (product == null) return NotFound();
        repo.DeleteProduct(product);

        if (!await repo.SaveChangesAsync())
            return BadRequest("Failed to delete product.");
        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return repo.ProductExists(id);
    }
}
