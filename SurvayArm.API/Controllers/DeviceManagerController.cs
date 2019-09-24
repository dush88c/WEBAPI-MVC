using AutoMapper;
using log4net;
using SurvayArm.Application.IService;
using System;
using System.Web.Http;

namespace SurvayArm.API.Controllers
{
    [Authorize(Roles = "DataCollector")]
    [RoutePrefix("api/DeviceManager")]
    public class DeviceManagerController : ApiController
    {
        private readonly IDeviceManagerService _deviceManagerService ;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public DeviceManagerController(IDeviceManagerService service , IMapper mapper)
        {
            _deviceManagerService = service;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(DeviceManagerController));

        }

        [Route("IsRegistered")]
        [AllowAnonymous]
        public IHttpActionResult GetByDeviceId(string deviceId) 
        {
            try
            {
                var deviceInfo   = _deviceManagerService.GetByDeviceId(deviceId);
                if (deviceInfo != null)
                {
                    return Ok(new { isRegistered = true, registeredId = deviceInfo.Id }); 
                }
                
                return Ok(new { isRegistered = false , registeredId = 0 });  

            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError();
            }
        }
    }
}
