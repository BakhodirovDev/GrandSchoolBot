using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Feedback
    {
        public int Id { get; set; } // Fikr identifikatori
        public int CourseId { get; set; } // Kurs IDsi
        public Course Course { get; set; } // Kurs obyekti
        public int UserId { get; set; } // O'quvchi IDsi
        public Users User { get; set; } // O'quvchi obyekti
        public string Comment { get; set; } // Fikr yoki tavsiya
        public int Rating { get; set; } // Baholash (1 dan 5 gacha)
        public DateTime SubmittedAt { get; set; } // Fikrni berish vaqti
    }
}
