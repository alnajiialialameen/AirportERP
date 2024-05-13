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
using AirPortERP.Extenstion;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;

namespace AirPortERP.Controllers
{
    public class PushBackFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: PushBackFees
        public ActionResult Index()
        {
            return View(db.PushBackFees.ToList());
        }


        public ActionResult GetAllPushBackFees()
        {
            IQueryable<PushBackFee> PushBackFees = db.PushBackFees;


            var PushBackFeesList = new List<PushBackFeesViewModels>();
            foreach (var item in PushBackFees)
            {
                PushBackFeesList.Add(new PushBackFeesViewModels
                {
                    Id = item.Id,
                    AireCraftSize = item.AireCraftSize,
                    AmountInDollar = item.AmountInDollar,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }


            return Json(new { data = PushBackFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetPushBackFeesParial(int? id)
        {

            PushBackFee PushBackFees = new PushBackFee();
            if (id > 0)
            {

                PushBackFees = db.PushBackFees.Find(id);

            }


            return PartialView("_AddEditPushBackFees", PushBackFees);

        }

        public ActionResult CreateOrUpdatePushBackFees(PushBackFee model)
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
                    db.PushBackFees.Add(model);
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
            PushBackFee PushBackFees = db.PushBackFees.Find(id);
            if (PushBackFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.Id == PushBackFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var CarServiceFeesDelete = db.CarServiceFees.Find(id);
            db.CarServiceFees.Remove(CarServiceFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptPushBackFees.rpt")));
            IQueryable<PushBackFee> PushBackFees;
            PushBackFees = db.PushBackFees;
            rd.SetDataSource(PushBackFees.Select(p => new
            {
                Id = p.Id,
                AireCraftSize = p.AireCraftSize,
                AmountInDollar = p.AmountInDollar,
                CreatedBy = p.CreatedBy,
                CreationDate = p.CreationDate,
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
            return File(Stream, "application/pdf", "PushBackFees.pdf");
        }



        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PushBackFee PushBackFees = db.PushBackFees.Find(id);
            if (PushBackFees == null)
            {
                return HttpNotFound();
            }


            var item = db.PushBackFees.Single(x => x.Id == id);





            var PushBackFeesViewModelList = new PushBackFeesViewModels
            {

                Id = item.Id,
                AireCraftSize = item.AireCraftSize,
                AmountInDollar = item.AmountInDollar,
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(PushBackFeesViewModelList);
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
