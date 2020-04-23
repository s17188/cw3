using System;
using System.Linq;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        [HttpPost]
        public IActionResult AddStudentAndEnroll(EnrollStudent enrollStudent)
        {
            var response = _dbService.AddNewStudentAndEnroll(enrollStudent);
            if(response.FirstOrDefault()?.status == "Ok")
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
            //return Ok(_dbService.AddNewStudentAndEnroll(enrollStudent));
        }
    }
}