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
using System.IO;

namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class AirLineAgentsController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetAllAirLineAgents()
        {

            IQueryable<AirLineAgent> AirLineAgent = db.AirLineAgents;
            var AirLineAgentsList = new List<AirLineAgentsViewMosels>();
            foreach (var item in AirLineAgent)
            {
                AirLineAgentsList.Add(new AirLineAgentsViewMosels
                {
                    Id = item.Id,
                    NameAr = item?.NameAr,
                    NameAn = item?.NameAn,
                    Phone1 = item?.Phone1,
                    Phone2 = item?.Phone2,
                    Email = item?.Email,
                    Address = item?.Address,
                    AirLineName = item?.AirLine.Name,
                    IsActive = GetActiveName(Convert.ToBoolean(item.IsActive)),


                });
            }

            return Json(new { data = AirLineAgentsList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAirLineAgentParial(int? id)
        {




            AirLineAgent AirLineAgent = new AirLineAgent();
          
            if (id > 0)
            {

                AirLineAgent = db.AirLineAgents.Find(id);
               
            }
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", AirLineAgent.AirLineId);

            return PartialView("_AddEditAirLineAgent", AirLineAgent);

        }

        public ActionResult CreateOrUpdateAirLineAgent(AirLineAgent model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.AirLineAgents.Any(x => x.NameAn == model.NameAn && x.Id!=model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.AirLineAgents.Any(x => x.NameAn == model.NameAn))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }


                    db.AirLineAgents.Add(model);
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
            AirLineAgent AirLineAgent = db.AirLineAgents.Find(id);
            if (AirLineAgent == null)
            {
                return HttpNotFound();
            }

            //if (db.AirLinesFlightTrips.Any(x => x.AirPortId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var AirLineAgentDelete = db.AirLineAgents.Find(id);
            db.AirLineAgents.Remove(AirLineAgentDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }







        public string GetActiveName(bool IsActive)
        {
            string StatusName;
            if (IsActive == false)
            {
                StatusName = "Not Active";
            }
            else
            {
                StatusName = "Active";
            }
            return StatusName;
        }

      




























        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirLineAgent airLineAgent = db.AirLineAgents.Find(id);
            if (airLineAgent == null)
            {
                return HttpNotFound();
            }
            return View(airLineAgent);
        }

        // GET: AirLineAgents/Create
        public ActionResult Create()
        {
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name");
            return View();
        }

        // POST: AirLineAgents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NameAr,NameAn,Phone1,Phone2,Email,Address,AirLineId,IsActive")] AirLineAgent airLineAgent)
        {
            if (ModelState.IsValid)
            {
                db.AirLineAgents.Add(airLineAgent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", airLineAgent.AirLineId);
            return View(airLineAgent);
        }

        // GET: AirLineAgents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirLineAgent airLineAgent = db.AirLineAgents.Find(id);
            if (airLineAgent == null)
            {
                return HttpNotFound();
            }
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", airLineAgent.AirLineId);
            return View(airLineAgent);
        }

        // POST: AirLineAgents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NameAr,NameAn,Phone1,Phone2,Email,Address,AirLineId,IsActive")] AirLineAgent airLineAgent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(airLineAgent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", airLineAgent.AirLineId);
            return View(airLineAgent);
        }

        // GET: AirLineAgents/Delete/5
    

        // POST: AirLineAgents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AirLineAgent airLineAgent = db.AirLineAgents.Find(id);
            db.AirLineAgents.Remove(airLineAgent);
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
