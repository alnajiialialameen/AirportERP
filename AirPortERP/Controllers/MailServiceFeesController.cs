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
    public class MailServiceFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();

        // GET: MailServiceFees
        public ActionResult Index()
        {
            return View(db.MailServiceFees.ToList());
        }


        public ActionResult GetAllMailServiceFees()
        {

            IQueryable<MailServiceFee> MailServiceFees = db.MailServiceFees;


            var MailServiceFeesList = new List<MailServiceFeeViewModels>();
            foreach (var item in MailServiceFees)
            {
                MailServiceFeesList.Add(new MailServiceFeeViewModels
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


            return Json(new { data = MailServiceFeesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetMailServiceFeesParial(int? id)
        {

            MailServiceFee MailServiceFees = new MailServiceFee();
            if (id > 0)
            {

                MailServiceFees = db.MailServiceFees.Find(id);

            }


            return PartialView("_AddEditMailServiceFees", MailServiceFees);

        }

        public ActionResult CreateOrUpdateMailServiceFees(MailServiceFee model)
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
                    db.MailServiceFees.Add(model);
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
            MailServiceFee MailServiceFees = db.MailServiceFees.Find(id);
            if (MailServiceFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.Id == MailServiceFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var MailServiceFeesDelete = db.MailServiceFees.Find(id);
            db.MailServiceFees.Remove(MailServiceFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptMailServiceFees.rpt")));
            IQueryable<MailServiceFee> MailServiceFees;
            MailServiceFees = db.MailServiceFees;
            rd.SetDataSource(MailServiceFees.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
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
            return File(Stream, "application/pdf", "MailServiceFees.pdf");
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailServiceFee MailServiceFees = db.MailServiceFees.Find(id);
            if (MailServiceFees == null)
            {
                return HttpNotFound();
            }


            var item = db.MailServiceFees.Single(x => x.Id == id);





            var MailServiceFeesViewModelList = new MailServiceFeeViewModels
            {





                Id = item.Id,
                Name = item.Name,
                AmountInDollar = item.AmountInDollar.ToString(),
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(MailServiceFeesViewModelList);
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
