using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Repo;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;
        private IProducts _products;
        private IWebHostEnvironment _hostingEnvironment;
        public DefaultController(ILogger<DefaultController> logger, IWebHostEnvironment hostingEnvironment, IProducts products)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _products = products;

        }
        
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await _products.GetAllProducts());
        }
        [HttpGet("GetProducts/{productID}")]
        public async Task<IActionResult> GetProducts(string productID)
        {
            return Ok(await _products.GetProduct(productID));
        }
        [HttpGet]
        public IActionResult GetAllCategory()
        {
            return Ok( _products.GetAllCategory());
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrders([FromBody] Order orders)
        {
            return Ok(await _products.CreateOrders(orders));
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            return Ok(await _products.GetOrderProducts());
        }
        [HttpPost]
        public async Task<IActionResult> PostNewProducts([FromBody] List<NewProducts> products)
        {
            return Ok(await _products.SaveNewProducts(products)); 
        }
        [HttpPost]
        public async Task<IActionResult> PostPic([FromBody]List<Product> products)
        {
            return Ok(await _products.SaveImageToBlobstroage(products));
        }
        
    }
}
