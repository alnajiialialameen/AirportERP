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

namespace AirPortERP.Controllers
{
    public class FlightNoCoresController : Controller
    {
        private Entities db = new Entities();

        // GET: FlightNoCores
        public ActionResult Index()
        {
            return View(db.FlightNoCores.ToList());
        }

        public ActionResult GetAllFlightNoCores()
        {
            IQueryable<FlightNoCore> FlightNoCore = db.FlightNoCores;


            var FlightNoCoreList = new List<FlightNoCoreViewModels>();
            foreach (var item in FlightNoCore)
            {
                FlightNoCoreList.Add(new FlightNoCoreViewModels
                {
                    id = item.id,
                    FlightNo = item.FlightNo.ToString(),
                    CompanyCode = item.CompanyCode,
                    FlightTo = item.FlightTo.ToString(),



                });

            }
            return Json(new { data = FlightNoCoreList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetFlightNoCoresParial(int? id)
        {

            FlightNoCore FlightNoCores = new FlightNoCore();
            if (id > 0)
            {


                FlightNoCores = db.FlightNoCores.Find(id);

            }


            return PartialView("_AddEditFlightNoCores", FlightNoCores);

        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightNoCore FlightNoCore = db.FlightNoCores.Find(id);
            if (FlightNoCore == null)
            {
                return HttpNotFound();
            }
            return View(FlightNoCore);
        }

        // GET: FlightNoCores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FlightNoCores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,FlightNo,CompanyCode,FlightTo")] FlightNoCore FlightNoCore)
        {
            if (ModelState.IsValid)
            {
                db.FlightNoCores.Add(FlightNoCore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(FlightNoCore);
        }

        // GET: FlightNoCores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightNoCore FlightNoCore = db.FlightNoCores.Find(id);
            if (FlightNoCore == null)
            {
                return HttpNotFound();
            }
            return View(FlightNoCore);
        }

        // POST: FlightNoCores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,FlightNo,CompanyCode,FlightTo")] FlightNoCore FlightNoCore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(FlightNoCore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(FlightNoCore);
        }

        // GET: FlightNoCores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightNoCore FlightNoCore = db.FlightNoCores.Find(id);
            if (FlightNoCore == null)
            {
                return HttpNotFound();
            }
            return View(FlightNoCore);
        }

        // POST: FlightNoCores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FlightNoCore FlightNoCore = db.FlightNoCores.Find(id);
            db.FlightNoCores.Remove(FlightNoCore);
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
