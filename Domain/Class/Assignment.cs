using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Assignment
    {
        public int Id { get; set; } // Topshiriqning noyob identifikatori
        public int LessonId { get; set; } // Dars IDsi
        public Lesson Lesson { get; set; } // Dars obyekti
        public string Title { get; set; } // Topshiriq nomi
        public string Description { get; set; } // Topshiriq tavsifi
        public DateTime DueDate { get; set; } // Topshiriqning topshirish vaqti
    }
}
