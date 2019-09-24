using App.Filters;
using App.Models;
using App.Models.AppModel;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SurvayArm.Utility;
using Syncfusion.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    [SessionExpire]
    [Authorize]
    public class UserManagementController : Controller 
    {
            
        private readonly ILog _log;

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

        public UserManagementController()
        {
            _log = LogManager.GetLogger(typeof(UserManagementController));
            
        }
        // GET: UserManagment
        public ActionResult Index()
        {
            return View();
        }              

        public JsonResult GetAllUserForGrid(DataManager data)
        {
            try
            {
                if (UserManager.Users != null)
                {
                    var users = UserManager.Users
                                    .OrderByDescending(x => x.UpdatedDate)
                                    .ThenByDescending(x => x.IsActive).ToList();
                    
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

                    var result = new DataResult();
                    var userViewModels = userModel as IList<UserViewModel> ?? userModel.ToList();
                    result.count = userViewModels.Count;
                    result.result = userViewModels.Skip(data.Skip).Take(data.Take);
                     
                    return Json(result, JsonRequestBehavior.AllowGet);

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
                return PartialView("_UserAddView");
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Create(string model)
        {
            try
            {
                var value = JsonConvert.DeserializeObject<UserViewModel>(model);
                var crypto = new Encryption();
                var user = new ApplicationUser
                {
                    FirstName = value.FirstName,
                    LastName = value.LastName,
                    Gender = '0',
                    Email = value.Email,
                    AddressLine01 = value.AddressLine01,
                    AddressLine02 = value.AddressLine02,
                    City = value.City,
                    BirthOfDate = value.BirthOfDate,
                    IsActive = true,
                    HomePhoneNumber = value.HomePhoneNumber,
                    PhoneNumber = value.MobileNumber,
                    PasswordHash = value.Password,
                    Password = crypto.DoEncrypt(value.Password),
                    UserName = value.Email,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.GetUserFullName(), 
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = User.Identity.GetUserFullName()
                };
                var exist = UserManager.Users.SingleOrDefault(x => x.UserName == user.UserName);

                if (exist == null)
                {
                    var result = UserManager.Create(user, value.Password);
                    if (result.Succeeded)
                    {
                        return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
                    }

                    _log.Error($"Error :  {string.Join("and" , result.Errors)}");
                }
                else
                {
                    return Json(new { status = false, message = "Email is already used, please type new " },
                        JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = false, message = "Error in inserting, please try again" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "Error in inserting, please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetLoaddedEditorView(string id)
        {
            try
            {
                var dbUser = UserManager.Users.FirstOrDefault(x=> x.Id.ToLower().Trim() == id.ToLower().Trim());

                if (dbUser != null)
                {
                    return PartialView("_UserEditView", dbUser);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Update(string model)
        {
            try
            {
                var value = JsonConvert.DeserializeObject<UserViewModel>(model);

                var dbUser = UserManager.Users.SingleOrDefault(x=>x.Id== value.Id);
                if (dbUser == null)
                {
                    return Json(new { status = false, message = "User not found" }, JsonRequestBehavior.AllowGet);
                }

                dbUser.FirstName = value.FirstName;
                dbUser.LastName = value.LastName;
                dbUser.Email = value.Email;
                dbUser.AddressLine01 = value.AddressLine01;
                dbUser.AddressLine02 = value.AddressLine02;
                dbUser.City = value.City;
                dbUser.BirthOfDate = value.BirthOfDate;
                dbUser.HomePhoneNumber = value.HomePhoneNumber;
                dbUser.PhoneNumber = value.MobileNumber;
                dbUser.UpdatedDate = DateTime.Now;
                dbUser.UserName = value.Email;
                dbUser.IsActive = value.IsActive;
                dbUser.UpdatedBy = User.Identity.GetUserFullName();

                var exist = UserManager.Users.SingleOrDefault(x => x.UserName == dbUser.UserName && x.Id != dbUser.Id);

                if (exist == null)
                {
                    UserManager.Update(dbUser);
                    
                    return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "Email is already used, please type new " },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "Error in updating , please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new { status = false, message = "Bad request, please try again in few minutes" },
                        JsonRequestBehavior.AllowGet);
                }
                var user = UserManager.FindById(id);
                if (user == null)
                {
                    return Json(new { status = false, message = "User not found" }, JsonRequestBehavior.AllowGet);
                }

                user.IsActive = false;
                user.UpdatedDate = DateTime.Now;
                user.UpdatedBy = User.Identity.GetUserFullName();

                UserManager.Update(user);
               
                return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { status = false, message = "Error , please try again in few minutes" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetLoaddedPwdEditorView(string id)
        {
            try
            {
                var dbUser = await UserManager.FindByIdAsync(id);
                var token = await UserManager.GeneratePasswordResetTokenAsync(id);

                if (dbUser != null)
                {
                    var pwdResetModel = new UserViewModel()
                    {
                        Email = dbUser.Email,
                        FirstName = dbUser.FirstName,
                        LastName = dbUser.LastName,
                        Password = dbUser.Password,
                        Token = token,
                        Id = dbUser.Id
                    };
                    return PartialView("_UserPwdEditView", pwdResetModel);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> ResetPassword(string model)
        {

            try
            {
                var value = JsonConvert.DeserializeObject<UserViewModel>(model);

                var user = await UserManager.FindByNameAsync(value.Email);
                if (user == null)
                {
                    throw new Exception($"{value.Email}  is not exist");
                }
                var result = await UserManager.ResetPasswordAsync(user.Id, value.Token, value.ConfirmPassword);
                if (result.Succeeded)
                {
                    return Json(new { status = true, message = "Success" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = false, message = "Password change is not completed, please re-try in few minutes.!" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(new { status = false, message = "Error in Restting password , please try again" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllUser()
        {
            try
            {
                if (UserManager.Users != null)
                {
                    var users = UserManager.Users.ToList()
                                    .OrderByDescending(x => x.UpdatedDate)
                                    .ThenByDescending(x => x.IsActive);                    

                     return Json(users, JsonRequestBehavior.AllowGet);

                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}