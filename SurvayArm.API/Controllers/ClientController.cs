using AutoMapper;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SurvayArm.Application.Dto;
using SurvayArm.Application.IService;
using SurvayArm.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SurvayArm.API.Controllers
{
    [Authorize(Roles = "DataCollector")]
    [RoutePrefix("api/Client")]
    public class ClientController : ApiController
    {
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public ClientController(IClientService clientService, IMapper mapper)
        {
            _clientService = clientService;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(SurvayController));
        }

        [Route("GetAll")]
        public IHttpActionResult GetAllClient()
        {
            try
            {
                var clients = _clientService.GetAll();
                return Ok(clients);
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError(e);
            }
        }

        [Route("Find")]
        public IHttpActionResult FindClient([FromBody]JObject criteria) 
        {
            try
            {
                var search = criteria["criteria"];
                if (search !=null)
                {
                    var searchCriteria = JsonConvert.DeserializeObject<ClientDto>(search.ToString());
                    var results = _clientService.Find(searchCriteria);
                    return Ok(results);
                }
                _log.Info($"Info : client search criteia is not in valid format  : {criteria?.ToString()}");
                return BadRequest("Request body is not in correct format");
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError();
            }
        }

        [Route("Save")]
        public IHttpActionResult Save(ClientDto clientDto) 
        {
            try
            {
                if (clientDto == null)
                {
                    return BadRequest($"Request body is null or not in correct format");
                }

                clientDto.CreatedBy = User.Identity.GetUserFullName();
                clientDto.CreatedDate = DateTime.Now;
                
                var results = _clientService.Insert(clientDto);
                return Ok(results);
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError(e);
            }
        }
    }
}
