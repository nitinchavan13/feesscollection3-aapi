using FeesCollection.BusinessLayer.ExamService;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.StudentExamModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StudentFeesCollection.Web.Controllers
{
    [RoutePrefix("api/v1/internal")]
    public class StudentExamController : ApiController
    {
        public IStudentExamService _examService { get; set; }
        public StudentExamController()
        {
            _examService = new StudentExamService();
        }

        [Route("studExam/getExamsForStudent")]
        [HttpPost]
        public async Task<IHttpActionResult> GetExams(BaseModel model)
        {
            try
            {
                var data = await _examService.FetchMyTodaysExams(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("studExam/getExamQuestions/{examId}")]
        [HttpPost]
        public async Task<IHttpActionResult> GetExamQuestions(int examId)
        {
            try
            {
                var data = await _examService.FetchQuestions(examId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("studExam/updateAttempt/{examId}")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateAttempt(int examId, BaseModel model)
        {
            try
            {
                var data = await _examService.UpdateExamAttendance(model.UserId, examId);
                return Ok(new { id = data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("studExam/saveExamAnswers")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveExamAnswers(StudentExamResponse model)
        {
            try
            {
                var data = await _examService.SaveExamAnswers(model);
                return Ok(new { id = data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
