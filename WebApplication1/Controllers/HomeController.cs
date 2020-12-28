using Microsoft.AspNetCore.Mvc;
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
        public IActionResult mySignalr()
        {
            return View();
        }

    }
}
