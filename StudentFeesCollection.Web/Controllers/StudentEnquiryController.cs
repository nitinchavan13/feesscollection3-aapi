using FeesCollection.BusinessLayer.StudentEnquiry;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.StudentModels;
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
    public class StudentEnquiryController : ApiController
    {
        public IStudentEnquiryService _studentEnquiryService { get; set; }
        public StudentEnquiryController()
        {
            _studentEnquiryService = new StudentEnquiryService();
        }

        [Route("enquiry/addStudentEnquiry")]
        [HttpPost]
        public async Task<IHttpActionResult> AddStudentEnquiry(StudentEnquiryModel model)
        {
            try
            {
                var data = await _studentEnquiryService.CreateNewEnquiry(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("enquiry/getStudentEnquiry")]
        [HttpPost]
        public async Task<IHttpActionResult> getStudentEnquiry(BaseModel model)
        {
            try
            {
                var data = await _studentEnquiryService.GetAllEnquiries(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
