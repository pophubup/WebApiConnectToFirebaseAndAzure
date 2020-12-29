using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using isRock.LineBot;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineBotApiController : ControllerBase
    {
        private readonly LineBotConfig _lineBotConfig;
        public LineBotApiController(LineBotConfig lineBotConfig)
        {
            _lineBotConfig = lineBotConfig;
        }
        [HttpPost]
        public async Task<IActionResult> POST()
        {
            try
            {

                //建立Bot instance
                isRock.LineBot.Bot bot = new isRock.LineBot.Bot(_lineBotConfig.accessToken);  //傳入Channel access token
                string postData = string.Empty;
                //取得 http Post RawData(should be JSO
                using (var reader = new StreamReader(Request.Body))
                {
                    postData = await reader.ReadToEndAsync();
                }
                //剖析JSON
                var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);
                //user這一台裝置LINE，的身分證字號
                var UserID = isRock.LineBot.Utility.Parsing(postData).events[0].source.userId;
                //user資訊(ex:帳號)
                var userInfo = bot.GetUserInfo(ReceivedMessage.events.FirstOrDefault().source.userId);

                //user insert
                var UserSays = ReceivedMessage.events[0].message.text;
                //?
                var ReplyToken = ReceivedMessage.events[0].replyToken;

                //依照用戶說的特定關鍵字來回應
                switch (UserSays.ToLower())
                {
                    case "貼圖":
                        //回覆貼圖
                        bot.ReplyMessage(ReplyToken, 2, 100);
                        // 參考貼圖網址 ==> hhttps://devdocs.line.me/files/sticker_list.pdf
                        break;
                    case "圖片":
                        //回覆圖片
                        //建立actions，作為ButtonTemplate的用戶回覆行為
                        var actions1 = new List<isRock.LineBot.TemplateActionBase>();
                        actions1.Add(new isRock.LineBot.MessageAction() { label = "標題1-文字回覆", text = "回覆文字" });
                        actions1.Add(new isRock.LineBot.UriAction() { label = "標題1-開啟URL", uri = new Uri("http://www.google.com") });
                        actions1.Add(new isRock.LineBot.PostbackAction() { label = "標題1-發生postack", data = "abc=aaa&def=111" });

                        var actions2 = new List<isRock.LineBot.TemplateActionBase>();
                        actions2.Add(new isRock.LineBot.MessageAction() { label = "標題2-文字回覆", text = "回覆文字" });
                        actions2.Add(new isRock.LineBot.UriAction() { label = "標題2-開啟URL", uri = new Uri("http://www.google.com") });
                        actions2.Add(new isRock.LineBot.PostbackAction() { label = "標題2-發生postack", data = "abc=aaa&def=111" });

                        var actions3 = new List<isRock.LineBot.TemplateActionBase>();
                        actions3.Add(new isRock.LineBot.MessageAction() { label = "標題3-文字回覆", text = "回覆文字" });
                        actions3.Add(new isRock.LineBot.UriAction() { label = "標題3-開啟URL", uri = new Uri("http://www.google.com") });
                        actions3.Add(new isRock.LineBot.PostbackAction() { label = "標題3-發生postack", data = "abc=aaa&def=111" });

                        var actions4 = new List<isRock.LineBot.TemplateActionBase>();
                        actions4.Add(new isRock.LineBot.MessageAction() { label = "標題4-文字回覆", text = "回覆文字" });
                        actions4.Add(new isRock.LineBot.UriAction() { label = "標題4-開啟URL", uri = new Uri("http://www.google.com") });
                        actions4.Add(new isRock.LineBot.PostbackAction() { label = "標題4-發生postack", data = "abc=aaa&def=111" });

                        
                        List<Column> c = new List<Column>();

                        c.Add(new Column() { title = "標題1", text = "cccc.", actions = actions1 });
                        c.Add(new Column() { title = "標題2", text = "fd..", actions = actions2 });
                        c.Add(new Column() { title = "標題3", text = "Atr.", actions = actions3 });
                        c.Add(new Column() { title = "標題4", text = "ABC...敘rtrt..", actions = actions4 });

                        var CarouselTemplate = new isRock.LineBot.CarouselTemplate()
                        {
                            columns = c
                        };
                        //發送
                        //bot.PushMessage(UserID, CarouselTemplate);
                        bot.ReplyMessage(_lineBotConfig.accessToken, new isRock.LineBot.TemplateMessage(CarouselTemplate));
                        //bot.ReplyMessage(ReplyToken, new Uri("https://external-tpe1-1.xx.fbcdn.net/safe_image.php?d=AQAc_b9uFr5VR0cg&w=476&h=249&url=fbstaging%3A%2F%2Fgraph.facebook.com%2Fstaging_resources%2FMDExMzQ5NDA3ODM1MDk4Nzg2OjI2MDY5NTAzMQ%3D%3D&cfs=1&upscale=1&_nc_hash=AQDJXtISk8IViKtg"));
                        break;
                    case "hi":
                        //回覆訊息
                        string Message = "hi," + userInfo.displayName + " ~ today will be a amazing day for you. " + UserSays;
                        //回覆用戶
                        bot.ReplyMessage(ReplyToken, Message);
                        break;
                    case "carouseltemplate":
                        //建立actions，作為ButtonTemplate的用戶回覆行為
                        var act1 = new List<isRock.LineBot.TemplateActionBase>();
                        act1.Add(new isRock.LineBot.MessageAction() { label = "標題1-文字回覆", text = "回覆文字" });
                        act1.Add(new isRock.LineBot.UriAction() { label = "標題1-開啟URL", uri = new Uri("http://www.google.com") });
                        act1.Add(new isRock.LineBot.PostbackAction() { label = "標題1-發生postack", data = "abc=aaa&def=111" });

                        var act2 = new List<isRock.LineBot.TemplateActionBase>();
                        act2.Add(new isRock.LineBot.MessageAction() { label = "標題2-文字回覆", text = "回覆文字" });
                        act2.Add(new isRock.LineBot.UriAction() { label = "標題2-開啟URL", uri = new Uri("http://www.google.com") });
                        act2.Add(new isRock.LineBot.PostbackAction() { label = "標題2-發生postack", data = "abc=aaa&def=111" });

                        var act3 = new List<isRock.LineBot.TemplateActionBase>();
                        act3.Add(new isRock.LineBot.MessageAction() { label = "標題3-文字回覆", text = "回覆文字" });
                        act3.Add(new isRock.LineBot.UriAction() { label = "標題3-開啟URL", uri = new Uri("http://www.google.com") });
                        act3.Add(new isRock.LineBot.PostbackAction() { label = "標題3-發生postack", data = "abc=aaa&def=111" });

                        var act4 = new List<isRock.LineBot.TemplateActionBase>();
                        act4.Add(new isRock.LineBot.MessageAction() { label = "標題4-文字回覆", text = "回覆文字" });
                        act4.Add(new isRock.LineBot.UriAction() { label = "標題4-開啟URL", uri = new Uri("http://www.google.com") });
                        act4.Add(new isRock.LineBot.PostbackAction() { label = "標題4-發生postack", data = "abc=aaa&def=111" });


                        List<Column> cc = new List<Column>();

                        cc.Add(new Column() { title = "標題1", text = "ABC...敘述...", thumbnailImageUrl = new Uri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShM4OsAL9Y7-4iGKI0mvP2WsQ3gQeEApOwnjsXdUs90dQe2ph8"), actions = act1 });
                        cc.Add(new Column() { title = "標題2", text = "ABC...敘述...", thumbnailImageUrl = new Uri("https://media.istockphoto.com/photos/book-heart-picture-id503708758?k=6&m=503708758&s=612x612&w=0&h=5_lHyNzJazKgkoyIlDkbMS4s1eFANPqrsqQGY6t8Jwg="), actions = act2 });
                        cc.Add(new Column() { title = "標題3", text = "ABC...敘述...", thumbnailImageUrl = new Uri("https://s3.eu-west-2.amazonaws.com/littlewriter-production/stories/4aKYJcQvuD.jpeg"), actions = act3 });
                        cc.Add(new Column() { title = "標題4", text = "ABC...敘述...", thumbnailImageUrl = new Uri("https://github.com/apple-touch-icon.png/"), actions = act4 });

                        var CarouselTemplate1 = new isRock.LineBot.CarouselTemplate()
                        {
                            columns = cc
                        };
                        //發送
                        //bot.PushMessage(UserID, CarouselTemplate);
                        bot.ReplyMessage(_lineBotConfig.accessToken, new isRock.LineBot.TemplateMessage(CarouselTemplate1));
                        break;
                    case "buttonstemplate":
                        //建立actions，作為ButtonTemplate的用戶回覆行為
                        var actions = new List<isRock.LineBot.TemplateActionBase>();
                        actions.Add(new isRock.LineBot.MessageAction() { label = "標題-文字回覆", text = "回覆文字" });
                        actions.Add(new isRock.LineBot.MessageAction() { label = "標題-文字回覆", text = "回覆文字" });
                        actions.Add(new isRock.LineBot.UriAction() { label = "標題-開啟URL", uri = new Uri("http://www.google.com") });

                        //單一Button Template Message
                        var ButtonTemplate = new isRock.LineBot.ButtonsTemplate()
                        {
                            text = "ButtonsTemplate文字訊息",
                            title = "ButtonsTemplate標題",
                            //設定圖片
                            thumbnailImageUrl = new Uri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShM4OsAL9Y7-4iGKI0mvP2WsQ3gQeEApOwnjsXdUs90dQe2ph8"),
                            actions = actions //設定回覆動作
                        };

                        //發送
                        bot.PushMessage(UserID, ButtonTemplate);
                        //bot.ReplyMessage(ChannelAccessToken , new isRock.LineBot.TemplateMessage(ButtonTemplate));
                        break;
                }

                return Ok(UserSays.ToLower());
            }
            catch (Exception e)
            {
                string err_msg = e.Message;
                return Ok();
            }
        }
    }
}
