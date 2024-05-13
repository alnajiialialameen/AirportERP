using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using AirPortERP.ViewModels;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;

namespace AirPortERP.Controllers
{
    public class LandingServiceFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();

        // GET: LandingServiceFees
        public ActionResult Index()
        {
            return View(db.LandingServiceFees.ToList());
        }

        public ActionResult GetAllLandingServiceFees()
        {

            IQueryable<LandingServiceFee> CarServiceFee = db.LandingServiceFees;


            var LandingServiceFeesList = new List<LandingServiceFeeViewModels>();
            foreach (var item in CarServiceFee)
            {
                LandingServiceFeesList.Add(new LandingServiceFeeViewModels
                {
                    Id = item.Id,
                    WieghtFrom=item.WieghtFrom,
                    WieghtTo = item.WieghtTo,
                    AmountInDollar = item.AmountInDollar,
                    CheckAppendWieght =item.CheckAppendWieght,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationData = CommonUsed.CheckDate(item.CreationData.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }
                return Json(new { data = LandingServiceFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetLandingServiceFeesParial(int? id)
        {

            LandingServiceFee LandingServiceFees = new LandingServiceFee();
            if (id > 0)
            {

                LandingServiceFees = db.LandingServiceFees.Find(id);

            }


            return PartialView("_AddEditLandingServiceFees", LandingServiceFees);

        }

        public ActionResult CreateOrUpdateLandingServiceFees(LandingServiceFee model)
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
                    model.CreationData = System.DateTime.Now;
                    db.LandingServiceFees.Add(model);
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
            LandingServiceFee LandingServiceFees = db.LandingServiceFees.Find(id);
            if (LandingServiceFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.CountryId == LandingServiceFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var LandingServiceFeesDelete = db.LandingServiceFees.Find(id);
            db.LandingServiceFees.Remove(LandingServiceFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptLandingServiceFees.rpt")));
            IQueryable<LandingServiceFee> LandingServiceFees;
            LandingServiceFees = db.LandingServiceFees;
            rd.SetDataSource(LandingServiceFees.Select(p => new
            {
                Id = p.Id,
                WieghtFrom = p.WieghtFrom,
                WieghtTo = p.WieghtTo,
                AmountInDollar = p.AmountInDollar,
                CheckAppendWieght9 = p.CheckAppendWieght,
                CreatedBy = p.CreatedBy,
                CreationData = p.CreationData,
                UpdatedBy = p.UpdatedBy,
                UpdatingDate = p.UpdatingDate,
            }).ToList());



            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream Stream = rd.ExportToStream
                //WordForWindows
                (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream.Seek(0, SeekOrigin.Begin);//application/doc
            return File(Stream, "application/pdf", "LandingServiceFees.pdf");
        }




        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LandingServiceFee LandingServiceFees = db.LandingServiceFees.Find(id);
            if (LandingServiceFees == null)
            {
                return HttpNotFound();
            }


            var item = db.LandingServiceFees.Single(x => x.Id == id);





            var LandingServiceFeesViewModelList = new LandingServiceFeeViewModels
            {




                    Id = item.Id,
                    WieghtFrom=item.WieghtFrom,
                    WieghtTo = item.WieghtTo,
                    AmountInDollar = item.AmountInDollar,
                    CheckAppendWieght =item.CheckAppendWieght,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationData = CommonUsed.CheckDate(item.CreationData.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


             

        };

            return View(LandingServiceFeesViewModelList);
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
