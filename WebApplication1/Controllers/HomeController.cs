using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Utility;
using static WebApplication1.Utility.Helpers;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<BaseModel> controllerActions = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type)).SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).Select(x => 
            {
                int Nameindex = x.DeclaringType.Name.IndexOf("Controller");
                var controllerName = x.DeclaringType.Name.Remove(Nameindex);
                return new BaseModel
                {
                    Controller = controllerName,
                    Action = x.Name,
                    ReturnType = x.ReturnType.Name,
                    Attribute = x.GetCustomAttributes().Select(x => x.GetType().Name).ToList().Where(x => x.Contains("Http")).ToList().Select(x =>
                    {
                        var index = x.IndexOf("A");
                        var result = x.Remove(index);
                        return result;
                    }).FirstOrDefault()
                };
             
            }) ;
           
          
            return View(controllerActions);
        }
        public IActionResult mySignalr()
        {
            
            return View();
        }
        public async Task<ActionResult> getFlaskApi()
        {
            var json = await new HttpClient().GetAsync("https://damp-mountain-73806.herokuapp.com/api/resources/test");
            var data = from s1 in Some(json)
                       .Map(Helpers.parseJsonToStringLinq)
                       .Map(Helpers.parseStringToObjectLinq)
                       .Map(Helpers.stockNameArrangeLinq)
                       select s1.Value;
            return Ok(data);
        }
        public async Task<ActionResult> getFlaskApi_fluentStyle() => 
                         from s2 in await Some(new HttpClient().GetAsync("https://damp-mountain-73806.herokuapp.com/api/resources/test"))
                        .Map(Helpers.parseJsonToStringAsync)
                        .Map(Helpers.parseStringToObjectAsync)
                        .Map(Helpers.stockNameArrangeAsync)
                         select s2.Status switch
                         {
                             OptionStatus.HasValue => (ActionResult)View(s2.Value),
                             OptionStatus.Error => StatusCode(500),
                             _ => NotFound()
                         };

        public IActionResult getFlaskApi_piplineStyle() => Ok(
            new HttpClient().GetAsync("https://damp-mountain-73806.herokuapp.com/api/resources/test")
            .parseJsonToString()
            .parseStringToObject(new StockData())
            .stockNameArrange()
            );
       


    }
}
