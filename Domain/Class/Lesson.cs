using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Lesson
    {
        public int Id { get; set; } // Darsning noyob identifikatori
        public int CourseId { get; set; } // Kurs IDsi
        public Course Course { get; set; } // Kurs obyekti

        public string Title { get; set; } // Dars nomi
        public string Content { get; set; } // Dars kontenti (matn, video, fayllar)
        public DateTime ScheduledAt { get; set; } // Dars vaqti
    }
}
