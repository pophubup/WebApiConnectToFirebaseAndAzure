using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    [EnableCors]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WebCrawlingController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> getFlaskApi()
        {
            var json = await new HttpClient().GetAsync("https://damp-mountain-73806.herokuapp.com/api/resources/test");
            var data = from s1 in WebApplication1.Utility.Helpers.Some(json)
                       .Map(Helpers.parseJsonToStringLinq)
                       .Map(Helpers.parseStringToObjectLinq)
                       .Map(Helpers.stockNameArrangeLinq)
                       select s1.Value;
            return Ok(data);
        }
        [HttpGet]
        public async Task<ActionResult> getFlaskApi_fluentStyle() =>
                         from s2 in await WebApplication1.Utility.Helpers.Some(new HttpClient().GetAsync("https://damp-mountain-73806.herokuapp.com/api/resources/test"))
                        .Map(Helpers.parseJsonToStringAsync)
                        .Map(Helpers.parseStringToObjectAsync)
                        .Map(Helpers.stockNameArrangeAsync)
                         select s2.Status switch
                         {
                             OptionStatus.HasValue => (ActionResult)Ok(s2.Value),
                             OptionStatus.Error => StatusCode(500),
                             _ => NotFound()
                         };
        [HttpGet]
        public IActionResult getFlaskApi_piplineStyle() => Ok(
            new HttpClient().GetAsync("https://damp-mountain-73806.herokuapp.com/api/resources/test")
            .parseJsonToString()
            .parseStringToObject(new StockData())
            .stockNameArrange()
            );

    }
}
