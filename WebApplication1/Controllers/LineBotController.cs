﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Line.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    public class LineBotController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _httpContext;
        private readonly LineBotConfig _lineBotConfig;
        public readonly ILogger _logger;

        public LineBotController(IServiceProvider serviceProvider, LineBotConfig lineBotConfig, ILogger<LineBotController> logger)
        {
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _httpContext = _httpContextAccessor.HttpContext;
            _lineBotConfig = lineBotConfig;
            _logger = logger;
        }
      
        public IActionResult onMessagePushy()
        {
            //get configuration from appsettings.json
            var token = _lineBotConfig.accessToken; 
            var AdminUserId = _lineBotConfig.user_ID;
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
