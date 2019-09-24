using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Models;
using App.Models.AppModel;
using AutoMapper;
using log4net;
using Newtonsoft.Json;
using SurvayArm.Application.Dto;
using SurvayArm.Application.Enum;
using SurvayArm.Application.IService;
using SurvayArm.Utility;
using Syncfusion.JavaScript;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml;
using System.Data;
using OfficeOpenXml.Table;
using System.Drawing;
using App.Filters;

namespace App.Controllers
{
    [Authorize(Roles = "Admin")]
    [SessionExpire]
    public class SurvayController : Controller
    {
        private readonly IFieldDependantService _fieldDependantService;
        private readonly ILog _log;
        private readonly ISurvayService _survayService;
        private readonly ISurvayTypeService _survayTypeService;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserSurvayService _userSurvayService;
        private readonly IAnswerSurvayService _survayAnswer;
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        public SurvayController(ISurvayService survayService, IMapper mapper, IFieldDependantService fieldService
                    , ISurvayTypeService survayType, IUserSurvayService userSurvayService, IAnswerSurvayService survayAnswer)
        {
            _mapper = mapper;
            _survayService = survayService;
            _fieldDependantService = fieldService;
            _survayTypeService = survayType;
            _userSurvayService = userSurvayService;
            _survayAnswer = survayAnswer;
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
            _log = LogManager.GetLogger(typeof(SurvayController));
        }

        // GET: Survay
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllSurvayForGrid(DataManager data)
        {
            try
            {
                var filterBy = string.Empty;
                var sortBy = string.Empty;

                if (data.Sorted != null)
                {
                    var sortByList = data.Sorted.Select(sort => $"{sort.Name}{" "}{sort.Direction}").ToList();

                    sortBy = string.Join(",", sortByList);
                }
                if (data.Where != null)
                {
                    var filterByList = new List<string>();

                    var firstOrDefault = data.Where.FirstOrDefault();

                    if (firstOrDefault?.predicates != null)
                    {
                        foreach (var prd in firstOrDefault.predicates)
                        {
                            filterByList.Add($"{prd.Field} , {prd.value}");
                        }
                    }
                    else
                    {
                        if (firstOrDefault != null)
                        {
                            filterByList.Add($"{firstOrDefault.Field} , {firstOrDefault.value}");
                        }
                    }

                    filterBy = string.Join("|", filterByList);
                }

                var allSurvays = _survayService.GetAllWithPagination(data.Skip, data.Take, sortBy, filterBy).ToList();
                var survays = _mapper.Map<List<SurvayDto>, List<SurvayViewModel>>(allSurvays);
                var count = _survayService.GetCount(sortBy, filterBy);

                var result = new DataResult
                {
                    count = count,
                    result = survays.ToList()
                };

                var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e.Message}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateNew(int language, int? survayId)
        {
            try
            {
                ViewBag.Language = language;
                ViewBag.Id = 0;
                if (survayId != null)
                {
                    var survay = _survayService.GetSurvayById(survayId.Value);
                    if (survay.SurvayTypes == null) return View();
                    var survayType = survay.SurvayTypes.First();
                    var survayTypeMapped = _mapper.Map<SurvayTypeDto, SurvayTypeViewModel>(survayType);
                    return View("WithNewLanguageView", survayTypeMapped);
                }
                return View();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return View();
            }
        }

        [HttpPost]
        public ActionResult SaveNewSurvay(string model, string name, string description, int languageType)
        {
            try
            {
                var survayType = JsonConvert.DeserializeObject<SurvayTypeViewModel>(model);
                if (survayType == null)
                    return Json(new { status = false, message = "Fields are not in proper formate" }, JsonRequestBehavior.AllowGet);
                survayType.IsActive = true;
                survayType.Description = description;
                survayType.LanguageType = languageType;
                survayType.CreatedBy = User.Identity.GetUserFullName();
                survayType.UpdatedBy = User.Identity.GetUserFullName();
                survayType.CreatedDate = DateTime.Now;
                survayType.UpdatedDate = DateTime.Now;

                if (survayType.fields != null && survayType.fields.Any())
                {
                    OrderFieldsByOrderNo(survayType.fields.ToList());
                }
                var survayModel = new SurvayViewModel();
                survayModel.SurvayTypes.Add(survayType);
                survayModel.IsActive = true;
                survayModel.Name = name;
                survayModel.NoOfQuestion = survayType.fields.Count;
                survayModel.CreatedBy = User.Identity.GetUserFullName();
                survayModel.CreatedDate = DateTime.Now;
                survayModel.UpdatedBy = User.Identity.GetUserFullName();
                survayModel.UpdatedDate = DateTime.Now;

                var dto = _mapper.Map<SurvayViewModel, SurvayDto>(survayModel);

                _survayService.InsertSurvay(dto);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult GetCreatedSurvaysView()
        {
            try
            {
                var surays = _survayService.GetActiveSurvay();

                var listItems = surays.Select(x => new SelectListItem
                {
                    Text = $"{x.Id} - {x.Name}",
                    Value = $"{x.Id}"
                });
                return PartialView("_CreatedSurvayLoadedPartialView", listItems);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCreatedSurvayTypesView(int survayId)
        {
            try
            {
                var suraysTypes = _survayTypeService.GetSurvayTypesBySurvayId(survayId);
                var survayTypesModel = _mapper.Map<List<SurvayTypeDto>, List<SurvayTypeViewModel>>(suraysTypes.ToList());

                return PartialView("_CreatedSurvayTypesPartialView", survayTypesModel);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetEditSurvayView(int id)
        {
            try
            {
                var suray = _survayTypeService.GetSurvayTypeById(id);

                var model = _mapper.Map<SurvayTypeDto, SurvayTypeViewModel>(suray);
                return PartialView("_EditorView", model);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "UnSuccess ,Please try again" },
                    JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult UpdateSurvay(string model, string name, string description, int survayId, int id, bool isActive)
        {
            try
            {
                var survayType = JsonConvert.DeserializeObject<SurvayTypeViewModel>(model);

                if (survayType == null)
                    return Json(new { status = false, message = "UnSuccess ,Please try again" },
                        JsonRequestBehavior.AllowGet);

                survayType.IsActive = isActive;
                survayType.Id = id;
                survayType.Description = description;
                survayType.UpdatedBy = User.Identity.GetUserFullName();
                survayType.UpdatedDate = DateTime.Now;

                if (survayType.fields != null && survayType.fields.Any())
                {
                    OrderFieldsByOrderNo(survayType.fields.ToList());
                }

                var updatedSurvayDto = new SurvayDto()
                {
                    Id = survayId,
                    NoOfQuestion = survayType.fields != null ? survayType.fields.Count : 0,
                    Name = name,
                    UpdatedBy = User.Identity.GetUserFullName(),
                    UpdatedDate = DateTime.Now,
                    IsActive = isActive

                };
                var dto = _mapper.Map<SurvayTypeViewModel, SurvayTypeDto>(survayType);
                updatedSurvayDto.SurvayTypes.Add(dto);
                _survayService.UpdateSurvay(updatedSurvayDto);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "UnSuccess ,Please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetLanguageBasedSurvayById(int survayId)
        {
            try
            {
                var survays = _survayService.GetSurvayById(survayId);
                var isSinhala = false;
                var isEnglish = false;
                var isTamil = false;
                if (survays == null) return Json(new { isSinhala, isEnglish, isTamil }, JsonRequestBehavior.AllowGet);
                foreach (var survay in survays.SurvayTypes)
                    switch (survay.LanguageType)
                    {
                        case (int)EnumSurvayLanguageType.Sinhala:
                            isSinhala = true;
                            break;
                        case (int)EnumSurvayLanguageType.English:
                            isEnglish = true;
                            break;
                        default:
                            isTamil = true;
                            break;
                    }

                return Json(new { isSinhala, isEnglish, isTamil }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveNewLanguageSurvay(string model, string description, int languageType, int code)
        {
            try
            {
                var dModel = JsonConvert.DeserializeObject<SurvayTypeViewModel>(model);

                if (dModel == null)
                {
                    return Json(new { status = false, message = "Questions are not properly structured. Please re try in a while" }, JsonRequestBehavior.AllowGet);
                }
                if (dModel.fields != null && dModel.fields.Any())
                {
                    OrderFieldsByOrderNo(dModel.fields.ToList());
                }

                dModel.IsActive = true;
                dModel.SurvayId = code;
                dModel.Description = description;
                dModel.LanguageType = languageType;
                dModel.CreatedBy = User.Identity.GetUserFullName();
                dModel.CreatedDate = DateTime.Now;
                dModel.UpdatedBy = User.Identity.GetUserFullName();
                dModel.UpdatedDate = DateTime.Now;

                var dto = _mapper.Map<SurvayTypeViewModel, SurvayTypeDto>(dModel);

                _survayTypeService.InsertSurvayType(dto);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                var model = new SurvayDto()
                {
                    Id = id,
                    IsActive = false,
                    UpdatedBy = User.Identity.GetUserFullName(),
                    UpdatedDate = DateTime.Now
                };

                _survayService.ManageActivation(model);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ReActivate(int id)
        {
            try
            {
                var model = new SurvayDto()
                {
                    Id = id,
                    IsActive = true,
                    UpdatedBy = User.Identity.GetUserFullName(),
                    UpdatedDate = DateTime.Now
                };
                _survayService.ManageActivation(model);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetCreatedSurvayTypesForDependantView(int survayId)
        {
            try
            {
                var suraysTypes = _survayTypeService.GetSurvayTypesBySurvayId(survayId);
                var survayTypesModel = _mapper.Map<List<SurvayTypeDto>, List<SurvayTypeViewModel>>(suraysTypes.ToList());

                return PartialView("_CreatedSurvayTypesForDependantPartialView", survayTypesModel);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetFieldDependantBySurvayCode(int survayId)
        {
            try
            {
                var survayType = _survayTypeService.GetSurvayTypeById(survayId);
                var fieldDependants = new List<FieldDependantViewModel>();
                if (survayType == null)
                    return Json(new { status = false, message = "UnSuccess ,No any survay type found" },
                        JsonRequestBehavior.AllowGet);
                foreach (var field in survayType.Fields)
                {
                    // logic belonged to that quetion
                    if (field.FieldDependants != null && field.FieldDependants.Any())
                    {
                        var dependants =
                            _mapper.Map<IList<FieldDependantDto>, IList<FieldDependantViewModel>>(field.FieldDependants);

                        foreach (var depe in dependants)
                        {
                            if (fieldDependants.Count == 0)
                            {
                                fieldDependants.Add(depe);
                            }
                            else if (fieldDependants.SingleOrDefault(i => i.Id == depe.Id) == null)
                            {
                                fieldDependants.Add(depe);
                            }
                        }
                    }
                    // logic belonged to other quetion
                    if (field.FieldDependants1 != null && field.FieldDependants1.Any())
                    {
                        var dependants =
                            _mapper.Map<IList<FieldDependantDto>, IList<FieldDependantViewModel>>(field.FieldDependants1);
                        foreach (var depe in dependants)
                        {
                            if (fieldDependants.Count == 0)
                            {
                                fieldDependants.Add(depe);
                            }
                            else if (fieldDependants.SingleOrDefault(i => i.Id == depe.Id) == null)
                            {
                                fieldDependants.Add(depe);
                            }
                        }

                    }
                }
                var fieldsModel = _mapper.Map<List<FieldDto>, List<FieldViewModel>>(survayType.Fields.ToList());
                ViewBag.Fields = fieldsModel.OrderBy(x => x.orderNo).ToList();
                return PartialView("_DependantInsertView", fieldDependants);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveDependantField(string model, int survayId)
        {
            try
            {
                var dependantList = JsonConvert.DeserializeObject<List<FieldDependantViewModel>>(model);

                if (dependantList != null)
                {
                    ICollection<FieldDependantDto> dependantModelList = new HashSet<FieldDependantDto>();

                    foreach (var dependant in dependantList)
                        if (dependant.Id > 0)
                        {
                            dependant.UpdatedBy = User.Identity.GetUserFullName();
                            dependant.UpdatedDate = DateTime.Now;
                            dependant.IsActive = true;
                            dependant.DependantFieldId = dependant.DependantFieldId == 0 ? null : dependant.DependantFieldId;

                            var dto = _mapper.Map<FieldDependantViewModel, FieldDependantDto>(dependant);
                            dependantModelList.Add(dto);
                        }
                        else
                        {
                            dependant.CreatedBy = User.Identity.GetUserFullName();
                            dependant.CreatedDate = DateTime.Now;
                            dependant.UpdatedBy = User.Identity.GetUserFullName();
                            dependant.UpdatedDate = DateTime.Now;
                            dependant.IsActive = true;
                            dependant.DependantFieldId = dependant.DependantFieldId == 0 ? null : dependant.DependantFieldId;
                            var dto = _mapper.Map<FieldDependantViewModel, FieldDependantDto>(dependant);
                            dependantModelList.Add(dto);
                        }
                    if (dependantModelList.Any())
                    {
                        _fieldDependantService.InsertFieldDependant(dependantModelList, survayId);
                    }
                    else
                    {
                        _fieldDependantService.DeleteFieldDependancyBySurvayId(survayId);
                    }


                    // dependantList empty and survay Id has value then check fileds dependancies for that survay and delete them
                    return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "UnSuccess ,Please try again" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "UnSuccess ,Please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUsersAssignedToSurvay(int survayId)
        {
            try
            {
                var dataCollectors = GetDataCollectorUser();
                var userSurvays = _userSurvayService.GetBySurvayId(survayId);

                //var unAssignedUser = dataCollectors.FindAll(x => !userSurvays.Select(s => s.UserId).Any(d => x.Id == d));
                ViewBag.Users = dataCollectors;
                return PartialView("_UserSurvayEditView", userSurvays);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "UnSuccess ,Please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveUsersAssignedToSurvay(string model)
        {
            try
            {
                var usersAssigned = JsonConvert.DeserializeObject<List<UserSurvayDto>>(model);

                if (usersAssigned != null && usersAssigned.Any())
                {
                    var users = usersAssigned.Select(x => new UserSurvayDto()
                    {
                        SurvayId = x.SurvayId,
                        UserId = x.UserId,
                        IsActive = true,
                        CreatedBy = User.Identity.GetUserFullName(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.GetUserFullName(),
                        UpdatedDate = DateTime.Now
                    }).ToList();

                    _userSurvayService.Insert(users, usersAssigned.First().SurvayId);
                    return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "UnSuccess ,Please try again" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "UnSuccess ,Please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult CheckAnswerAvailable(int survayId)
        {
            try
            {
                var isAvailable = _survayAnswer.GetSurvayCountHasDone(survayId) > 0;

                if (isAvailable)
                {
                    return Json(new { isAvailable = true, message = "Answers are available" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { isAvailable = false, message = "Answers are not available for this survay yet." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { isAvailable = false, message = "Error occured ,Please try again" },
                    JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ExportAnswerToCsv(int survayId)
        {
            try
            {
                var survayAnswersDataSet = _survayAnswer.ExportToCsv(survayId);

                foreach (DataTable dataTable in survayAnswersDataSet.Tables)
                {
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dataTable.TableName);
                        ws.Cells["A1"].LoadFromDataTable(dataTable, true, TableStyles.Medium22); //You can Use TableStyles property of your desire.    
                        ws.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                        ws.Cells.AutoFitColumns();                                                                           //Read the Excel file in a byte array    
                        Byte[] fileBytes = pck.GetAsByteArray();
                        HttpContext.Response.ClearContent();
                        HttpContext.Response.AddHeader("content-disposition", $"attachment; filename =SurvayCode_{survayId}_{DateTime.Now.ToShortDateString()}.xlsx");
                        HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        HttpContext.Response.BinaryWrite(fileBytes);
                        HttpContext.Response.End();

                    }
                }

                return RedirectToAction("Index", "Survay");
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return RedirectToAction("Index", "Survay");
            }

        }

        /// <summary>
        /// Return all Users  belong to Data collector role
        /// </summary>
        /// <param name="survayId"></param>
        /// <returns></returns>
        private List<ApplicationUser> GetDataCollectorUser()
        {
            var dataCollectorUsers = new List<ApplicationUser>();
            var assignedSupervisorUsersEntity = new List<ApplicationUser>();
            try
            {
                var datacollectors = _roleManager.FindByName("DataCollector").Users.ToList();
                var allUsers = UserManager.Users.Where(x => x.IsActive).ToList();

                dataCollectorUsers.AddRange(datacollectors.Select(user => allUsers.FirstOrDefault(x => x.Id == user.UserId)));

                return dataCollectorUsers;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return null;
            }
        }
        private void OrderFieldsByOrderNo(List<FieldViewModel> fields)
        {
            for (int i = 0; i < fields.ToArray().Length; i++)
            {
                fields[i].orderNo = i + 1;
            }
        }
    }
}