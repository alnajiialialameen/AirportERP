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
using Microsoft.AspNet.Identity;
using AirPortERP.Extenstion;
namespace AirPortERP.Controllers
{
    public class WorkOnsController : Controller
    {
        private Entities db = new Entities();
        CommonUsed CommonUsed  = new CommonUsed();
        // GET: WorkOns
        public ActionResult Index()
        {
         
            return View();
        }
        


        public ActionResult WorkOnUpdate()
        {

            return View();
        }

        // GET: WorkOns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkOn workOn = db.WorkOns.Find(id);
            if (workOn == null)
            {
                return HttpNotFound();
            }
            return View(workOn);
        }

        // GET: WorkOns/Create
  

        // GET: WorkOns/Edit/5
     

      
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkOn workOn = db.WorkOns.Find(id);
            if (workOn == null)
            {
                return HttpNotFound();
            }
            return View(workOn);
        }

        // POST: WorkOns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkOn workOn = db.WorkOns.Find(id);
            db.WorkOns.Remove(workOn);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult GetAllWorkOn()
        {
            var id = User.Identity.GetUserId();
            IQueryable<WorkOn> WorkOn = db.WorkOns.Where(x => x.UserId == id);


            var WorkOnList = new List<WorkOnViewModel>();
            foreach (var item in WorkOn)
            {
              //  AirLinesFlightTripsList.Add(new AirLinesFlightTripsViewMosels
                WorkOnViewModel A = new WorkOnViewModel();
                A.Id = item.Id;
                A.MouthWorkOn = item?.MouthWorkOn.Mouth;
                A.YearWorkOn = Convert.ToInt32(item?.YearWorkOn.Year);



                WorkOnList.Add(A);

            }

            return Json(new { data = WorkOnList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWorkOnParial(int? id)
        {
        

            WorkOn WorkOn = new WorkOn();

            if (id > 0)
            {

                WorkOn = db.WorkOns.Find(id);

            }
          

            ViewBag.YearWorkOnId = new SelectList(db.YearWorkOns, "Id", "Year", WorkOn.YearWorkOnId);
            ViewBag.MouthWorkOnId = new SelectList(db.MouthWorkOns, "Id", "Mouth", WorkOn.MouthWorkOnId);

            return PartialView("_AddEditWorkOn", WorkOn);

        }

        public ActionResult CreateOrUpdateWorkOn(WorkOn model)
        {
            var id = User.Identity.GetUserId();
          
            CommonUsed.HasMouth(id);
            if (ModelState.IsValid)
            {

          
                if (model.Id > 0)
                {
                    model.UserId = id;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                   
                }
                else
                {

                    model.UserId = id;
                    db.WorkOns.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                 
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
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
