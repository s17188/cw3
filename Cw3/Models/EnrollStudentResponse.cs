using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Models
{
    public class EnrollStudentResponse
    {
        public string status { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public Enrollment enrollment { get; set; }

    }
}
