using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;

namespace AirPortERP.Controllers
{
    public class FlightRCoresController : Controller
    {
        private Entities db = new Entities();



        public ActionResult FlightRCores()
        {
            return View();
        }

        public ActionResult GetAllFlightRCores()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var InOutCasesList = db.FlightRCores.ToList();


            return Json(new { data = InOutCasesList }, JsonRequestBehavior.AllowGet);

        }



        // GET: FlightRCores
        public ActionResult Index()
        {
            return View(db.FlightRCores.ToList());
        }

        // GET: FlightRCores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightRCore flightRCore = db.FlightRCores.Find(id);
            if (flightRCore == null)
            {
                return HttpNotFound();
            }
            return View(flightRCore);
        }

        // GET: FlightRCores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FlightRCores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FlightNo,FlightName,CreationDate,CreatedBy,UpdatedBy,UpdateDate,Destination,AirlineId,IsDomistic")] FlightRCore flightRCore)
        {
            if (ModelState.IsValid)
            {
                db.FlightRCores.Add(flightRCore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(flightRCore);
        }

        // GET: FlightRCores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightRCore flightRCore = db.FlightRCores.Find(id);
            if (flightRCore == null)
            {
                return HttpNotFound();
            }
            return View(flightRCore);
        }

        // POST: FlightRCores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FlightNo,FlightName,CreationDate,CreatedBy,UpdatedBy,UpdateDate,Destination,AirlineId,IsDomistic")] FlightRCore flightRCore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flightRCore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(flightRCore);
        }

        // GET: FlightRCores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightRCore flightRCore = db.FlightRCores.Find(id);
            if (flightRCore == null)
            {
                return HttpNotFound();
            }
            return View(flightRCore);
        }

        // POST: FlightRCores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FlightRCore flightRCore = db.FlightRCores.Find(id);
            db.FlightRCores.Remove(flightRCore);
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
