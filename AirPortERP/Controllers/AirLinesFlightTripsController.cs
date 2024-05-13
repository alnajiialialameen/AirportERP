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
using AirPortERP.Extenstion;
namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class AirLinesFlightTripsController : Controller
    {
        private CommonUsed CommonUsed = new CommonUsed();
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetAllAirLinesFlightTrips()
        {

            IQueryable<AirLinesFlightTrip> AirLinesFlightTrip = db.AirLinesFlightTrips;


            var AirLinesFlightTripsList = new List<AirLinesFlightTripsViewMosels>();
            foreach (var item in AirLinesFlightTrip)
            {
                //AirLinesFlightTripsList.Add(new AirLinesFlightTripsViewMosels
                AirLinesFlightTripsViewMosels A = new AirLinesFlightTripsViewMosels();
                A.Id = item.Id;
                A.FlightNo = item.FlightNo;
                A.AirLineName = item?.AirLine.Name;
                A.AirPortName = item?.AirPort.NameEn;
                A.AirPortCode = item?.AirPort.Code;
                if (item.AirPort.CountryId>0)
                {
                    A.Country = item?.AirPort.Country.Code;
                   
                }
                else
                {
                    A.Country = " ";
                }
                A.IsActive = GetIsActiveName(Convert.ToBoolean(item.IsActive));

                AirLinesFlightTripsList.Add(A);

            }

            return Json(new { data = AirLinesFlightTripsList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAirLinesFlightTripParial(int? id)
        {




            AirLinesFlightTrip AirLinesFlightTrip = new AirLinesFlightTrip();
        
            if (id > 0)
            {

                AirLinesFlightTrip = db.AirLinesFlightTrips.Find(id);
              
            }

            ViewBag.AirPortId = new SelectList(db.AirPorts, "Id", "Code", AirLinesFlightTrip.AirPortId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", AirLinesFlightTrip.AirLineId);
            return PartialView("_AddEditAirLinesFlightTrip", AirLinesFlightTrip);

        }

        public ActionResult CreateOrUpdateAirLinesFlightTrip(AirLinesFlightTrip model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {

                    if (db.AirLinesFlightTrips.Any(x => x.FlightNo == model.FlightNo && x.Id != model.Id && x.AirLineId==model.AirLineId))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    if (db.AirLinesFlightTrips.Any(x => x.FlightNo == model.FlightNo && x.AirLineId == model.AirLineId))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    db.AirLinesFlightTrips.Add(model);
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
            AirLinesFlightTrip AirLinesFlightTrip = db.AirLinesFlightTrips.Find(id);
            if (AirLinesFlightTrip == null)
            {
                return HttpNotFound();
            }

            //if (db.Countries.Any(x => x.Id == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var AirLinesFlightTripDelete = db.AirLinesFlightTrips.Find(id);
            db.AirLinesFlightTrips.Remove(AirLinesFlightTripDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }



        public string GetIsActiveName(bool Islocal)
        {
            string StatusName;
            if (Islocal == false)
            {
                StatusName = "غير نشط";
            }
            else
            {
                StatusName = "نشط";
            }
            return StatusName;
        }


        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptAirLinesFlightTrips.rpt")));
            IQueryable<AirLinesFlightTrip> AirLinesFlightTrip;
            AirLinesFlightTrip = db.AirLinesFlightTrips;


            var AirLinesFlightTrips = new List<AirLinesFlightTripsViewMosels>();
            foreach (var item in AirLinesFlightTrip)
            {

                AirLinesFlightTripsViewMosels A = new AirLinesFlightTripsViewMosels();
                A.FlightNo = item.FlightNo;
                A.AirPortName = item.AirPort.NameEn;
                A.AirLineName = item.AirLine.Name;
                A.IsActive = CommonUsed.isActive(item.IsActive);

               // A.AirPortName = item?.AirPort.NameEn;
                A.AirPortCode = item?.AirPort.Code;
                if (item.AirPort.CountryId > 0)
                {
                    A.Country = item?.AirPort.Country.Code;

                }
                else
                {
                    A.Country = " ";
                }





                AirLinesFlightTrips.Add(A);

            }









            rd.SetDataSource(AirLinesFlightTrips.Select(p => new
            {

                FlightNo = p.FlightNo,
                AirPortName = p.AirPortName,
                AirLineName = p.AirLineName,
                IsActive = p.IsActive,
                AirPortCode = p.AirPortCode,
                Country = p.Country,
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


































        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirLinesFlightTrip airLinesFlightTrip = db.AirLinesFlightTrips.Find(id);
            if (airLinesFlightTrip == null)
            {
                return HttpNotFound();
            }
            return View(airLinesFlightTrip);
        }

        // GET: AirLinesFlightTrips/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AirLinesFlightTrips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FlightNo,AirPortId,AirLineId,IsActive")] AirLinesFlightTrip airLinesFlightTrip)
        {
            if (ModelState.IsValid)
            {
                db.AirLinesFlightTrips.Add(airLinesFlightTrip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(airLinesFlightTrip);
        }

        // GET: AirLinesFlightTrips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirLinesFlightTrip airLinesFlightTrip = db.AirLinesFlightTrips.Find(id);
            if (airLinesFlightTrip == null)
            {
                return HttpNotFound();
            }
            return View(airLinesFlightTrip);
        }

        // POST: AirLinesFlightTrips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FlightNo,AirPortId,AirLineId,IsActive")] AirLinesFlightTrip airLinesFlightTrip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(airLinesFlightTrip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(airLinesFlightTrip);
        }

        // GET: AirLinesFlightTrips/Delete/5
      

        // POST: AirLinesFlightTrips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AirLinesFlightTrip airLinesFlightTrip = db.AirLinesFlightTrips.Find(id);
            db.AirLinesFlightTrips.Remove(airLinesFlightTrip);
            db.SaveChanges();
            return RedirectToAction("Index");
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
