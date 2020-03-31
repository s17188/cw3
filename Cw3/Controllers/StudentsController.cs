using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cw3.DAL;
using System.Data.SqlClient;
using Cw3.Models;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudent()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentsEnrollment(string id)
        {
            return Ok(_dbService.GetStudentsEnrollment(id));
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

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

    }
}