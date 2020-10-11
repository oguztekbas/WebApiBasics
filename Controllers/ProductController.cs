using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiBasics.Models;
using WebApiBasics.ORM.Entity;
using WebApiBasics.ORM.Entity;

namespace TryNetCoreWebApi.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly DenemeContext db;

        public ProductController(DenemeContext _db)
        {
            db = _db;
        }

        [Route("getproducts")]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {

            var products = await db.Product
                .Select(i => new DtoGetProduct()
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductPrice = i.ProductPrice,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category.CategoryName
                })
                .ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        [Route("getproduct/{productId}")]
        [HttpGet]
        public async Task<IActionResult> GetProduct(int productId)
        {

            if (productId <= 0)
            {
                return BadRequest(); // 400 Status Kodu
            }

            var product = await db.Product
                .Select(i => new DtoGetProduct()
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductPrice = i.ProductPrice,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category.CategoryName
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [Route("addproduct")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(DtoAddProduct product)
        {
            var addproduct = new Product()
            {
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                CategoryId = product.CategoryId
            };

            await db.Product.AddAsync(addproduct);
            await db.SaveChangesAsync();
            //201 Status Kodu => Eklenen entity yi client'e geri göndermek için.

            return CreatedAtAction("GetProduct", new { productId = addproduct.Id }, addproduct);

        }

        [Route("updateproduct/{productId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(int productId, DtoUpdateProduct product)
        {

            if (productId != product.Id)
            {
                return BadRequest(); // 400 Status Kodu
            }

            var updateProduct = await db.Product.Where(i => i.Id == productId).FirstOrDefaultAsync();

            if (updateProduct == null)
            {
                return NotFound();
            }

            using(var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {

                    updateProduct.ProductName = product.ProductName;
                    updateProduct.ProductPrice = product.ProductPrice;
                    updateProduct.CategoryId = product.CategoryId;

                    await transaction.CommitAsync();

                    return CreatedAtAction("GetProduct", new { productId = updateProduct.Id }, updateProduct);
                }
                catch(Exception e)
                {

                    await transaction.RollbackAsync();

                    return StatusCode(500);

                }
            }



        }

        [Route("deleteproduct/{productId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int productId)

        {

            var deleteProduct = await db.Product.Where(i => i.Id == productId).FirstOrDefaultAsync();

            if (deleteProduct == null)
            {
                return NotFound();
            }

            db.Product.Remove(deleteProduct);
            await db.SaveChangesAsync();

            return NoContent();

            //201 Status Kodu, İşlem Başarılı,Ancak body göndermiyosun ama header bilgileri gönderiyosun.


        }



    }
}