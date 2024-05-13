using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using AirPortERP.Extenstion;
using System.Data.Entity;

namespace AirPortERP.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        CommonUsed CommonUsed = new CommonUsed();
        RoleController RoleManager = new RoleController();
        Entities db = new Entities();

        // GET: Account

    private readonly UserManager<ApplicationUser> userManager;
        
    //  private readonly TraineeService TraineeService;


    //public ActionResult gett()
    //{
    //    var data = from m in db.ExecuteCourses
    //               select new CourseViewModel
    //               {
    //                   Amount = Convert.ToDecimal(m.Amount),
    //                   Name = Convert.ToDateTime(m.Course.Time.Year).Year.ToString()
    //               };

    //    return Json(data, JsonRequestBehavior.AllowGet);
    //}

    public AccountController()
        {
            var db = new ApplicationDbContext();

            var userStore = new UserStore<ApplicationUser>(db);
            userManager = new UserManager<ApplicationUser>(userStore);
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };




        }
        //  GET: Account
      


        public ActionResult Edit(string id)
        {

            AspNetUser AspNetUser = db.AspNetUsers.Single(x=>x.Id==id);


             UsersViewModel UsersViewModel = new UsersViewModel();


            UsersViewModel.Id = AspNetUser.Id;
            UsersViewModel.UserName = AspNetUser.UserName;
            UsersViewModel.Email = AspNetUser.Email;
            UsersViewModel.RoleCurrent = RoleManager.GetRole(AspNetUser.Id);
            string Role = UsersViewModel.RoleCurrent.ToString();
            UsersViewModel.RoleId = db.AspNetRoles.Single(x => x.Name == Role).Id;

            ViewBag.RoleNew = new SelectList(db.AspNetRoles.ToList(), "Name", "Name", UsersViewModel.RoleCurrent);

            return PartialView(UsersViewModel);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsersViewModel UsersViewModel)
        {
            if (ModelState.IsValid)
            {
                AspNetUser AspNetUser = db.AspNetUsers.Single(x => x.Id == UsersViewModel.Id);
               // AspNetUser.Email = UsersViewModel.Email;
                // asp AspNetRole = db.AspNetRoles.Single(x => x.Id == UsersViewModel.RoleId);
                // AspNetRole.
                //userManager.ro(UserId, UsersViewModel.UserType);

                if (UsersViewModel.Id!="" && UsersViewModel.RoleCurrent!="" && UsersViewModel.RoleNew != "")
                {
                    userManager.RemoveFromRole(UsersViewModel.Id, UsersViewModel.RoleCurrent);
                    userManager.AddToRole(UsersViewModel.Id, UsersViewModel.RoleNew);

                }
                db.Entry(AspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserManager");
            }
            return View();
        }


        public ActionResult ResetPassword(string id)
        {
            AspNetUser AspNetUser = db.AspNetUsers.Single(x => x.Id == id);


            ResetPasswordViewModel ResetPasswordViewModel = new ResetPasswordViewModel();


            ResetPasswordViewModel.Id = AspNetUser.Id;
            ResetPasswordViewModel.UserName = AspNetUser.UserName;
            ResetPasswordViewModel.Email = AspNetUser.Email;
            
            return View(ResetPasswordViewModel);
        }

        public ActionResult DeleteAsync(string userId)
        {
            if (userId != null)
            {
                ApplicationUser user = userManager.FindByIdAsync(userId).Result;
                var RoleCurrent = RoleManager.GetRole(user.Id);

              var DeleteRoleResult =  userManager.RemoveFromRole(user.Id, RoleCurrent);
            
               if(DeleteRoleResult.Succeeded)
                {
                    var DeleteUserResult =  userManager.Delete(user);

                    if (DeleteUserResult.Succeeded) {
                       return RedirectToAction("Usermanager", "Account");
                    }

                  
                }


                //  IdentityResult deletionResult = await userManager.RemoveFromRoleAsync(userId, roleName);

            }
            return RedirectToAction("Usermanager", "Account");
        }





        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            ApplicationUser user = userManager.FindById(model.Id);


            user.PasswordHash = userManager.PasswordHasher.HashPassword(model.Password);

            //var user = await userManager.FindByIdAsync(model.Id);
            //if (user == null)
            //{

            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}

            var result =  userManager.Update(user);

            if (result.Succeeded)
            {
             
                return RedirectToAction("UserManager", "Account");
            }

            return RedirectToAction("UserManager");
        }







        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl = "")
        {

            return View(new LoginViewModel
            {
                ReturnUrl = ReturnUrl
            });
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel LoginViewModel)
        {
            if (ModelState.IsValid)
            {
                var existuser = await userManager.FindAsync(LoginViewModel.UserName, LoginViewModel.Password);

                if (existuser != null)
                {
                    await SignIn(existuser);
                    if (!string.IsNullOrEmpty(LoginViewModel.ReturnUrl))
                    {
                        return Redirect(LoginViewModel.ReturnUrl);
                    }


                    var userRoles = userManager.GetRoles(existuser.Id);
                    var role = userRoles.FirstOrDefault();
                    if (role == "Admin")
                    {
                        return RedirectToAction("Index", "Home"/*, new { area = "Admin" }*/);

                    }

                    return RedirectToAction("Index", "Home");
                }

                LoginViewModel.Message = "Email Or Password Incorrect";

            }
            return View(LoginViewModel);
        }





        private async Task SignIn(ApplicationUser ApplicationUser)
        {
            var Identity = await userManager.CreateIdentityAsync(ApplicationUser, DefaultAuthenticationTypes.ApplicationCookie);
            var OwinContext = Request.GetOwinContext();
            var AuthManager = OwinContext.Authentication;
            AuthManager.SignIn(Identity);
        }














        [AllowAnonymous]
        public ActionResult Register()
        {
           ViewBag.UserType = new SelectList(db.AspNetRoles.Where(a => !a.Name.Contains("Admin")).ToList(), "Name", "Name");

            ViewBag.CompanyInfo = new SelectList(db.CompanyInfoes.ToList(), "Id", "NameEn");

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel RegisterViewModel)
        {
            ViewBag.UserType = new SelectList(db.AspNetRoles.Where(a => !a.Name.Contains("Admin")).ToList(), "Name", "Name");

            ViewBag.CompanyInfo = new SelectList(db.CompanyInfoes.ToList(), "Id", "NameEn");
            if (ModelState.IsValid)
            {            
                //string name = db.AspNetUsers.Single(x => x.UserName == RegisterViewModel.UserName).UserName;
                if (!db.AspNetUsers.Any(x => x.UserName == RegisterViewModel.UserName))
                {
                    ViewBag.CompanyInfo = new SelectList(db.CompanyInfoes.ToList(), "Id", "NameEn");


                    var identityUser = new ApplicationUser
                    {

                        Email = RegisterViewModel.Email,
                        UserName = RegisterViewModel.UserName,
                        CompanyInfoId = RegisterViewModel.CompanyInfo

                    };




                    var creationReasult = await userManager.CreateAsync(identityUser, RegisterViewModel.Password);


                    // User Created
                    if (creationReasult.Succeeded)
                    {
                        var UserId = identityUser.Id;
                        creationReasult = userManager.AddToRole(UserId, RegisterViewModel.UserType);

                        // Role Assigned
                        if (creationReasult.Succeeded)
                        {
                            //   await userManager.AddToRoleAsync(UserId, RegisterViewModel.UserType);


                            return RedirectToAction("UserManager", "Account");

                        }

                    }

                    var message = creationReasult.Errors.FirstOrDefault();
                    RegisterViewModel.Message = message;

                    return View(RegisterViewModel);

                }
                RegisterViewModel.Message = "User Name Already Found";

            }
           // RegisterViewModel.Message = "User Name Already Found";
            return View(RegisterViewModel);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            var OwinContext = Request.GetOwinContext();
            var AuthManager = OwinContext.Authentication;
            AuthManager.SignOut("ApplicationCookie");
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public async Task<ActionResult> ChangeProfile()
        {
            var id = User.Identity.GetUserId();
            ApplicationUser user = await userManager.FindByIdAsync(id);


            RegisterViewModel RegisterViewModel = new RegisterViewModel();
            RegisterViewModel.Id = user.Id;
            RegisterViewModel.Email = user.Email;
            RegisterViewModel.UserName = user.UserName;
            RegisterViewModel.OldEmail = user.Email;
            RegisterViewModel.OldUserName = user.UserName;
            return View(RegisterViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> changeProfile(RegisterViewModel RegisterViewModel)
        {

            ApplicationUser user = await userManager.FindByIdAsync(RegisterViewModel.Id);
            if (user == null)
            {
                return HttpNotFound();
            }
           
            var existuser = await userManager.FindAsync(RegisterViewModel.OldUserName, RegisterViewModel.OldPassword);

            if (existuser != null)
            {
                if (RegisterViewModel.Password == RegisterViewModel.ConfirmPassword)
                {
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(RegisterViewModel.Password);
                    user.Email = RegisterViewModel.Email;
                    user.UserName = RegisterViewModel.UserName;



                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        Logout();
                        return RedirectToAction("Login", "Account");
                    }
                }
                else { ViewBag.Message = " Password Not Match"; }
            }
            else { ViewBag.Message = "old Password Wrong"; }
            return View(RegisterViewModel);
        }



        /// <summary>
        /// //////////////////////////////////UserManager///////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>

        public ActionResult UserManager()
        {

            IQueryable<AspNetUser> AspNetUser = db.AspNetUsers;



            var UsersViewModel = new List<UsersViewModel>();
            foreach (var item in AspNetUser)
            {
                UsersViewModel.Add(new UsersViewModel
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    Email = item?.Email,
                    RoleCurrent = RoleManager.GetRole(item.Id),
                    // IsActive = GetIsActiveName(Convert.ToBoolean(item.IsActive)),


                });
            }
            return View(UsersViewModel);
        }

        public ActionResult UserManagers()
        {

            return View();
        }
        public ActionResult GetAllUserManager()
        {

            IQueryable<AspNetUser> AspNetUser = db.AspNetUsers;



            var UsersViewModel = new List<UsersViewModel>();
            foreach (var item in AspNetUser)
            {
                UsersViewModel.Add(new UsersViewModel
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    Email = item?.Email,
                    RoleCurrent = RoleManager.GetRole(item.Id),
                    // IsActive = GetIsActiveName(Convert.ToBoolean(item.IsActive)),


                });
            }
            return Json(new { data = UsersViewModel }, JsonRequestBehavior.AllowGet);
        }







        public ActionResult Users()
        {
            IQueryable<AspNetUser> AspNetUser = db.AspNetUsers;


            var UserList = new List<UsersViewModel>();
            foreach (var item in AspNetUser)
            {
                UserList.Add(new UsersViewModel
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    Email = item?.Email,
                    RoleCurrent = RoleManager.GetRole(item.Id),
                    // IsActive = GetIsActiveName(Convert.ToBoolean(item.IsActive)),


                });
            }

            return Json(new { data = UserList.ToList() }, JsonRequestBehavior.AllowGet);

        }


        ////////////////////////////////////////////UserManager///////////////////////////////////////////////////////////////////////


    }

}