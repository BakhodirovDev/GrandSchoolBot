using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Result
    {
        public int Id { get; set; } // Natijalar identifikatori
        public int UserId { get; set; } // O'quvchi IDsi
        public Users User { get; set; } // O'quvchi obyekti
        public int CourseId { get; set; } // Kurs IDsi
        public Course Course { get; set; } // Kurs obyekti
        public double Score { get; set; } // Olingan baho
        public DateTime TakenAt { get; set; } // Imtihon sanasi yoki baholash vaqti
    }
}
