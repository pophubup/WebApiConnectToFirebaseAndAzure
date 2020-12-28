using Line.Messaging;
using Line.Messaging.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Utility
{
    public class LineBotApp : WebhookApplication
    {
        private readonly LineMessagingClient _messagingClient;
        public LineBotApp(LineMessagingClient lineMessagingClient)
        {
            _messagingClient = lineMessagingClient;
        }

        protected override async Task OnMessageAsync(MessageEvent ev)
        {
            var result = null as List<ISendMessage>;

            switch (ev.Message)
            {
                //文字訊息
                case TextEventMessage textMessage:
                    {
                        //頻道Id
                        var channelId = ev.Source.Id;
                        //使用者Id
                        var userId = ev.Source.UserId;
                        result = new List<ISendMessage>
                    {
                        new TextMessage($"{userId}")
                    };
                    }
                    break;
            }
            
            if (result != null)
                await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
        }
    }

}
