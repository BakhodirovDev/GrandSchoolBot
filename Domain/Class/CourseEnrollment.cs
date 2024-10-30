using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class CourseEnrollment
    {
        public int Id { get; set; } // Yozilishning noyob identifikatori
        public int CourseId { get; set; } // Kurs IDsi
        public Course Course { get; set; } // Kurs obyekti
        public int UserId { get; set; } // Foydalanuvchi (o'quvchi) IDsi
        public Users User { get; set; } // O'quvchi obyekti
        public DateTime EnrolledAt { get; set; } // Yozilish vaqti
    }
}
