using System;
using Microsoft.AspNetCore.Mvc;
using Cw3.DAL;
using Cw3.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Cw3.ModelsGenerated;
using System.Linq;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly IStudentDbService _service;

        public IConfiguration Configuration { get; }
        public StudentsController(IDbService dbService, IStudentDbService service,IConfiguration configuration)
        {
            _dbService = dbService;
            _service = service;
            Configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetStudent()
        {
            var db = new s17188Context();
            var res = db.Student.ToList();
            //return Ok(_dbService.GetStudents());
            return Ok(res);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentsEnrollment(string id)
        {
            return Ok(_dbService.GetStudentsEnrollment(id));
        }

        [HttpPost]
        [Route("{id}")]
        public IActionResult UpdateStudent(ModelsGenerated.Student student,string id)
        {
            var db = new s17188Context();
            var s1 = new ModelsGenerated.Student
            {
                IndexNumber = id,
                FirstName = student.FirstName
            };

            db.Attach(s1);
            db.Entry(s1).Property("FirstName").IsModified = true;
            
            db.SaveChanges();
            return Ok(s1);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokonczona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)
        {
            var db = new s17188Context();

            var s = db.Student.Where(s => s.IndexNumber == id).First();
            db.Student.Remove(s);
            db.SaveChanges();
            return Ok("Usuwanie ukonczone");
        }

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            var response = _service.LoginStudent(request.Login, request.Haslo);
            if(response == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(response);
            }
            

        }

        [HttpPost]
        [Route("refreshToken/{token}")]
        public IActionResult RefreshToken(string token)
        {
            var response = _service.RefreshToken(token);
            if (response == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(response);
            }


        }


    }
}