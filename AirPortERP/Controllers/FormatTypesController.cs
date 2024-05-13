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
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class FormatTypesController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllFormatTypes()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var FormatTypesList = db.FormatTypes.ToList();


            return Json(new { data = FormatTypesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetFormatTypeParial(int? id)
        {

            FormatType FormatType = new FormatType();
            if (id > 0)
            {

                FormatType = db.FormatTypes.Find(id);

            }


            return PartialView("_AddEditFormatType", FormatType);

        }





        public ActionResult CreateOrUpdateFormatType(FormatType model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.FormatTypes.Any(x => x.Type == model.Type && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.FormatTypes.Any(x => x.Type == model.Type))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }


                    db.FormatTypes.Add(model);
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
            FormatType FormatType = db.FormatTypes.Find(id);
            if (FormatType == null)
            {
                return HttpNotFound();
            }

            if (db.PrintFormats.Any(x => x.FormatTypeId == FormatType.Id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            var FormatTypeDelete = db.FormatTypes.Find(id);
            db.FormatTypes.Remove(FormatTypeDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
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
