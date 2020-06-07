using Google.Type;
using Line.Messaging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Repo;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;
        private IProducts _products;
        private readonly LineBotConfig _lineBotConfig;
        private readonly HttpContext _httpContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DefaultController(
            ILogger<DefaultController> logger,
            IProducts products,
            LineBotConfig lineBotConfig,
            IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            _products = products;
            _lineBotConfig = lineBotConfig;
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _httpContext = _httpContextAccessor.HttpContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await _products.GetAllProducts());
        }
        [HttpPost]
        public async Task<IActionResult> GetProducts([FromBody] Product product)
        {
            return Ok(await _products.GetProduct(product));
        }
        [HttpGet]
        public IActionResult GetAllCategory()
        {
            return Ok( _products.GetAllCategory());
        }
        [HttpPost]
        public async Task<IActionResult> PostCreateOrders([FromBody] Order orders)
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
