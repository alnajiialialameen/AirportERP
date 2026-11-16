using AirPortERP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirPortERP.Extenstion
{
    public class RoleController : Controller
    {
        public readonly UserManager<ApplicationUser> userManager;
        Entities db = new Entities();

        public RoleController()
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

        public string GetRole(string id)
        {
            //ApplicationUser user = userManager.FindById(id);
            string rolename = userManager.GetRoles(id).FirstOrDefault();
            if (string.IsNullOrEmpty(id))
            {
                rolename = " ";

            }
           
               //Session["Role"] = rolename;
                ViewBag.Role = rolename;
                return rolename;
            
        }

   

        public int GetCompanyId(string id)
        {        
            ApplicationUser user = userManager.FindById(id);
          
            
            int? CompanyId = db.WorkOns.Where(x=>x.UserId == id).FirstOrDefault().CompanyInfoId;


            return Convert.ToInt32(CompanyId);

        }


    }
}