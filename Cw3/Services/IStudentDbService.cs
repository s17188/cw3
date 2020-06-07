using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        ModelsGenerated.Enrollment PromoteStudents(int semester, string studies);
        Student GetStudent(string index);
        LoginResponse LoginStudent(string login, string haslo);
        LoginResponse RefreshToken(string refToken);
    }
}
