using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

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
    }
}
