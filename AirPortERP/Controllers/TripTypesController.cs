using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class TripTypesController : Controller
    {
        private Entities db = new Entities();

        // GET: TripTypes

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetAllTripTypes()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var TripTypesList = db.TripTypes.ToList();


            return Json(new { data = TripTypesList }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetTripTypeParial(int? id)
        {




            TripType TripType = new TripType();
          
            if (id > 0)
            {

                TripType = db.TripTypes.Find(id);
               
            }


            return PartialView("_AddEditTripType", TripType);

        }

        public ActionResult CreateOrUpdateTripType(TripType model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {


                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    db.TripTypes.Add(model);
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
            TripType TripType = db.TripTypes.Find(id);
            if (TripType == null)
            {
                return HttpNotFound();
            }

            if (db.TowerDatas.Any(x => x.TripTypeId == id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            var TripTypeDelete = db.TripTypes.Find(id);
            db.TripTypes.Remove(TripTypeDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }












































































        // GET: TripTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TripType tripType = db.TripTypes.Find(id);
            if (tripType == null)
            {
                return HttpNotFound();
            }
            return View(tripType);
        }

        // GET: TripTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TripTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] TripType tripType)
        {
            if (ModelState.IsValid)
            {
                db.TripTypes.Add(tripType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tripType);
        }

        // GET: TripTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TripType tripType = db.TripTypes.Find(id);
            if (tripType == null)
            {
                return HttpNotFound();
            }
            return View(tripType);
        }

        // POST: TripTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] TripType tripType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tripType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tripType);
        }

        // GET: TripTypes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TripType tripType = db.TripTypes.Find(id);
        //    if (tripType == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tripType);
        //}

        // POST: TripTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TripType tripType = db.TripTypes.Find(id);
            db.TripTypes.Remove(tripType);
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
