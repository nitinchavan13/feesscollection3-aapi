using FeesCollection.BusinessLayer.StudentService;
using FeesCollection.BusinessLayer.Utility;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.StudentModels;
using FeesCollection.ResponseModel.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<IHttpActionResult> GetStudents(BaseModel model)
        {
            try
            {
                var data = await _studentService.GetStudents(model.AcademicYearId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/getStudentDetails/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> GetStudentDetails(int id)
        {
            try
            {
                var data = await _studentService.GetStudentDetails(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/addStudent")]
        [HttpPost]
        public async Task<IHttpActionResult> AddStudent()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Unsupported media type");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // Extract form fields
                var formData = provider.Contents
                    .Where(c => string.IsNullOrEmpty(c.Headers.ContentDisposition.FileName))
                    .ToDictionary(c => c.Headers.ContentDisposition.Name.Trim('\"'), async c => await c.ReadAsStringAsync());

                // Extract file
                var fileContent = provider.Contents
                    .FirstOrDefault(c => c.Headers.ContentDisposition.FileName != null);

                string profilePicPath = AppConstants.DEFAULT_STUDENT_PROFILE_PIC;
                if (fileContent != null)
                {
                    var fileBytes = await fileContent.ReadAsByteArrayAsync();
                    var fileName = Guid.NewGuid() + Path.GetExtension(fileContent.Headers.ContentDisposition.FileName.Trim('\"'));
                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/"+ AppConstants.DEFAULT_STUDENT_PROFILE_PIC_PATH+ ""), fileName.ToString());
                    File.WriteAllBytes(filePath, fileBytes);
                    profilePicPath = AppConstants.DEFAULT_STUDENT_PROFILE_PIC_PATH + fileName;
                }

                // Map formData to StudentModel (example)
                var model = new StudentModel
                {
                    FirstName = await formData["firstName"],
                    MiddleName = await formData["middleName"],
                    LastName = await formData["lastName"],
                    MobileNumber = await formData["mobileNumber"],
                    EmailId = await formData["emailId"],
                    Address = await formData["address"],
                    CourseId = int.Parse(await formData["courseId"]),
                    Race = await formData["race"],
                    Cast = await formData["cast"],
                    Gender = await formData["gender"],
                    Birthdate = DateTime.Parse(await formData["birthdate"]),
                    Qualification = await formData["qualification"],
                    AadharNumber = await formData["aadharNumber"],
                    PanNumber = await formData["panNumber"],
                    IsHavingHeavyLicence = bool.Parse(await formData["isHavingHeavyLicence"]),
                    AcademicYearId = int.Parse(await formData["academicYearId"]),
                    UserId = int.Parse(await formData["userId"]),
                    ProfilePic = profilePicPath
                };

                var data = await _studentService.AddStudent(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/editStudent")]
        [HttpPost]
        public async Task<IHttpActionResult> EditStudent(StudentModel model)
        {
            try
            {
                var data = await _studentService.EditStudent(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/deleteStudent/{id}/{academicYearId}")]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteStudent(int id, int academicYearId)
        {
            try
            {
                var data = await _studentService.DeleteStudent(id, academicYearId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/addStudentFees/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> AddStudentFees(int id, StudentFeeModel model)
        {
            try
            {
                var data = await _studentService.AddStudentFees(id, model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("student/editStudentFees/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> EditStudentFees(int id, StudentFeeModel model)
        {
            try
            {
                var data = await _studentService.EditStudentFees(id, model);
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
