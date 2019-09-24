using App.Filters;
using App.Models;
using App.Models.AppModel;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Syncfusion.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    [SessionExpire]
    [Authorize]
    public class UserRoleController : Controller
    {
        private readonly ILog _log;

        private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public List<IdentityRole> AllRoles
        {
            get
            {
                return (List<IdentityRole>)System.Web.HttpContext.Current.Session["Roles"];
            }
            set
            {
                System.Web.HttpContext.Current.Session["Roles"] = value;
            }
        }
        public List<UserViewModel> AllUsers
        {
            get
            {
                return (List<UserViewModel>)System.Web.HttpContext.Current.Session["Users"];
            }
            set
            {
                System.Web.HttpContext.Current.Session["Users"] = value;
            }
        }

        public UserRoleController()
        {
            _log = LogManager.GetLogger(typeof(UserRoleController));
            _roleManager =  new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());            
            
        }       

        // GET: UserRole
        public ActionResult Index()
        {
            SetAllRole();
            SetAllUsers();
            ViewBag.Roles = AllRoles;
            
            return View();
        }       

        [HttpPost]
        public ActionResult GetAllUserRoleForGrid(DataManager data ,string roleName)
        {
            try
            {               
                var uRoles = _roleManager.FindByName(roleName).Users.ToList();

                var result = new DataResult()
                {
                    count = uRoles.Count,
                    result = uRoles.Skip(data.Skip).Take(data.Take)
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllRole()
        {
            try
            {
                if (_roleManager.Roles != null)
                {
                    var roles = _roleManager.Roles.ToList();                   

                    return Json(roles, JsonRequestBehavior.AllowGet);

                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetLoaddedAddView()
        {
            try
            {
                ViewBag.Roles = AllRoles;
                ViewBag.Users = AllUsers;
                return  PartialView("_AddView");
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AssignUserRole(string model)
        {
            try
            {
                var userRole = JsonConvert.DeserializeObject<IdentityUserRole>(model);

                UserManager?.AddToRole(userRole.UserId, userRole.RoleId);

                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "Error in saved , please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult DeleteUserRole(string userId , string role)
        {
            try
            {
                //var userRole = JsonConvert.DeserializeObject<IdentityUserRole>(model);

                UserManager?.RemoveFromRole(userId, role);

                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "Error in  remove user from roles , please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        private void SetAllRole()
        {
            try
            {
                if (AllRoles == null)
                {
                    if (_roleManager.Roles != null)
                    {
                        var roles = _roleManager.Roles.ToList();
                        AllRoles = roles;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");

            }
        }

        private void SetAllUsers()
        {
            try
            {
                if (UserManager?.Users != null)
                {
                    var users = UserManager.Users.ToList();
                    var userModel = users.Select(x => new UserViewModel
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Gender = x.Gender,
                        Email = x.Email,
                        AddressLine01 = x.AddressLine01,
                        AddressLine02 = x.AddressLine02,
                        City = x.City,
                        BirthOfDateDisplayName = x.BirthOfDate?.ToShortDateString() ?? "N/A",
                        IsActive = x.IsActive,
                        HomePhoneNumber = x.HomePhoneNumber,
                        MobileNumber = x.PhoneNumber,
                        Password = x.PasswordHash
                    });

                    AllUsers = userModel.ToList(); ;
                }

            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");

            }
        }

    }
}