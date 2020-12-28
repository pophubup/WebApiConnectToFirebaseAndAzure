using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Repo.Service;

namespace WebApplication1.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private IProduct _products;
        public ProductController(ILogger<ProductController> logger,IProduct products)
        {
            _logger = logger;
            _products = products;
        }
        
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok( _products.GetAllProducts());
        }
        [HttpPost]
        public IActionResult GetProducts([FromBody] Product product)
        {
            return Ok( _products.GetProduct(product));
        }
        [HttpGet]
        public IActionResult GetAllCategory()
        {
            return Ok( _products.GetAllCategory());
        }
        [HttpPost]
        public IActionResult PostCreateOrders([FromBody] Order orders)
        {
            return Ok(_products.CreateOrders(orders));
        }
        [HttpGet]
        public IActionResult GetOrders()
        {
            return Ok( _products.GetOrderProducts());
        }
        [HttpPost]
        public IActionResult PostNewProducts([FromBody] List<NewProducts> products)
        {
            return Ok( _products.SaveNewProducts(products)); 
        }
        [HttpPost]
        public  IActionResult PostPic([FromBody]List<Product> products)
        {
            return Ok(_products.SaveImageToBlobstroage(products));
        }


    }
}
