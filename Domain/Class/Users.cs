using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Users
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Foydalanuvchining noyob identifikatori
        public string FullName { get; set; } // Ismi
        public string Username { get; set; } // Foydalanuvchi nomi
        public string Password { get; set; } // Parol
        public string PhoneNumber { get; set; } // Elektron pochta
        public Role Role { get; set; } // Foydalanuvchining roli (O'quvchi, O'qituvchi, Admin)
        public DateTime CreatedAt { get; set; } // Yaratilish vaqti
        public DateTime? UpdateDate { get; set; }
        public long TelegramId { get; set; }
        public long ChatId { get; set; } // Unique Chat ID for the user

        public ICollection<CourseEnrollment> Enrollments { get; set; } // Kurslarga yozilishlar
    }

    public enum Role
    {
        Student,
        Teacher,
        Admin
    }
}
