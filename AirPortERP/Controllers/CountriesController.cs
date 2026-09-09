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
using System.Threading;
namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin")]
    public class CountriesController : Controller
    {
        private Entities db = new Entities();

        // GET: Countries
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllCountries()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var InOutCasesList = db.Countries.ToList();


            return Json(new { data = InOutCasesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetCountryParial(int? id)
        {

            Country Country = new Country();
            if (id > 0)
            {

                Country = db.Countries.Find(id);

            }


            return PartialView("_AddEditCountry", Country);

        }





        public ActionResult CreateOrUpdateCountry(Country model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.Countries.Any(x => x.Code == model.Code && x.Id != model.Id)) 
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.Countries.Any(x => x.Code == model.Code))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }


                    db.Countries.Add(model);
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
            Country Country = db.Countries.Find(id);
            if (Country == null)
            {
                return HttpNotFound();
            }

            if (db.AirPorts.Any(x => x.CountryId == Country.Id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            var InOutCaseDelete = db.Countries.Find(id);
            db.Countries.Remove(InOutCaseDelete);
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
