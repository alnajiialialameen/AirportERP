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

namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class AircraftRegistrationsController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetAllAircraftRegistration()
        {

            IQueryable<AircraftRegistration> AircraftRegistration = db.AircraftRegistrations;


            var AircraftRegistrationList = new List<AircraftRegistrationModels>();
            foreach (var item in AircraftRegistration)
            {
                // AircraftRegistrationList.Add(new AircraftRegistrationModels

                AircraftRegistrationModels A = new AircraftRegistrationModels();
                A.Id = item.Id;
                A.Registration = item?.Registration;

                if (Convert.ToInt32(item.AircraftTypeId) > 0)
                {
                    A.AircraftTypeName = item?.AircraftType.Type.ToString();
                }
                else {
                    A.AircraftTypeName = "Not Defined";
                }
              
                A.MaxTakoffWieght = Convert.ToDouble(item?.MaxTakoffWieght).ToString();


                AircraftRegistrationList.Add(A);

            }

            return Json(new { data = AircraftRegistrationList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAircraftRegistrationParial(int? id)
        {
            AircraftRegistration AircraftRegistration = new AircraftRegistration();
          
            if (id > 0)
            {

                AircraftRegistration = db.AircraftRegistrations.Find(id);
              //  string max = (AircraftRegistration.MaxTakoffWieght).ToString();
              //  AircraftRegistration.MaxTakoffWieght = Convert.ToDecimal(max);

            }
            ViewBag.AircraftTypeId = new SelectList(db.AircraftTypes, "Id", "Type", AircraftRegistration.AircraftTypeId);

            return PartialView("_AddEditAircraftRegistration", AircraftRegistration);
        }

        public ActionResult CreateOrUpdateAircraftRegistration(AircraftRegistration model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                 if (db.AircraftRegistrations.Any(x => x.Registration == model.Registration && x.Id != model.Id)) 
                 {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                  }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.AircraftRegistrations.Any(x => x.Registration == model.Registration))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    db.AircraftRegistrations.Add(model);
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
            AircraftRegistration AircraftRegistration = db.AircraftRegistrations.Find(id);
            if (AircraftRegistration == null)
            {
                return HttpNotFound();
            }

            //if (db.Countries.Any(x => x.Id == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var AircraftRegistrationDelete = db.AircraftRegistrations.Find(id);
            db.AircraftRegistrations.Remove(AircraftRegistrationDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult PrintData()
        {
          
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptAircraftRegistrations.rpt")));
            IQueryable<AircraftRegistration> AircraftRegistrations;
            AircraftRegistrations = db.AircraftRegistrations;

          
            var AircraftRegistrationModels = new List<AircraftRegistrationModels>();
            foreach (var item in AircraftRegistrations)
            {
                AircraftRegistrationModels R = new AircraftRegistrationModels();
                R.Id = item.Id;
                R.AircraftTypeName = item.AircraftType.Type;
                R.Registration = item.Registration;
                R.MaxTakoffWieght =Convert.ToDouble(item.MaxTakoffWieght).ToString();
                R.Size = item.AircraftType.AircraftSize.Size;

                AircraftRegistrationModels.Add(R);

            }


            rd.SetDataSource(AircraftRegistrationModels.Select(p => new
            {

                Registration = p.Registration,
                Type = p.AircraftTypeName,
             ///   AirLineName = p.AirLine.Name,
                MaxTakoffWieght = p.MaxTakoffWieght,
                Size = p.Size,

            }).ToList());



            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream Stream = rd.ExportToStream
                //WordForWindows
                (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream.Seek(0, SeekOrigin.Begin);//application/doc
            return File(Stream, "application/pdf", "Registrations.pdf");
        }








































































        // GET: AircraftRegistrations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AircraftRegistration aircraftRegistration = db.AircraftRegistrations.Find(id);
            if (aircraftRegistration == null)
            {
                return HttpNotFound();
            }
            return View(aircraftRegistration);
        }

        // GET: AircraftRegistrations/Create
        public ActionResult Create()
        {
            ViewBag.AircraftTypeId = new SelectList(db.AircraftTypes, "Id", "AircraftTypeName");
            return View();
        }

        // POST: AircraftRegistrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Registration,AircraftTypeId")] AircraftRegistration aircraftRegistration)
        {
            if (ModelState.IsValid)
            {
                db.AircraftRegistrations.Add(aircraftRegistration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AircraftTypeId = new SelectList(db.AircraftTypes, "Id", "AircraftTypeName", aircraftRegistration.AircraftTypeId);
            return View(aircraftRegistration);
        }

        // GET: AircraftRegistrations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AircraftRegistration aircraftRegistration = db.AircraftRegistrations.Find(id);
            if (aircraftRegistration == null)
            {
                return HttpNotFound();
            }
            ViewBag.AircraftTypeId = new SelectList(db.AircraftTypes, "Id", "AircraftTypeName", aircraftRegistration.AircraftTypeId);
            return View(aircraftRegistration);
        }

        // POST: AircraftRegistrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Registration,AircraftTypeId")] AircraftRegistration aircraftRegistration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aircraftRegistration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AircraftTypeId = new SelectList(db.AircraftTypes, "Id", "AircraftTypeName", aircraftRegistration.AircraftTypeId);
            return View(aircraftRegistration);
        }

        // GET: AircraftRegistrations/Delete/5
       

        // POST: AircraftRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AircraftRegistration aircraftRegistration = db.AircraftRegistrations.Find(id);
            db.AircraftRegistrations.Remove(aircraftRegistration);
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
