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
    //[EnableCors]
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
        public async Task<IActionResult> onMessageReply()
        {
            try
            {

                var events = await HttpContext.Request.GetWebhookEventsAsync("842dbf693e99e5fd75e83f8200250109", "U8e75dde4f4dddc3510f7a37200531788");
                var lineMessagingClient = new LineMessagingClient(_lineBotConfig.accessToken);
                var lineBotApp = new LineBotApp(lineMessagingClient);
                await lineBotApp.RunAsync(events);
                return Ok();
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
            }


            

        }
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            string my = "DateTime : " + DateTime.Now.ToString();
            
            return Ok(my);

        }

        [HttpGet]
        public async Task<IActionResult> Test22()
        {
            string my = "DateTime : " + DateTime.Now.ToString();
           
        
            return Ok(my);

        }
        [HttpGet]
        public async Task<IActionResult> onMessagePushy()
        {
            //get configuration from appsettings.json
            var token = "RG0IloNI+yCgdqoTv5s5V98isIFeS67I1FSdlNs/wEU84X5xfNH6x4jEgznsZ8geonJ+igrHae4L07FkU0IYOa8RjQUJ90OHhrbHXa2cllvDMNICVZkVoeAZkmqLQh3MAz0/FllCM/KXaQ+wgYgrGQdB04t89/1O/w1cDnyilFU=";// _config.GetSection("LINE-Bot-Setting:channelAccessToken");
            var AdminUserId = "U8e75dde4f4dddc3510f7a37200531788";// _config.GetSection("LINE-Bot-Setting:adminUserID");
            var body = ""; //for JSON Body
            //create vot instance
            var bot = new isRock.LineBot.Bot(token);
            isRock.LineBot.MessageBase responseMsg = null;
            //message collection for response multi-message 
            List<isRock.LineBot.MessageBase> responseMsgs =
                new List<isRock.LineBot.MessageBase>();

            try
            {

                //get JSON Body
                using (StreamReader reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8))
                {
                    body = reader.ReadToEndAsync().Result;
                }
                bot.PushMessage(AdminUserId, "Exception : \n" + body);
                //parsing JSON
                var ReceivedMessage = isRock.LineBot.Utility.Parsing(body);
                //Get LINE Event
                var LineEvent = ReceivedMessage.events.FirstOrDefault();
                //prepare reply message
                if (LineEvent.type.ToLower() == "message")
                {
                    switch (LineEvent.message.type.ToLower())
                    {
                        case "text":
                            //add text response
                            responseMsg =
                                new isRock.LineBot.TextMessage($"you said : {LineEvent.message.text}");
                            responseMsgs.Add(responseMsg);
                            //add ButtonsTemplate if user say "/Show ButtonsTemplate"
                            if (LineEvent.message.text.ToLower().Contains("/show buttonstemplate"))
                            {
                                //define actions
                                var act1 = new isRock.LineBot.MessageAction()
                                { text = "test action1", label = "test action1" };
                                var act2 = new isRock.LineBot.MessageAction()
                                { text = "test action2", label = "test action2" };

                                var tmp = new isRock.LineBot.ButtonsTemplate()
                                {
                                    text = "Button Template text",
                                    title = "Button Template title",
                                    thumbnailImageUrl = new Uri("https://i.imgur.com/wVpGCoP.png"),
                                };

                                tmp.actions.Add(act1);
                                tmp.actions.Add(act2);
                                //add TemplateMessage into responseMsgs
                                responseMsgs.Add(new isRock.LineBot.TemplateMessage(tmp));
                            }
                            break;
                        case "sticker":
                            responseMsg =
                            new isRock.LineBot.StickerMessage(1, 2);
                            responseMsgs.Add(responseMsg);
                            break;
                        default:
                            responseMsg = new isRock.LineBot.TextMessage($"None handled message type : { LineEvent.message.type}");
                            responseMsgs.Add(responseMsg);
                            break;
                    }
                }
                else
                {
                    responseMsg = new isRock.LineBot.TextMessage($"None handled event type : { LineEvent.type}");
                    responseMsgs.Add(responseMsg);
                }

                //回覆訊息
                bot.ReplyMessage(LineEvent.replyToken, responseMsgs);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //如果有錯誤，push給admin
                bot.PushMessage(AdminUserId, "Exception : \n" + ex.Message);
                //response OK
                return Ok();
            }
        }

    }
}
