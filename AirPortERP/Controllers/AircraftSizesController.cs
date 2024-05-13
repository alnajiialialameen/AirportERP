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
    public class AircraftSizesController : Controller
    {
        private Entities db = new Entities();

      
        // GET: AircraftSizes
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetAllAircraftSize()
        {

            db.Configuration.ProxyCreationEnabled = false;
            //var data = db.AircraftSizes.Select(new 
            //{
            //    asd = 
            //});
            IQueryable<AircraftSize> AircraftSize = db.AircraftSizes;


            //var AircraftRegistrationList = new List<AircraftRegistrationModels>();
            //foreach (var item in AircraftRegistration)
            //{
            //    AircraftRegistrationList.Add(new AircraftRegistrationModels
            //    {
            //        Id = item.Id,
            //        Registration = item?.Registration,
            //        AircraftTypeName = item?.AircraftType.Type,



            //    });
            //}

            return Json(new { data = AircraftSize.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAircraftSizeParial(int? id)
        {




            AircraftSize AircraftSize = new AircraftSize();

            if (id > 0)
            {

                AircraftSize = db.AircraftSizes.Find(id);


            }
           // ViewBag.AircraftTypeId = new SelectList(db.AircraftTypes, "Id", "Type", AircraftSize.AircraftTypeId);

            return PartialView("_AddEditAircraftSize", AircraftSize);

        }

        public ActionResult CreateOrUpdateAircraftSize(AircraftSize model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.AircraftSizes.Any(x => x.Size == model.Size && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }




                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.AircraftSizes.Any(x => x.Size == model.Size))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    db.AircraftSizes.Add(model);
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
            AircraftSize AircraftSize = db.AircraftSizes.Find(id);
            if (AircraftSize == null)
            {
                return HttpNotFound();
            }

            if (db.AircraftTypes.Any(x => x.SizeId == id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            var SizeDelete = db.AircraftSizes.Find(id);
            db.AircraftSizes.Remove(SizeDelete);
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
