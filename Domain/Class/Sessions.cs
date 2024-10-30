using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Sessions
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public long TelegramId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ExpiredAt { get; set; }


    }
}
