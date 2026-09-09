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
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;

namespace AirPortERP.Controllers
{
    public class CarServiceFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: CarServiceFees
        public ActionResult Index()
        {
            return View(db.CarServiceFees.ToList());
        }

       
        public ActionResult GetAllCarServiceFees()
        {
            IQueryable<CarServiceFee> CarServiceFee = db.CarServiceFees;


            var CarServiceFeeList = new List<CarServiceViewModels>();
            foreach (var item in CarServiceFee)
            {
                CarServiceFeeList.Add(new CarServiceViewModels
                {
                    Id = item.Id,
                    Name = item.Name,
                    AmountInDollar = item.AmountInDollar.ToString(),
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }
                return Json(new { data = CarServiceFeeList }, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult GetCarServiceFeesParial(int? id)
        {
           
            CarServiceFee CarServiceFees = new CarServiceFee();
            if (id > 0)
            {

                
                CarServiceFees = db.CarServiceFees.Find(id);

            }


            return PartialView("_AddEditCarServiceFees", CarServiceFees);

        }

        public ActionResult CreateOrUpdateCarServiceFees(CarServiceFee model)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.Identity.GetUserId();
                if (model.Id > 0)
                {

                    model.UpdatedBy = UserId;
                    model.UpdatingDate = System.DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.CreatedBy = UserId;
                    model.CreationDate = System.DateTime.Now;
                    db.CarServiceFees.Add(model);
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
            CarServiceFee CarServiceFees = db.CarServiceFees.Find(id);
            if (CarServiceFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.Id == CarServiceFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var CarServiceFeesDelete = db.CarServiceFees.Find(id);
            db.CarServiceFees.Remove(CarServiceFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }





        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarServiceFee CarServiceFees = db.CarServiceFees.Find(id);
            if (CarServiceFees == null)
            {
                return HttpNotFound();
            }


            var item = db.CarServiceFees.Single(x => x.Id == id);





            var CarServiceFeesViewModelList = new CarServiceViewModels
            {

             

                

                Id = item.Id,
                Name = item.Name,
                AmountInDollar = item.AmountInDollar.ToString(),
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(CarServiceFeesViewModelList);
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
