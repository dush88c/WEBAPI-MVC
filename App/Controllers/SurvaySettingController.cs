using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Models;
using App.Models.AppModel;
using AutoMapper;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SurvayArm.Application.Dto;
using SurvayArm.Application.IService;
using SurvayArm.Utility;

namespace App.Controllers
{
    public class SurvaySettingController : Controller
    {
        private readonly ILog _log;
        private readonly ISurvaySettingService _survaySettingService;
        private readonly ISurvayService _survayService;
        private readonly IMapper _mapper;
        private readonly IDistricService _districService;
        private readonly IProvinceService _provinceService;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISurvaySupervisorService _survaySupervisorService;

        public ApplicationUserManager UserManager =>  HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        public SurvaySettingController(ISurvaySettingService survaySettingService, IMapper mapper 
            ,IDistricService districService , IProvinceService provinceService 
            , ISurvayService survayService, ISurvaySupervisorService survaySupervisorService)
        {
            _mapper = mapper;
            _provinceService = provinceService;
            _districService = districService;
            _survayService = survayService;
            _survaySupervisorService = survaySupervisorService;
            _survaySettingService = survaySettingService;
            _log = LogManager.GetLogger(typeof(SurvaySettingController));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
        }

        [HttpGet]
        public ActionResult GetActiveSurvaySettings(int survayId)
        {
            try
            {
                var survaySetting = _survaySettingService.GetSurvaySettingById(survayId);
                var districs = _districService.GetActiveAll();
                var provinces = _provinceService.GetActiveAll();
                var survayDto = _survayService.GetSurvayById(survayId);
                var survay = _mapper.Map<SurvayDto, SurvayViewModel>(survayDto);
                var supervisors = GetAssignedSuperviosrUsers(survayId);

                ViewBag.AllSupervisorUsers = supervisors.Item1 ?? new List<ApplicationUser>();
                ViewBag.AssignedSupervisorUsers = supervisors.Item2 ?? new List<ApplicationUser>();
                ViewBag.Survay = survay;
                ViewBag.Districs = districs;
                ViewBag.Provinces = provinces;
                ViewBag.AllTargets = survaySetting?.Target ?? 0; 

                var setting = new List<SurvayTargetViewModel>();
                if (survaySetting == null) return PartialView("_SurvaySettingView", setting);
                ViewBag.AllTargets = survaySetting.Target;
                setting = _mapper.Map<List<SurvayTargetDto>, List<SurvayTargetViewModel>>(survaySetting.SurvayTargets);

                return PartialView("_SurvaySettingView", setting);

            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveSurvaySettings(string settings, string superviosrs)
        {
            try
            {
                var survaySetting = JsonConvert.DeserializeObject<SurvaySettingViewModel>(settings);

                if (survaySetting == null) return Json(new {status = false}, JsonRequestBehavior.AllowGet);

                survaySetting.CreatedBy = User.Identity.GetUserFullName();
                survaySetting.CreatedDate = DateTime.Now;
                survaySetting.UpdatedBy = User.Identity.GetUserFullName();
                survaySetting.UpdatedDate = DateTime.Now;
                survaySetting.IsActive = true;

                foreach (var target in survaySetting.SurvayTargets)
                {
                    if (target.Id <= 0)
                    {
                        target.CreatedBy = User.Identity.GetUserFullName();
                        target.CreatedDate = DateTime.Now;
                    }
                    target.UpdatedBy = User.Identity.GetUserFullName();
                    target.UpdatedDate = DateTime.Now;
                }

                var supervisors = JsonConvert.DeserializeObject<List<SurvaySupervisorDto>>(superviosrs);

                foreach (var sup in supervisors)
                {
                    sup.IsActive = true;
                    sup.CreatedBy = User.Identity.GetUserFullName();
                    sup.CreatedDate = DateTime.Now;
                    sup.UpdatedBy = User.Identity.GetUserFullName();
                    sup.UpdatedDate = DateTime.Now;
                }

                var dto = _mapper.Map<SurvaySettingViewModel, SurvaySettingDto>(survaySetting);
                _survaySettingService.UpdateSurvaySetting(dto);
                _survaySupervisorService.UpdateSurvaySupervisors(supervisors, survaySetting.SurvayId);
                return Json(new {status = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetDistricsByProvinceId(int provinceId)
        {
            try
            {
                var districs = _districService.GetProvinceId(provinceId);
                return Json(districs , JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Return all user list in first parm and assigned users in second param
        /// </summary>
        /// <param name="survayId"></param>
        /// <returns></returns>
        private Tuple<List<ApplicationUser> ,List<ApplicationUser>> GetAssignedSuperviosrUsers( int survayId)
        {
            var supervisorUsersEntity = new List<ApplicationUser>();
            var assignedSupervisorUsersEntity = new List<ApplicationUser>(); 
            try
            {
                var supervisorUsers = _roleManager.FindByName("Supervisor").Users.ToList();
                var allUsers = UserManager.Users.Where(x=>x.IsActive);

                supervisorUsersEntity.AddRange(supervisorUsers.Select(user => allUsers.SingleOrDefault(x => x.Id == user.UserId)));
                
                var assignedUsers = _survaySupervisorService.GetSupervisorsBySurvayId(survayId);
                if (assignedUsers != null && !assignedUsers.Any())
                {
                    var orderedUsers = supervisorUsersEntity.OrderBy(o => o.FirstName).ToList();
                    return Tuple.Create<List<ApplicationUser> , List<ApplicationUser>>(orderedUsers, null);
                }

                if (assignedUsers != null)
                    assignedSupervisorUsersEntity.AddRange(
                        assignedUsers.Select(d => allUsers.SingleOrDefault(x => x.Id == d.SupervisorId)));

                return Tuple.Create<List<ApplicationUser>, List<ApplicationUser>>(supervisorUsersEntity, assignedSupervisorUsersEntity);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return null;
            }
        }
    }
}