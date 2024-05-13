using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.ViewModels;

namespace AirPortERP.Controllers
{
    public class AccountGroupsController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllAccountGroups()
        {

            IQueryable<AccountGroup> Groups = db.AccountGroups;


            var GroupList = new List<AccountGroupViewModel>();
            foreach (var item in Groups)
            {

                AccountGroupViewModel r = new AccountGroupViewModel();
                r.Id = item.Id;
                r.Name = item?.Name;
                GroupList.Add(r);
            }
            return Json(new { data = GroupList.ToList() }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult GetGroupParial(int? id)
        {

            AccountGroup Group = new AccountGroup();
          //  ViewBag.CountryId = new SelectList(db.Countries, "Id", "NameAr", AirPort.CountryId);
            if (id > 0)
            {

                Group = db.AccountGroups.Find(id);
               // ViewBag.CountryId = new SelectList(db.Countries, "Id", "NameAr", AirPort.CountryId);
            }


            return PartialView("_AddEditGroup", Group);

        }


        public ActionResult CreateOrUpdateGroup(AccountGroup model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.AccountGroups.Any(x => x.Name == model.Name && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                 

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    if (db.AccountGroups.Any(x => x.Name == model.Name))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    int last = db.AccountGroups.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                    if (last >= 3)
                    {
                        return Json(new { success = true, message = "Over" }, JsonRequestBehavior.AllowGet);
                    }
                    db.AccountGroups.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountGroup Group = db.AccountGroups.Find(id);
            if (Group == null)
            {
                return HttpNotFound();
            }

            if (db.AccountTrees.Any(x => x.AccGroupId == id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            if (id == 1 || id == 2 || id == 3 || id == 4 || id == 5) 
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }



            var GroupDelete = db.AccountGroups.Find(id);
            db.AccountGroups.Remove(GroupDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }




    }
}
