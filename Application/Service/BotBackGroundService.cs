using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot;

namespace Application.Service
{
    public class BotBackGroundService : BackgroundService
    {
        private readonly TelegramBotClient _client;
        private readonly IUpdateHandler _handler;
        private readonly ILogger<BotBackGroundService> _logger;

        public BotBackGroundService(ILogger<BotBackGroundService> logger, TelegramBotClient client, IUpdateHandler handler)
        {
            _logger = logger;
            _client = client;
            _handler = handler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bot = await _client.GetMeAsync(stoppingToken);
            _logger.LogInformation($"Bot started successfully {bot.Username}");
            _client.StartReceiving(_handler.HandleUpdateAsync, _handler.HandlePollingErrorAsync, new ReceiverOptions
            {
                ThrowPendingUpdates = true,
            }, stoppingToken);
        }
    }


}
