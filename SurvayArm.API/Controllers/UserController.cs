using log4net;
using SurvayArm.Utility;
using System;
using System.Net;
using System.Web.Http;

namespace SurvayArm.API.Controllers
{
    [RoutePrefix("api/User")]
    [Authorize]
    public class UserController : ApiController
    {
        private readonly ILog _log;

        public UserController()
        {
            _log = LogManager.GetLogger(typeof(UserController));
        }

        [Route("GetLoggedUserInfo")]
        public IHttpActionResult GetLoggedUserInfo()
        {
            try
            {
              return  Ok(new
                {
                    UserName = User.Identity.Name,
                    FullName = User.Identity.GetUserFullName(),
                    MobileNumber = User.Identity.GetUserMobileNumber(),
                    Address = User.Identity.GetUserAddress()
                });

            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

    }
}
