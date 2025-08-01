using FeesCollection.BusinessLayer.AuthService;
using FeesCollection.ResponseModel.AuthModels;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace StudentFeesCollection.web.Controllers
{
    [RoutePrefix("api/v1/internal")]
    public class AuthController : ApiController
    {
        public IAuthService _authService { get; set; }
        public AuthController()
        {
            _authService = new AuthService();
        }

        [Route("auth/login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(AuthModel model)
        {
            try
            {
                var data = await _authService.Login(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("auth/studentLogin")]
        [HttpPost]
        public async Task<IHttpActionResult> StudentLogin(StudentAuthModel model)
        {
            try
            {
                var data = await _authService.StudentLogin(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("auth/academicYears")]
        [HttpPost]
        public async Task<IHttpActionResult> FetchAcademicYears()
        {
            try
            {
                var data = await _authService.FetchAcademicYears();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
