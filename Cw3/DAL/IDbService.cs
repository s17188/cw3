using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;

namespace Cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public IEnumerable<Enrollment> GetStudentsEnrollment(string id);
    }
}