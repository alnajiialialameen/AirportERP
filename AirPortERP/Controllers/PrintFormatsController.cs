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
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class PrintFormatsController : Controller
    {
        private Entities db = new Entities();

        // GET: PrintFormats
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetAllPrintFormats()
        {

            IQueryable<PrintFormat> PrintFormat = db.PrintFormats;


            var PrintFormatList = new List<PrintFormatsViewModels>();
            foreach (var item in PrintFormat)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                PrintFormatsViewModels r = new PrintFormatsViewModels();
                r.Id = item.Id;
                r.Type = item.FormatType.Type.ToString();

                //});
                PrintFormatList.Add(r);
            }

            return Json(new { data = PrintFormatList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPrintFormatParial(int? id)
        {




            PrintFormat PrintFormat = new PrintFormat();
            ViewBag.FormatTypeId = new SelectList(db.FormatTypes, "Id", "Type", PrintFormat.FormatTypeId);
            if (id > 0)
            {

                PrintFormat = db.PrintFormats.Find(id);
                ViewBag.FormatTypeId = new SelectList(db.FormatTypes, "Id", "Type", PrintFormat.FormatTypeId);
            }


            return PartialView("_AddEditPrintFormat", PrintFormat);

        }

        public ActionResult CreateOrUpdatePrintFormat(PrintFormat model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.PrintFormats.Any(x => x.FormatTypeId == model.FormatTypeId && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.PrintFormats.Any(x => x.FormatTypeId == model.FormatTypeId))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.PrintFormats.Add(model);
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
            PrintFormat PrintFormat = db.PrintFormats.Find(id);
            if (PrintFormat == null)
            {
                return HttpNotFound();
            }

            //if (db.AirLinesFlightTrips.Any(x => x.AirPortId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            //var PrintFormatDelete = db.PrintFormats.Find(id);
            //db.PrintFormats.Remove(PrintFormatDelete);
            //db.SaveChanges();
            //return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
        }

        // GET: PrintFormats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrintFormat printFormat = db.PrintFormats.Find(id);
            if (printFormat == null)
            {
                return HttpNotFound();
            }
            return View(printFormat);
        }

        // GET: PrintFormats/Create
        public ActionResult Create()
        {
            ViewBag.FormatTypeId = new SelectList(db.FormatTypes, "Id", "Type");
            return View();
        }

        // POST: PrintFormats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FormatTypeId")] PrintFormat printFormat)
        {
            if (ModelState.IsValid)
            {
                db.PrintFormats.Add(printFormat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FormatTypeId = new SelectList(db.FormatTypes, "Id", "Type", printFormat.FormatTypeId);
            return View(printFormat);
        }

        // GET: PrintFormats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrintFormat printFormat = db.PrintFormats.Find(id);
            if (printFormat == null)
            {
                return HttpNotFound();
            }
            ViewBag.FormatTypeId = new SelectList(db.FormatTypes, "Id", "Type", printFormat.FormatTypeId);
            return View(printFormat);
        }

        // POST: PrintFormats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FormatTypeId")] PrintFormat printFormat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(printFormat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FormatTypeId = new SelectList(db.FormatTypes, "Id", "Type", printFormat.FormatTypeId);
            return View(printFormat);
        }

        // GET: PrintFormats/Delete/5
     

        // POST: PrintFormats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrintFormat printFormat = db.PrintFormats.Find(id);
            db.PrintFormats.Remove(printFormat);
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
