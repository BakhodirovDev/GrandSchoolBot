using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Keyboard
{
    public class InlineKeyboards
    {
        public InlineKeyboardMarkup AuthKeyboard()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🔐 Tizimga kirish", "login"),
                        InlineKeyboardButton.WithCallbackData("🔐 Ro'yxatdan o'tish", "register")
                    }
                });

            return inlineKeyboard;
        }

        public InlineKeyboardMarkup AdminHomeKeyboard()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Admin", "Admin")
                    }
                });

            return inlineKeyboard;
        }

        public InlineKeyboardMarkup TeacherHomeKeyboard()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Teacher", "Teacher")
                    }
                });

            return inlineKeyboard;
        }

        public InlineKeyboardMarkup StudentHomeKeyboard()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Student", "Student")
                    }
                });

            return inlineKeyboard;
        }

        public InlineKeyboardMarkup RegisterCheckInfoKeyboard()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("✅ Tasdiqlash", "register_confirm"),
                        InlineKeyboardButton.WithCallbackData("❌ Bekor qilish", "register_cancel")
                    }
                });

            return inlineKeyboard;
        }

    }
}
