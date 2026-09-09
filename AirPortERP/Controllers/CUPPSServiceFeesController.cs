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
using AirPortERP.ViewModels;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;

namespace AirPortERP.Controllers
{
    public class CUPPSServiceFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();

        // GET: CUPPSServiceFees
        public ActionResult Index()
        {
            return View(db.CUPPSServiceFees.ToList());
        }

        public ActionResult GetAllCUPPSServiceFees()
        {
            IQueryable<CUPPSServiceFee> CUPPSServiceFees = db.CUPPSServiceFees;


            var CUPPSServiceFeesList = new List<CUPPSServiceFeeViewModels>();
            foreach (var item in CUPPSServiceFees)
            {
                CUPPSServiceFeesList.Add(new CUPPSServiceFeeViewModels
                {
                    Id = item.Id,
                    Name = item.Name,
                    AmountInDollar = item.AmountInDollar.ToString(),
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });
            }


            return Json(new { data = CUPPSServiceFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetCUPPSServiceFeesParial(int? id)
        {

            CUPPSServiceFee CUPPSServiceFees = new CUPPSServiceFee();
            if (id > 0)
            {

                CUPPSServiceFees = db.CUPPSServiceFees.Find(id);

            }


            return PartialView("_AddEditCUPPSServiceFees", CUPPSServiceFees);

        }
        public ActionResult CreateOrUpdateCUPPSServiceFees(CUPPSServiceFee model)
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
                    db.CUPPSServiceFees.Add(model);
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
            CUPPSServiceFee CUPPSServiceFees = db.CUPPSServiceFees.Find(id);
            if (CUPPSServiceFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.CountryId == CUPPSServiceFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var CUPPSServiceFeesDelete = db.CUPPSServiceFees.Find(id);
            db.CUPPSServiceFees.Remove(CUPPSServiceFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUPPSServiceFee CUPPSServiceFees = db.CUPPSServiceFees.Find(id);
            if (CUPPSServiceFees == null)
            {
                return HttpNotFound();
            }


            var item = db.CUPPSServiceFees.Single(x => x.Id == id);





            var CUPPSServiceFeesViewModelList = new CUPPSServiceFeeViewModels
            {





                Id = item.Id,
                Name = item.Name,
                AmountInDollar = item.AmountInDollar.ToString(),
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(CUPPSServiceFeesViewModelList);
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
