using System;
using System.Linq;
using Cw3.DAL;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;
        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }
        [Route("api/enrollments")]
        [HttpPost]
        public IActionResult AddStudentAndEnroll(EnrollStudentRequest enrollStudent)
        {
            var response = _service.EnrollStudent(enrollStudent);
            if(response.status == "Student dodany")
            {
                return Created(response.status, response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        [Route("api/enrollments/promotions")]
        [HttpPost]
        public IActionResult PromotionStudent(Study study)
        {
            var response = _service.PromoteStudents(study.Semester, study.Studies);
            return Created("Students promoted",response);
        }

    }

}