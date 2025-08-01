using FeesCollection.BusinessLayer.ExpenceService;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.ExpenceModels;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace StudentFeesCollection.web.Controllers
{
    [RoutePrefix("api/v1/internal")]
    public class ExpenceController : ApiController
    {
        public IExpenceService _expenceService { get; set; }
        public ExpenceController()
        {
            _expenceService = new ExpenceService();
        }

        [Route("expences/getExpences")]
        [HttpPost]
        public async Task<IHttpActionResult> GetExpences(BaseModel model)
        {
            try
            {
                var data = await _expenceService.GetExpences(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("expences/addExpence")]
        [HttpPost]
        public async Task<IHttpActionResult> AddExpence(ExpenceModel model)
        {
            try
            {
                var data = await _expenceService.AddExpence(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("expences/editExpence")]
        [HttpPost]
        public async Task<IHttpActionResult> EditExpence(ExpenceModel model)
        {
            try
            {
                var data = await _expenceService.EditExpence(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("expences/deleteExpence")]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteExpence(ExpenceModel model)
        {
            try
            {
                var data = await _expenceService.DeleteExpence(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
