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
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Threading;
namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class AirPortsController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }

    

        public ActionResult GetAllAirPorts()
        {

            IQueryable<AirPort> AirPorts = db.AirPorts;


            var AirPortsList = new List<AirPortsViewMosels>();
            foreach (var item in AirPorts)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                AirPortsViewMosels r = new AirPortsViewMosels();
                r.Id = item.Id;
                r.NameAr = item?.NameAr;
                r.NameEn = item?.NameEn;
                r.Code = item?.Code;
                if (item.CountryId > 0)
                {
                    r.CountryName = item?.Country.NameEn;
                }
                else {
                    r.CountryName = "Not Define";
                }

                //});
                AirPortsList.Add(r);
            }

            return Json(new { data = AirPortsList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAirPortParial(int? id)
        {

           


            AirPort AirPort = new AirPort();
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "NameAr", AirPort.CountryId);
            if (id > 0)
            {

                AirPort = db.AirPorts.Find(id);
                ViewBag.CountryId = new SelectList(db.Countries, "Id", "NameAr", AirPort.CountryId);
            }


            return PartialView("_AddEditAirPort", AirPort);

        }

        public ActionResult CreateOrUpdateAirPort(AirPort model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.AirPorts.Any(x => x.Code == model.Code && x.Id!=model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.AirPorts.Any(x => x.Code == model.Code))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.AirPorts.Add(model);
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
            AirPort AirPort = db.AirPorts.Find(id);
            if (AirPort == null)
            {
                return HttpNotFound();
            }

            if (db.AirLinesFlightTrips.Any(x => x.AirPortId == id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            var AirPortDelete = db.AirPorts.Find(id);
            db.AirPorts.Remove(AirPortDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptAirPorts.rpt")));
            IQueryable<AirPort> AirPorts;
            AirPorts = db.AirPorts;
            rd.SetDataSource(AirPorts.Select(p => new
            {

                NameEn = p.NameEn,
                NameAr = p.NameAr,
                Code = p.Code,
                CountryName = p.Country.NameEn
            }).ToList());



            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream Stream = rd.ExportToStream
                //WordForWindows
                (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream.Seek(0, SeekOrigin.Begin);//application/doc
            return File(Stream, "application/pdf", "AirPorts.pdf");
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
