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
    public class NoiseFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();

        // GET: NoiseFees
        public ActionResult Index()
        {
            return View(db.NoiseFees.ToList());
        }


        public ActionResult GetAllNoiseFees()
        {

            IQueryable<NoiseFee> NoiseFees = db.NoiseFees;


            var NoiseFeesList = new List<NoiseFeeViewModels>();
            foreach (var item in NoiseFees)
            {
                NoiseFeesList.Add(new NoiseFeeViewModels
                {
                    Id = item.Id,
                    WieghtFrom = item.WieghtFrom,
                    WieghtTo = item.WieghtTo,
                    AmountInDollar = item.AmountInDollar.ToString(),
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }

            return Json(new { data = NoiseFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetNoiseFeesParial(int? id)
        {

            NoiseFee NoiseFees = new NoiseFee();
            if (id > 0)
            {

                NoiseFees = db.NoiseFees.Find(id);

            }


            return PartialView("_AddEditNoiseFees", NoiseFees);

        }

        public ActionResult CreateOrUpdateNoiseFees(NoiseFee model)
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
                    db.NoiseFees.Add(model);
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
            NoiseFee NoiseFees = db.NoiseFees.Find(id);
            if (NoiseFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.CountryId == NoiseFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var NoiseFeesDelete = db.NoiseFees.Find(id);
            db.NoiseFees.Remove(NoiseFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }





        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoiseFee NoiseFees = db.NoiseFees.Find(id);
            if (NoiseFees == null)
            {
                return HttpNotFound();
            }


            var item = db.NoiseFees.Single(x => x.Id == id);





            var NoiseFeesViewModelList = new NoiseFeeViewModels
            {





                Id = item.Id,
                WieghtFrom = item.WieghtFrom,
                WieghtTo=item.WieghtTo,
                AmountInDollar = item.AmountInDollar.ToString(),
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(NoiseFeesViewModelList);
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
