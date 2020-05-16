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
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentsEnrollment(string id)
        {
            return Ok(_dbService.GetStudentsEnrollment(id));
        }

        //[HttpPost]
        //public IActionResult CreateStudent(Student student)
        //{
        //    student.IndexNumber = $"s{new Random().Next(1, 20000)}";
        //    return Ok(student);
        //}

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokonczona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
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