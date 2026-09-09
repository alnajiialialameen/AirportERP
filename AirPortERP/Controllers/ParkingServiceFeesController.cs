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
    public class ParkingServiceFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: ParkingServiceFees
        public ActionResult Index()
        {
            return View(db.ParkingServiceFees.ToList());
        }


        public ActionResult GetAllParkingServiceFees()
        {

            IQueryable<ParkingServiceFee> ParkingServiceFees = db.ParkingServiceFees;


            var ParkingServiceFeesList = new List<ParkingServiceFeesViewModels>();
            foreach (var item in ParkingServiceFees)
            {
                ParkingServiceFeesList.Add(new ParkingServiceFeesViewModels
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


            return Json(new { data = ParkingServiceFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetParkingServiceFeesParial(int? id)
        {

            ParkingServiceFee ParkingServiceFees = new ParkingServiceFee();
            if (id > 0)
            {

                ParkingServiceFees = db.ParkingServiceFees.Find(id);

            }


            return PartialView("_AddEditParkingServiceFees", ParkingServiceFees);

        }

        public ActionResult CreateOrUpdateParkingServiceFees(ParkingServiceFee model)
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
                    db.ParkingServiceFees.Add(model);
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
            ParkingServiceFee ParkingServiceFees = db.ParkingServiceFees.Find(id);
            if (ParkingServiceFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.CountryId == ParkingServiceFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var ParkingServiceFeesDelete = db.ParkingServiceFees.Find(id);
            db.ParkingServiceFees.Remove(ParkingServiceFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }







        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParkingServiceFee ParkingServiceFees = db.ParkingServiceFees.Find(id);
            if (ParkingServiceFees == null)
            {
                return HttpNotFound();
            }


            var item = db.ParkingServiceFees.Single(x => x.Id == id);





            var ParkingServiceFeesViewModelList = new ParkingServiceFeesViewModels
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

            return View(ParkingServiceFeesViewModelList);
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
