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
    public class PlaneRCoresController : Controller
    {
        private Entities db = new Entities();

        // GET: PlaneRCores

        public ActionResult PlaneRCores()
        {
           return View();
        }


        public ActionResult GetAllPlaneRCores()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var InOutCasesList = db.PlaneRCores.ToList();


            return Json(new { data = InOutCasesList }, JsonRequestBehavior.AllowGet);

        }







        public ActionResult Index()
        {
            return View(db.PlaneRCores.ToList());
        }

        // GET: PlaneRCores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaneRCore planeRCore = db.PlaneRCores.Find(id);
            if (planeRCore == null)
            {
                return HttpNotFound();
            }
            return View(planeRCore);
        }

        // GET: PlaneRCores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlaneRCores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PlaneID,PlaneName,Notes,UpdatedBy,UpdateDate,Wt,PlaneType")] PlaneRCore planeRCore)
        {
            if (ModelState.IsValid)
            {
                db.PlaneRCores.Add(planeRCore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(planeRCore);
        }

        // GET: PlaneRCores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaneRCore planeRCore = db.PlaneRCores.Find(id);
            if (planeRCore == null)
            {
                return HttpNotFound();
            }
            return View(planeRCore);
        }

        // POST: PlaneRCores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PlaneID,PlaneName,Notes,UpdatedBy,UpdateDate,Wt,PlaneType")] PlaneRCore planeRCore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planeRCore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(planeRCore);
        }

        // GET: PlaneRCores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaneRCore planeRCore = db.PlaneRCores.Find(id);
            if (planeRCore == null)
            {
                return HttpNotFound();
            }
            return View(planeRCore);
        }

        // POST: PlaneRCores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlaneRCore planeRCore = db.PlaneRCores.Find(id);
            db.PlaneRCores.Remove(planeRCore);
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
