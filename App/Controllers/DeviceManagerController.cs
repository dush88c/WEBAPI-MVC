using App.Models;
using App.Models.AppModel;
using AutoMapper;
using log4net;
using Newtonsoft.Json;
using SurvayArm.Application.Dto;
using SurvayArm.Application.IService;
using SurvayArm.Utility;
using Syncfusion.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Filters;

namespace App.Controllers
{
    [Authorize(Roles ="Admin")]
    [SessionExpire]
    public class DeviceManagerController : Controller
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;

        private readonly IDeviceManagerService _deviceManagerService;

        public DeviceManagerController(IDeviceManagerService deviceMangerService , IMapper mapper)
        {
            _deviceManagerService = deviceMangerService;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(DeviceManagerController));
        }

        // GET: DeviceManager
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllDevicesForGrid(DataManager data)
        {
            try
            {
                var filterBy = string.Empty;
                var sortBy = string.Empty;

                if (data.Sorted != null)
                {
                    var sortByList = data.Sorted.Select(sort => string.Equals(sort.Name , "DeviceBrandName" ,StringComparison.OrdinalIgnoreCase) ? $"DeviceBrandId{" "}{sort.Direction}" : $"{ sort.Name}{" "}{sort.Direction}").ToList();

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
                            if (string.Equals(prd.Field ,"DeviceBrandName" ,StringComparison.OrdinalIgnoreCase))
                            {
                                filterByList.Add($"DeviceBrandId , {prd.value}");
                                continue;
                            }
                            filterByList.Add($"{prd.Field} , {prd.value}");
                        }
                    }
                    else
                    {
                        if (firstOrDefault != null)
                        {
                            if (string.Equals(firstOrDefault.Field, "DeviceBrandName", StringComparison.OrdinalIgnoreCase))
                            {
                                filterByList.Add($"DeviceBrandId , {firstOrDefault.value}");

                            }
                            else
                            {
                                filterByList.Add($"{firstOrDefault.Field} , {firstOrDefault.value}");
                            }
                            
                        }
                    }

                    filterBy = string.Join("|", filterByList);
                }

                var allSurvays = _deviceManagerService.GetAllWithPagination(data.Skip, data.Take, sortBy, filterBy).ToList();
                var survays = _mapper.Map<List<DeviceManagerDto>, List<DeviceManagerViewModel>>(allSurvays);
                var count = _deviceManagerService.GetCount(sortBy, filterBy);

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

        public ActionResult GetNewDeviceManagerView()
        {
            try
            {
                return PartialView("_NewDeviceView");
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Insert (string model)
        {
            try
            {
                var deviceInfo  = JsonConvert.DeserializeObject<DeviceManagerViewModel>(model);

                if (deviceInfo == null)
                {
                    return Json(new { status = false, message = "Invalid model, Please try agian" },
                    JsonRequestBehavior.AllowGet);
                }
                var existDevice = _deviceManagerService.GetByDeviceId(deviceInfo.DeviceId);

                if (existDevice != null)
                {
                    return Json(new { status = false, message = "Device id is already registered, Plese enter a new device Id" },
                   JsonRequestBehavior.AllowGet);
                }

                deviceInfo.CreatedBy = User.Identity.GetUserFullName();
                deviceInfo.CreatedDate = DateTime.Now;
                deviceInfo.UpdatedBy = User.Identity.GetUserFullName();
                deviceInfo.UpdatedDate = DateTime.Now;
                deviceInfo.IsActive = true;

                var dto = _mapper.Map<DeviceManagerViewModel, DeviceManagerDto>(deviceInfo);               

                _deviceManagerService.Insert(dto);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "Error in inserting, please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDeviceEditorView(int id)
        {
            try
            {
                var updateDevice = _deviceManagerService.GetById(id);
                var deviceInfo =  _mapper.Map<DeviceManagerDto, DeviceManagerViewModel>(updateDevice);
                return PartialView("_EditDeviceView" , deviceInfo);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut]
        public ActionResult Update(string model)
        {
            try
            {
                var deviceInfo = JsonConvert.DeserializeObject<DeviceManagerViewModel>(model);

                if (deviceInfo == null)
                {
                    return Json(new { status = false, message = "Invalid model, Please try agian" },
                    JsonRequestBehavior.AllowGet);
                }

                var updateDevice = _deviceManagerService.GetById(deviceInfo.Id);
                var existDevice = _deviceManagerService.GetByDeviceId(deviceInfo.DeviceId);
                if (updateDevice == null)
                {
                    _log.Info($"Info :  DeviceManager information is not found for Id : {deviceInfo.Id}");
                    return Json(new { status = false, message = "Device info not found, Plese try agin" },
                    JsonRequestBehavior.AllowGet);
                }

                if (existDevice != null && deviceInfo.Id != existDevice.Id)
                {
                    return Json(new { status = false, message = "Device id is already registered, Plese enter a new device Id" },
                  JsonRequestBehavior.AllowGet);
                }              

                updateDevice.DeviceModelName = deviceInfo.DeviceModelName;
                updateDevice.DeviceBrandId = deviceInfo.DeviceBrandId;
                updateDevice.DeviceId = deviceInfo.DeviceId;
                updateDevice.UpdatedBy = User.Identity.GetUserFullName();
                updateDevice.UpdatedDate = DateTime.Now;
                updateDevice.IsActive = true;

                _deviceManagerService.Update(updateDevice);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "Error in inserting, please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Activation(int id, bool isActive) 
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { status = false, message = "Invalid Device management id, Please try agian" },
                    JsonRequestBehavior.AllowGet);

                }

                var updateDevice = _deviceManagerService.GetById(id);

                if (updateDevice == null)
                {
                    _log.Info($"Info :  DeviceManager information is not found  when deleting , for Id : {id}");

                    return Json(new { status = false, message = "Device info not found, Plese try agin" },
                    JsonRequestBehavior.AllowGet);
                }

                updateDevice.IsActive = isActive;
                updateDevice.UpdatedBy = User.Identity.GetUserFullName();
                updateDevice.UpdatedDate = DateTime.Now;

                _deviceManagerService.Update(updateDevice);
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}