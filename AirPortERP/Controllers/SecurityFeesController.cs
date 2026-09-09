using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using System.IO;
using AirPortERP.Extenstion;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;

namespace AirPortERP.Controllers
{
    public class SecurityFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: SecurityFees
        public ActionResult Index()
        {
            return View(db.SecurityFees.ToList());
        }


        public ActionResult GetAllSecurityFees()
        {

            IQueryable<SecurityFee> SecurityFees = db.SecurityFees;


            var SecurityFeesList = new List<SecurityFeesViewModels>();
            foreach (var item in SecurityFees)
            {
                SecurityFeesList.Add(new SecurityFeesViewModels
                {
                    Id = item.Id,
                    WieghtFrom = item.WieghtFrom,
                    WieghtTo = item.WieghtTo,
                    FlightType=item.FlightType,
                    AmountInDollar = item.AmountInDollar,
                    CheckAppendWieght = item.CheckAppendWieght,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }


            return Json(new { data = SecurityFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetSecurityFeesParial(int? id)
        {

            SecurityFee SecurityFees = new SecurityFee();
            if (id > 0)
            {

                SecurityFees = db.SecurityFees.Find(id);

            }


            return PartialView("_AddEditSecurityFees", SecurityFees);

        }

        public ActionResult CreateOrUpdateSecurityFees(SecurityFee model)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.Identity.GetUserId();
                if (model.Id > 0)
                {
                  
                    model.UpdatedBy = UserId;
                    model.UpdatingDate = System.DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.CreatedBy = UserId;
                    model.CreationDate = System.DateTime.Now;
                    db.SecurityFees.Add(model);
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
            SecurityFee SecurityFees = db.SecurityFees.Find(id);
            if (SecurityFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.CountryId == SecurityFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var SecurityFeesDelete = db.SecurityFees.Find(id);
            db.SecurityFees.Remove(SecurityFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SecurityFee SecurityFees = db.SecurityFees.Find(id);
            if (SecurityFees == null)
            {
                return HttpNotFound();
            }


            var item = db.SecurityFees.Single(x => x.Id == id);





            var SecurityFeesViewModelList = new SecurityFeesViewModels
            {




                Id = item.Id,
                WieghtFrom = item.WieghtFrom,
                WieghtTo = item.WieghtTo,
                FlightType=item.FlightType,
                AmountInDollar = item.AmountInDollar,
                CheckAppendWieght = item.CheckAppendWieght,
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),




            };

            return View(SecurityFeesViewModelList);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
