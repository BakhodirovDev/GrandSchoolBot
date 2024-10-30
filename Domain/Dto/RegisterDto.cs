using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string GroupName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
