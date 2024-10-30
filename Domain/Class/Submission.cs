using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Class
{
    public class Submission
    {
        public int Id { get; set; } // Topshiriqning noyob identifikatori
        public int AssignmentId { get; set; } // Topshiriq IDsi
        public Assignment Assignment { get; set; } // Topshiriq obyekti
        public int UserId { get; set; } // O'quvchi IDsi
        public Users User { get; set; } // O'quvchi obyekti
        public DateTime SubmittedAt { get; set; } // Topshiriqni topshirish vaqti
        public string Content { get; set; } // Topshiriqni o'zida saqlash (fayl, matn)
    }
}
