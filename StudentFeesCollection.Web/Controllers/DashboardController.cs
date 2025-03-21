using FeesCollection.BusinessLayer.DashboardService;
using FeesCollection.ResponseModel.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StudentFeesCollection.web.Controllers
{
    [RoutePrefix("api/v1/internal")]
    public class DashboardController : ApiController
    {
        public IDashboardService _dashboardService { get; set; }
        public DashboardController()
        {
            _dashboardService = new DashboardService();
        }

        [Route("dashboard/getDashboardCards")]
        [HttpPost]
        public IHttpActionResult GetDashboardCards(BaseModel model)
        {
            try
            {
                var data = _dashboardService.GetDashboardCards(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
