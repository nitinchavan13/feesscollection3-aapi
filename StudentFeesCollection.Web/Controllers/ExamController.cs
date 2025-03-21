using FeesCollection.BusinessLayer.ExamService;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.ExamModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StudentFeesCollection.Web.Controllers
{
    [RoutePrefix("api/v1/internal")]
    public class ExamController : ApiController
    {
        public IExamService _examService { get; set; }
        public ExamController()
        {
            _examService = new ExamService();
        }

        [Route("exam/getExamsForAdmin")]
        [HttpPost]
        public IHttpActionResult GetAllExams(BaseModel model)
        {
            try
            {
                var data = _examService.GetAllExams(model.AcademicYearId, model.UserId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("exam/getExamDetailsForAdmin/{examId}")]
        [HttpPost]
        public IHttpActionResult GetExamDetails(int examId)
        {
            try
            {
                var data = _examService.GetExamDetails(examId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("exam/getExamQuestions/{examId}")]
        [HttpPost]
        public IHttpActionResult CreateExam(int examId)
        {
            try
            {
                var data = _examService.GetAllExamQuestion(examId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("exam/createNewExam")]
        [HttpPost]
        public IHttpActionResult CreateExam(ExamModel model)
        {
            try
            {
                var data = _examService.CreateNewExam(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("exam/createQuestion/{examId}")]
        [HttpPost]
        public IHttpActionResult CreateQuestion(int examId, QuestionModel model)
        {
            try
            {
                var data = _examService.CreateQuestion(model, examId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
