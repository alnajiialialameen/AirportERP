using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.Extenstion;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;

namespace AirPortERP.Controllers
{
    public class FreightFeesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();


        // GET: FreightFees
        public ActionResult Index()
        {
            return View(db.FreightFees.ToList());
        }

        public ActionResult GetAllFreightFees()
        {

            IQueryable<FreightFee> FreightFees = db.FreightFees;


            var FreightFeesList = new List<FreightFeeViewModels>();
            foreach (var item in FreightFees)
            {
                FreightFeesList.Add(new FreightFeeViewModels
                {
                    Id = item.Id,
                    Name = item.Name,
                    AmountInDollar = item.AmountInDollar,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),
                });

            }

            return Json(new { data = FreightFeesList }, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult GetFreightFeesParial(int? id)
        {
            FreightFee FreightFees = new FreightFee();
            if (id > 0)
            {
                FreightFees = db.FreightFees.Find(id);
            }

            return PartialView("_AddEditFreightFees", FreightFees);
        }

        public ActionResult CreateOrUpdateFreightFees(FreightFee model)
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
                    db.FreightFees.Add(model);
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
            FreightFee FreightFees = db.FreightFees.Find(id);
            if (FreightFees == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.Id == FreightFees.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var FreightFeesDelete = db.FreightFees.Find(id);
            db.FreightFees.Remove(FreightFeesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FreightFee FreightFees = db.FreightFees.Find(id);
            if (FreightFees == null)
            {
                return HttpNotFound();
            }


            var item = db.FreightFees.Single(x => x.Id == id);





            var FreightFeesViewModelList = new FreightFeeViewModels
            {





                Id = item.Id,
                Name = item.Name,
                AmountInDollar = item.AmountInDollar,
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(FreightFeesViewModelList);
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
