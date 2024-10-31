using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Application.Keyboard;
using Application.Service.Auth;
using Domain.Dto;
using Microsoft.Extensions.DependencyInjection;
using Application.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Application.Service
{
    public partial class BotUpdateHandler : IUpdateHandler
    {
        private readonly ILogger<BotUpdateHandler> _logger;
        private readonly InlineKeyboards _inlineKeyboard;
        private readonly IServiceProvider _serviceProvider;

        private Message user_input_message;

        // Dictionary to store user states
        private static readonly Dictionary<long, string> userStates = new();
        private static readonly Dictionary<long, RegisterDto> registerUsers = new();
        private static readonly Dictionary<long, LoginDto> loginUser = new();
        private static readonly Dictionary<long, List<int>> userMessageIds = new();

        public BotUpdateHandler(ILogger<BotUpdateHandler> logger, InlineKeyboards inlineKeyboard, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _inlineKeyboard = inlineKeyboard;
            _serviceProvider = serviceProvider;
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError("Update error: {exception.Message}", exception.Message);
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update == null) return;

            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await HandleMessageAsync(botClient, update.Message, cancellationToken);
                        break;
                    case UpdateType.EditedMessage:
                        await HandleEditedMessageAsync(botClient, update.EditedMessage, cancellationToken);
                        break;
                    case UpdateType.CallbackQuery:
                        await HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken);
                        break;
                    default:
                        _logger.LogInformation("Unknown update type: {update.Type}", update.Type);
                        break;
                }
            }
             catch (Exception ex)
            {
                await HandlePollingErrorAsync(botClient, ex, cancellationToken);
            }
        }

        // Helper method for deleting messages
        private async Task DeleteMessagesAsync(ITelegramBotClient botClient, long chatId, params int[] messageIds)
        {
            foreach (var messageId in messageIds)
            {
                await botClient.DeleteMessageAsync(chatId, messageId);
            }
        }

        // Helper method for sending messages
        private async Task SendMessageAsync(ITelegramBotClient botClient, long chatId, string message, IReplyMarkup replyMarkup = null)
        {
            var sentMessage = await botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
            if (!userMessageIds.ContainsKey(chatId))
            {
                userMessageIds[chatId] = new List<int>();
            }
            userMessageIds[chatId].Add(sentMessage.MessageId);
        }

        public async Task HandleMessageAsync(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
        {

            if (message == null) return;

            var chatId = message.Chat.Id;
            user_input_message = message;
            if (message.Text?.ToLower() == "/start")
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    var isLogin = await authService.IsLoginAsync(chatId);

                    if (isLogin.Success)
                    {
                        if (isLogin.Data.Role == Domain.Class.Role.Admin)
                        {
                            await SendMessageAsync(botClient, chatId, "Siz tizimga muvaffaqiyatli kirdingiz!", _inlineKeyboard.AdminHomeKeyboard());
                        }
                        else if (isLogin.Data.Role == Domain.Class.Role.Teacher)
                        {
                            await SendMessageAsync(botClient, chatId, "Siz tizimga muvaffaqiyatli kirdingiz!", _inlineKeyboard.TeacherHomeKeyboard());
                        }
                        else if (isLogin.Data.Role == Domain.Class.Role.Student)
                        {
                            await SendMessageAsync(botClient, chatId, "Siz tizimga muvaffaqiyatli kirdingiz!", _inlineKeyboard.StudentHomeKeyboard());
                        }

                    }
                    else
                    {
                        await StartCommand(botClient, chatId, message.MessageId, message.From?.FirstName, message.From?.LastName);
                    }
                }
                
            }
            else if (message.Text?.ToLower() == "/soat")
            {
                await SendMessageAsync(botClient, chatId, "Server vaqti: " + DateTime.Now.ToString("HH:mm:ss"), replyMarkup: new ReplyKeyboardRemove());
            }
            else if (userStates.TryGetValue(chatId, out var state))
            {
                await HandleUserStateAsync(botClient, chatId, message.MessageId, state, message.Text);
            }
            else
            {
                await HandleUnknownCommandAsync(botClient, chatId, message.MessageId);
            }
        }

        public async Task HandleEditedMessageAsync(ITelegramBotClient botClient, Message? editedMessage, CancellationToken cancellationToken)
        {
            if (editedMessage == null) return;

            var chatId = editedMessage.Chat.Id;

            // Here you can handle the edited message logic, for example:
            await SendMessageAsync(botClient, chatId, "Edited message received: " + editedMessage.Text);
        }



        private async Task StartCommand(ITelegramBotClient botClient, long chatId, int messageId, string firstName, string lastName)
        {
            await DeleteMessagesAsync(botClient, chatId, messageId);
            await SendMessageAsync(botClient, chatId, $"Assalomu Alaykum, {firstName ?? lastName}\nBotdan foydalanish uchun tizimga kirishingiz yoki ro'yxatdan o'tishingiz so'raladi.", _inlineKeyboard.AuthKeyboard());
        }

        private async Task HandleUserStateAsync(ITelegramBotClient botClient, long chatId, int messageId, string state, string userInput)
        {
            switch (state)
            {
                case "awaiting_username":
                    await HandleUsernameStateAsync(botClient, chatId, messageId, userInput);
                    break;
                case "awaiting_password":
                    await HandlePasswordStateAsync(botClient, chatId, messageId, userInput);
                    break;
                case "awaiting_registration_fullname":
                    await HandleRegistrationFullNameAsync(botClient, chatId, messageId, userInput);
                    break;
                case "awaiting_registration_GroupName":
                    await HandleRegistrationGroupNameAsync(botClient, chatId, messageId, userInput);
                    break;
                case "awaiting_registration_phone_number":
                    await HandleRegistrationPhoneNumberAsync(botClient, chatId, user_input_message);
                    break;
                case "awaiting_registration_Username":
                    await HandleRegistrationUsernameAsync(botClient, chatId, messageId, userInput);
                    break;
                case "awaiting_registration_Password":
                    await HandleRegistrationPasswordAsync(botClient, chatId, messageId, userInput);
                    break;
                
                default:
                    await HandleUnknownStateAsync(botClient, chatId, messageId);
                    break;
            }
        }

        

        private async Task HandleUsernameStateAsync(ITelegramBotClient botClient, long chatId, int messageId, string usernameInput)
        {
            loginUser[chatId].Username = usernameInput;

            userStates[chatId] = "awaiting_password";
            await SendMessageAsync(botClient, chatId, "Iltimos, parolingizni kiriting:");
        }

        private async Task HandlePasswordStateAsync(ITelegramBotClient botClient, long chatId, int messageId, string password)
        {
            loginUser[chatId].Password = password;

            var user = loginUser[chatId];

            using (var scope = _serviceProvider.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                var result = await authService.LoginAsync(user,chatId);

                if (result.Success)
                {
                    await DeleteMessagesAsync(botClient, chatId, messageId);
                    userStates.Remove(chatId); // Clear user state
                    loginUser.Remove(chatId);

                    if (result.Success)
                    {
                        if (result.Data.Role == Domain.Class.Role.Admin)
                        {
                            await SendMessageAsync(botClient, chatId, "Siz tizimga muvaffaqiyatli kirdingiz!", _inlineKeyboard.AdminHomeKeyboard());
                        }
                        else if (result.Data.Role == Domain.Class.Role.Teacher)
                        {
                            await SendMessageAsync(botClient, chatId, "Siz tizimga muvaffaqiyatli kirdingiz!", _inlineKeyboard.TeacherHomeKeyboard());
                        }
                        else if (result.Data.Role == Domain.Class.Role.Student)
                        {
                            await SendMessageAsync(botClient, chatId, "Siz tizimga muvaffaqiyatli kirdingiz!", _inlineKeyboard.StudentHomeKeyboard());
                        }

                    }
                }
                else
                {
                    await SendMessageAsync(botClient, chatId, "Username yoki parol noto'g'ri. Iltimos, qayta urinib ko'ring.");
                }
            }
        }

        private async Task HandleUnknownStateAsync(ITelegramBotClient botClient, long chatId, int messageId)
        {
            await DeleteMessagesAsync(botClient, chatId, messageId);
            await SendMessageAsync(botClient, chatId, "Noto'g'ri buyruq. Tizimga kirish yoki ro'yxatdan o'tishni tanlang.", _inlineKeyboard.AuthKeyboard());
        }

        private async Task HandleUnknownCommandAsync(ITelegramBotClient botClient, long chatId, int messageId)
        {
            await DeleteMessagesAsync(botClient, chatId, messageId);
            await SendMessageAsync(botClient, chatId, "Noto'g'ri buyruq. Tizimga kirish yoki ro'yxatdan o'tishni tanlang.", _inlineKeyboard.AuthKeyboard());
        }

        public async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery == null) return;

            var chatId = callbackQuery.Message.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;

            switch (callbackQuery.Data)
            {
                case "login":
                    await HandleLoginCallbackAsync(botClient, chatId, messageId);
                    break;
                case "register":
                    await HandleRegisterCallbackAsync(botClient, chatId, messageId);
                    break;
                case "register_confirm":
                    await HandleRegisterConfirmAsync(botClient, chatId, messageId);
                    break;
                case "register_cancel":
                    await HandleRegisterCancelAsync(botClient, chatId, messageId);
                    break;
                default:
                    await SendMessageAsync(botClient, chatId, "Noto'g'ri tugma");
                    break;
            }
        }

        private async Task HandleLoginCallbackAsync(ITelegramBotClient botClient, long chatId, int messageId)
        {
            loginUser[chatId] = new LoginDto();
            await DeleteMessagesAsync(botClient, chatId, messageId);
            userStates[chatId] = "awaiting_username"; // Start login process
            await SendMessageAsync(botClient, chatId, "Iltimos, username kiriting:");
        }

        private async Task HandleRegisterCallbackAsync(ITelegramBotClient botClient, long chatId, int messageId)
        {
            registerUsers[chatId] = new RegisterDto();
            await DeleteMessagesAsync(botClient, chatId, messageId);
            userStates[chatId] = "awaiting_registration_fullname"; // Start registration process
            await SendMessageAsync(botClient, chatId, "Iltimos, to'liq ismingizni kiriting:");
        }
        private async Task HandleRegisterConfirmAsync(ITelegramBotClient botClient, long chatId, int messageId)
        {
            if (userStates[chatId] == "register_info_check")
            {
                await DeleteMessagesAsync(botClient, chatId, messageId);
                await SendMessageAsync(botClient, chatId, "Iltimos Kuting ....");

                using(var scope = _serviceProvider.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    var result = await authService.RegisterAsync(registerUsers[chatId]);

                    if (result.Success)
                    {
                        userStates.Remove(chatId);
                        registerUsers.Remove(chatId);
                        await DeleteMessagesAsync(botClient, chatId, messageId);
                        await SendMessageAsync(botClient, chatId, "Ro'yxatdan o'tingiz! ✅");
                    }
                    else
                    {
                        await DeleteMessagesAsync(botClient, chatId, messageId);
                        await SendMessageAsync(botClient, chatId, "Ro'yxatdan o'tishda xatolik yuz berdi. Iltimos, qayta urinib ko'ring.", _inlineKeyboard.AuthKeyboard());
                    }
                }
            }
            
        }
        private async Task HandleRegisterCancelAsync(ITelegramBotClient botClient, long chatId, int messageId)
        {
            if (userStates[chatId] == "register_info_check")
            {
                await DeleteMessagesAsync(botClient, chatId, messageId);
                userStates.Remove(chatId);
                registerUsers.Remove(chatId);

                await SendMessageAsync(botClient, chatId, "Ro'yxatdan o'tish bekor qilindi! ❌", _inlineKeyboard.AuthKeyboard());
            }
        }
        private async Task HandleRegistrationFullNameAsync(ITelegramBotClient botClient, long chatId, int messageId, string inputMessage)
        {
            registerUsers[chatId].FullName = inputMessage;
            await DeleteMessagesAsync(botClient, chatId, messageId);
            userStates[chatId] = "awaiting_registration_GroupName"; // Start registration process
            await SendMessageAsync(botClient, chatId, "Iltimos, Gruhingiz nomingizni kiriting: \nNamnuna: \n1. Teacher Name 1\n2. Teacher Name 2\n...");
        }

        private async Task HandleRegistrationGroupNameAsync(ITelegramBotClient botClient, long chatId, int messageId, string inputMessage)
        {
            registerUsers[chatId].GroupName = inputMessage;
            await DeleteMessagesAsync(botClient, chatId, messageId);
            userStates[chatId] = "awaiting_registration_phone_number";

            var requestContactButton = new KeyboardButton("Telefon raqamni yuborish") { RequestContact = true };
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[] { requestContactButton }) { ResizeKeyboard = true };

            await SendMessageAsync(botClient, chatId, "Iltimos, telefon raqamingizni yuboring.", replyKeyboardMarkup);

        }

        private async Task HandleRegistrationPhoneNumberAsync(ITelegramBotClient botClient, long chatId, Message message)
        {
            if (message?.Contact != null) // Agar foydalanuvchi telefon raqamini yuborgan bo'lsa
            {
                registerUsers[chatId].PhoneNumber = message.Contact.PhoneNumber;
                await DeleteMessagesAsync(botClient, chatId, message.MessageId);
                userStates[chatId] = "awaiting_registration_Username"; // Start registration process
                await SendMessageAsync(botClient, chatId, "Iltimos, Username (Foydalanuvchi) nomingizni kiriting:", replyMarkup: new ReplyKeyboardRemove());

            }
            else
            {
                await DeleteMessagesAsync(botClient, chatId, message.MessageId); // Oldingi xabarni o'chirish

                // Agar foydalanuvchi telefon raqamini yubormasa
                await botClient.SendTextMessageAsync(chatId, "Iltimos,\"Telefon raqamni yuborish\" tugmasini bosing. 👇");
            }
        }

        private async Task HandleRegistrationUsernameAsync(ITelegramBotClient botClient, long chatId, int messageId, string inputMessage)
        {
            registerUsers[chatId].Username = inputMessage;
            await DeleteMessagesAsync(botClient, chatId, messageId);
            userStates[chatId] = "awaiting_registration_Password";

            await SendMessageAsync(botClient, chatId, "Iltimos, parolingizni kiriting:");

        }

        private async Task HandleRegistrationPasswordAsync(ITelegramBotClient botClient, long chatId, int messageId, string inputMessage)
        {
            registerUsers[chatId].Password = inputMessage;
            await DeleteMessagesAsync(botClient, chatId, messageId);
            userStates[chatId] = "register_info_check";

            var user = registerUsers[chatId];

            await SendMessageAsync(botClient, chatId, $"📄Ma'lumotlaringizni tekshiring:\n1️⃣1.Ismingiz: {user.FullName}\n2️⃣2.Gruhingiz: {user.GroupName}\n3️⃣3.Telefon raqamingiz: {user.PhoneNumber}\n4️⃣4.Parolingiz: {user.Password}", _inlineKeyboard.RegisterCheckInfoKeyboard());

        }

    }
}
