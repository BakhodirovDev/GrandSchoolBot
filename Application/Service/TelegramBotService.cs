using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;


namespace Application.Service
{
    public class TelegramBotService : IHostedService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly IConfiguration _configuration;
        private TelegramBotClient _botClient;

        public TelegramBotService(ILogger<TelegramBotService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var token = _configuration["TelegramBot:Token"];
            var webhookUrl = _configuration["TelegramBot:WebhookUrl"];

            _botClient = new TelegramBotClient(token);
            await _botClient.SetWebhookAsync(webhookUrl, cancellationToken: cancellationToken);

            var me = await _botClient.GetMeAsync(cancellationToken);
            _logger.LogInformation($"Telegram Bot @{me.Username} ishga tushdi.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Telegram Bot to'xtatildi.");
        }
    }
}
