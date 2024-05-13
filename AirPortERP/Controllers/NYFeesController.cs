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
    public class NYFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: NYFees
        public ActionResult Index()
        {
            return View(db.NYFees.ToList());
        }

        public ActionResult GetAllNYFees()
        {
            IQueryable<NYFee> NYFees = db.NYFees.OrderBy(x=>x.Id);
            var NYFeesList = new List<NYFeesViewModels>();
            foreach (var item in NYFees)
            {
                NYFeesList.Add(new NYFeesViewModels
                {
                    Id = item.Id,
                    FlightTypeName = CommonUsed.GetFlightTypeForSetting(item.FlightType),
                    AmountInDollar = item.AmountInDollar,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),
                });
            }

            return Json(new { data = NYFeesList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNYFeesParial(int? id)
        {

            NYFee NYFees = new NYFee();
            if (id > 0)
            {

                NYFees = db.NYFees.Find(id);

            }


            return PartialView("_AddEditNYFees", NYFees);

        }

        public ActionResult CreateOrUpdateNYFees(NYFee model)
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
                    db.NYFees.Add(model);
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
            NYFee NYFees = db.NYFees.Find(id);
            if (NYFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.Id == NYFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var NYFeesDelete = db.NYFees.Find(id);
            db.NYFees.Remove(NYFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptNYFees.rpt")));
            IQueryable<NYFee> NYFees;
            NYFees = db.NYFees;
            rd.SetDataSource(NYFees.Select(p => new
            {
                Id = p.Id,
                FlightType = p.FlightType,
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
            return File(Stream, "application/pdf", "NYFees.pdf");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NYFee NYFees = db.NYFees.Find(id);
            if (NYFees == null)
            {
                return HttpNotFound();
            }


            var item = db.NYFees.Single(x => x.Id == id);





            var NYFeesViewModelList = new NYFeesViewModels
            {





                Id = item.Id,
                FlightType = item.FlightType,
                AmountInDollar = item.AmountInDollar,
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(NYFeesViewModelList);
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