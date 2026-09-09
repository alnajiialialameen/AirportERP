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
using AirPortERP.ViewModels;

namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class AircraftTypesController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult GetAllAircraftTypes()
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var AircraftTypesList = db.AircraftTypes.ToList();


        //    return Json(new { data = AircraftTypesList }, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult GetAllAircraftTypes()
        {

            IQueryable<AircraftType> AirPorts = db.AircraftTypes;


            var AircraftTypeList = new List<AircraftTypeModel>();
            foreach (var item in AirPorts)
            {
                AircraftTypeModel A = new AircraftTypeModel();
                A.Id = item.Id;
                A.Type = item.Type;
               // A.MaxTakoffWieght = Convert.ToDecimal(item.MaxTakoffWieght);

                if (item.SizeId > 0)
                {
                    A.SizeName = item.AircraftSize.Size;
                }
                else { A.SizeName = "Not Defined"; }

                AircraftTypeList.Add(A);
            }

            return Json(new { data = AircraftTypeList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAircraftTypeParial(int? id)
        {




            AircraftType AircraftType = new AircraftType();
           
            if (id > 0)
            {

                AircraftType = db.AircraftTypes.Find(id);
               
            }
            ViewBag.SizeId = new SelectList(db.AircraftSizes, "Id", "Size", AircraftType.SizeId);


            return PartialView("_AddEditAircraftType", AircraftType);

        }

        public ActionResult CreateOrUpdateAircraftType(AircraftType model)
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


                    db.AircraftTypes.Add(model);
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
            AircraftType AircraftType = db.AircraftTypes.Find(id);
            if (AircraftType == null)
            {
                return HttpNotFound();
            }

            if (db.AircraftRegistrations.Any(x => x.AircraftTypeId == AircraftType.Id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            var AircraftTypeDelete = db.AircraftTypes.Find(id);
            db.AircraftTypes.Remove(AircraftTypeDelete);
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
