using AutoMapper;
using log4net;
using Newtonsoft.Json;
using SurvayArm.API.Helper;
using SurvayArm.API.Models.AppModel;
using SurvayArm.Application.Dto;
using SurvayArm.Application.IService;
using SurvayArm.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace SurvayArm.API.Controllers
{
    [Authorize(Roles = "DataCollector")]
    [RoutePrefix("api/Survay")]
    public class SurvayController : ApiController
    {
        private readonly ISurvayService _survayService;
        private readonly ISurvayTypeService _survayTypeService;
        private readonly IUserSurvayService _userSurvayService;
        private readonly IAnswerSurvayService _answerSurvayService;
        private readonly IClientService _clientService;
        private readonly IDeviceManagerService _deviceManager;
        private readonly IMapper _mapper;
        private readonly ILog _log;
        public SurvayController(ISurvayService survayService, IMapper mapper, ISurvayTypeService survayTypeService,
            IAnswerSurvayService answerSurvayService, IClientService clientService, IUserSurvayService userSurvay, IDeviceManagerService deviceManager)
        {
            _survayService = survayService;
            _mapper = mapper;
            _userSurvayService = userSurvay;
            _survayTypeService = survayTypeService;
            _answerSurvayService = answerSurvayService;
            _clientService = clientService;
            _deviceManager = deviceManager;
            _log = LogManager.GetLogger(typeof(SurvayController));

        }

        [Route("GetAllActive")]
        public IHttpActionResult GetAllActiveSurvay()
        {
            try
            {
                var UserId = User.Identity.GetUserId();
                var dtos = _userSurvayService.GetActiveSurvaysAssignedForLoggedUser(UserId).ToList();
                if (dtos != null)
                {
                    List<SurvayDto> results = new List<SurvayDto>();
                    var survays = dtos.Select(x => x.Survay).ToList();
                    foreach (var dto in survays)
                    {
                        var survayTypes = dto.SurvayTypes;
                        if (survayTypes != null && survayTypes.Any())
                        {
                            foreach (var type in survayTypes)
                            {
                                dto.Languages = new List<Language>();

                                if (type.LanguageType == (int)EnumLanguageType.Shinhala)
                                {
                                    dto.Languages.Add(new Language() { Code = 1, Name = "Sinhala" });
                                }
                                else if (type.LanguageType == (int)EnumLanguageType.English)
                                {
                                    dto.Languages.Add(new Language() { Code = 2, Name = "English" });
                                }
                                else
                                {
                                    dto.Languages.Add(new Language() { Code = 3, Name = "Tamil" });
                                }

                            }
                        }

                        dto.CountHasDone = _answerSurvayService.GetSurvayCountHasDone(dto.Id);
                        dto.Target = dto.SurvaySetting?.Target ?? 0;

                    }

                    return Ok(survays);
                }

                return BadRequest($"No any survays assigned to user yet");
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError();
            }
        }

        [Route("GetByCode")]
        [Authorize]
        public IHttpActionResult GetByCode(int code)
        {
            try
            {
                var dtos = _survayTypeService.GetSurvayTypesBySurvayId(code).ToList();
                // var models = _mapper.Map<List<SurvayDto>, List<SurvayModel>>(dtos);
                var countDone = _answerSurvayService.GetSurvayCountHasDone(code);
                int target = 0;
                if (dtos != null && dtos.Any())
                {
                    target = dtos.First().Survay.SurvaySetting?.Target ?? 0;
                }
                return Ok(new { survayTypes = dtos, target = target, countHasDone = countDone });

            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError(e);
            }
        }

        [Route("GetByCodeAndLanguage")]
        public IHttpActionResult GetByCodeAndLanguage(int code, int language)
        {

            try
            {
                var dto = _survayTypeService.GetSurvaysByCodeAndLanguage(code, language);
                if (dto == null) return NotFound();
                var dependatModel = new List<FieldDependantDto>();

                foreach (var field in dto.Fields)
                {
                    if (field != null && !field.FieldDependants.Any()) continue;
                    if (field != null) dependatModel.AddRange(field.FieldDependants);
                }

                return Ok(new { survayType = dto, dependancies = dependatModel });
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }
        }

        [Route("GetById")]
        [Authorize]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var dto = _survayService.GetSurvayById(id);
                //var model = _mapper.Map<SurvayDto,SurvayModel>(dto);
                return Ok(dto);

            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }
        }

        [Route("SaveAnswer")]
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> SaveAnswer()
        {
            try
            {
                // Check if the request contains multipart/form-data.  
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

                NameValueCollection formData = provider.FormData;
                //access files  
                IList<HttpContent> files = provider.Files;

                if (formData["result"] == null)
                {
                    return BadRequest($"Survay answer request body is null");
                }

                var answerJsonStrig = formData["result"];
                if (answerJsonStrig != null)
                {
                    var answer = JsonConvert.DeserializeObject<SurvayAnswerModel>(answerJsonStrig.ToString());
                    if (answer == null)
                    {
                        return BadRequest($"Survay answer model is null or not in correct format");
                    }

                    answer.Answer.CreatedBy = User.Identity.GetUserFullName();
                    var deviceUniqueId = answer.Answer.DeviceUniqueId;
                    if (string.IsNullOrEmpty(deviceUniqueId))
                    {
                        return BadRequest($"Device Id is not correct or registered");
                    }

                    var device = _deviceManager.GetByDeviceId(deviceUniqueId);
                    if (device == null)
                    {
                        return BadRequest($"Device Id is not correct or registered");
                    }
                    answer.Answer.DeviceId = device.Id;
                    int clientId = answer.Answer.ClientId;
                    if (clientId == 0)
                    {
                        var client = answer.Client;
                        if (client == null)
                        {
                            _log.Error($"Info : client details are not received");
                            return BadRequest($"Client inside the answer is null or not in correct format");
                        }

                        client.CreatedBy = User.Identity.GetUserFullName();
                        client.CreatedDate = DateTime.Now;
                        clientId = _clientService.Insert(client);
                    }
                    answer.Answer.ClientId = clientId;
                    _answerSurvayService.Insert(answer.Answer);

                    if (files.Any())
                    {
                        return await SaveFilesInDirectory(files, answer);
                    }

                    return Ok(new { status = true, message = "Successfully inserted" });
                }

                _log.Info($"Info : survay answer saveing  object is not in valid in MultipartContent ");

                return BadRequest($"Survay answer request is in valid");
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError(e);
            }
        }

        private async Task<IHttpActionResult> SaveFilesInDirectory(IList<HttpContent> files, SurvayAnswerModel answer)
        {
            try
            {
                if (answer != null)
                {
                    foreach (var file in files)
                    {
                        var thisFileName = file.Headers.ContentDisposition.FileName.Trim('\"');

                        Stream input = await file.ReadAsStreamAsync();
                        var path = WebConfigurationManager.AppSettings["AnswerDocumentDirectory"];

                        if (string.IsNullOrEmpty(path))
                        {
                            path = HttpRuntime.AppDomainAppPath;
                        }

                        string directoryName = Path.Combine(path, "FilesOfAnswer");

                        // check if Directory exist
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        string survayFolderName = Path.Combine(directoryName, answer?.Answer?.SurvayId.ToString());

                        // check if survay directory exist
                        if (!Directory.Exists(survayFolderName))
                        {
                            Directory.CreateDirectory(survayFolderName);
                        }

                        string survayTypeFolderName = Path.Combine(survayFolderName, answer?.Answer?.SurvayTypeID.ToString());

                        // check if survayType folder exist
                        if (!Directory.Exists(survayTypeFolderName))
                        {
                            Directory.CreateDirectory(survayTypeFolderName);
                        }
                        string clientFolderName = Path.Combine(survayTypeFolderName, $"{answer?.Answer?.ClientId.ToString()}");

                        // check if client directory in survayType folder exist
                        if (!Directory.Exists(clientFolderName))
                        {
                            Directory.CreateDirectory(clientFolderName);
                        }

                        string filename = Path.Combine(clientFolderName, thisFileName);

                        //Deletion exists file  
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }

                        using (Stream doc = File.OpenWrite(filename))
                        {
                            input.CopyTo(doc);
                            //close file  
                            doc.Close();
                        }
                    }
                    return Ok(new { status = true, message = "Successfully inserted with file uploading" });
                }
                return BadRequest($"Infomation related to save documents is not correct or in valid format");
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError(e);
            }
        }
    }

}

