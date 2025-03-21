using FeesCollection.BusinessLayer.StudentService;
using FeesCollection.BusinessLayer.Utility;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.StudentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StudentFeesCollection.web.Controllers
{
    [RoutePrefix("api/v1/internal")]
    public class StudentController : ApiController
    {
        public IStudentService _studentService { get; set; }
        public StudentController()
        {
            _studentService = new StudentService();
        }

        [Route("student/getStudents")]
        [HttpPost]
        public IHttpActionResult GetStudents(BaseModel model)
        {
            try
            {
                var data = _studentService.GetStudents(model.AcademicYearId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/getStudentDetails/{id}")]
        [HttpPost]
        public IHttpActionResult GetStudentDetails(int id)
        {
            try
            {
                var data = _studentService.GetStudentDetails(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/addStudent")]
        [HttpPost]
        public IHttpActionResult AddStudent(StudentModel model)
        {
            try
            {
                var data = _studentService.AddStudent(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/editStudent")]
        [HttpPost]
        public IHttpActionResult EditStudent(StudentModel model)
        {
            try
            {
                var data = _studentService.EditStudent(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/deleteStudent/{id}/{academicYearId}")]
        [HttpPost]
        public IHttpActionResult DeleteStudent(int id, int academicYearId)
        {
            try
            {
                var data = _studentService.DeleteStudent(id, academicYearId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/addStudentFees/{id}")]
        [HttpPost]
        public IHttpActionResult AddStudentFees(int id, StudentFeeModel model)
        {
            try
            {
                var data = _studentService.AddStudentFees(id, model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/editStudentFees/{id}")]
        [HttpPost]
        public IHttpActionResult EditStudentFees(int id, StudentFeeModel model)
        {
            try
            {
                var data = _studentService.EditStudentFees(id, model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/sendSms")]
        [HttpPost]
        public IHttpActionResult SendSms()
        {
            try
            {
                var message = "Thank you to enrolling in course of Diploma In Fire and Safety with MFNSCR. Your reference id is MFNSCR00001. Please save it for future reference.";
                var data = SMSUtility.SendSMS(message, "917020968801");
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
