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
    public class StayInFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: StayInFees
        public ActionResult Index()
        {
            return View(db.StayInFees.ToList());
        }


        public ActionResult GetAllStayInFees()
        {
            IQueryable<StayInFee> StayInFees = db.StayInFees;


            var StayInFeesList = new List<StayInFeesViewModels>();
            foreach (var item in StayInFees)
            {
                StayInFeesList.Add(new StayInFeesViewModels
                {
                    Id = item.Id,
                    WieghtFrom = item.WieghtFrom,
                    WieghtTo = item.WieghtTo,
                 
                    AmountInDollar = item.AmountInDollar,
                    CheckAppendWieght = item.CheckAppendWieght,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }


            return Json(new { data = StayInFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetStayInFeesParial(int? id)
        {

            StayInFee StayInFees = new StayInFee();
            if (id > 0)
            {

                StayInFees = db.StayInFees.Find(id);

            }


            return PartialView("_AddEditStayInFees", StayInFees);

        }

        public ActionResult CreateOrUpdateStayInFees(StayInFee model)
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
                    db.StayInFees.Add(model);
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
            StayInFee StayInFees = db.StayInFees.Find(id);
            if (StayInFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.CountryId == LandingServiceFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var StayInFeesDelete = db.StayInFees.Find(id);
            db.StayInFees.Remove(StayInFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StayInFee StayInFees = db.StayInFees.Find(id);
            if (StayInFees == null)
            {
                return HttpNotFound();
            }


            var item = db.StayInFees.Single(x => x.Id == id);





            var StayInFeesViewModelList = new StayInFeesViewModels
            {




                Id = item.Id,
                WieghtFrom = item.WieghtFrom,
                WieghtTo = item.WieghtTo,
               
                AmountInDollar = item.AmountInDollar,
                CheckAppendWieght = item.CheckAppendWieght,
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),




            };

            return View(StayInFeesViewModelList);
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
