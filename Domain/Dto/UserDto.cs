using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class UserDto
    {
        public string? FirstName { get; set; } // User's first name
        public string? LastName { get; set; } // User's last name
        public string? Username { get; set; } // User's Telegram username
        public long TelegramId { get; set; } // Unique Chat ID for the user
    }
}
