using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Course
    {
        public int Id { get; set; } // Kursning noyob identifikatori
        public string Name { get; set; } // Kurs nomi
        public string Description { get; set; } // Kurs tavsifi
        public int TeacherId { get; set; } // O'qituvchi IDsi
        public Users Teacher { get; set; } // O'qituvchi obyekti
        public DateTime CreatedAt { get; set; } // Kursning yaratilish vaqti

        public ICollection<CourseEnrollment> Enrollments { get; set; } // Kursga yozilgan o'quvchilar
    }
}
