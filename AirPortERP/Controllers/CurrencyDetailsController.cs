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
    public class CurrencyDetailsController : Controller
    {
        private Entities db = new Entities();

        // GET: CurrencyDetails
        public ActionResult Index()
        {
           
            return View();
        }


        public ActionResult GetAllCurrencyDetails()
        {

            IQueryable<CurrencyDetail> CurrencyDetails = db.CurrencyDetails;


            var CurrencyDetailList = new List<CurrencyDetailsViewModel>();
            foreach (var item in CurrencyDetails)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                CurrencyDetailsViewModel r = new CurrencyDetailsViewModel();
                r.Id = item.Id;
                r.CurrencyName = item?.Currency.CurrencyName;
                r.Month = item?.MouthWorkOn.Mouth;
                r.Year = Convert.ToInt32(item?.YearWorkOn.Year);
                r.ExchangeRate = item?.ExchangeRateAgainstPrimaryCurrency.ToString();

               
                //});
                CurrencyDetailList.Add(r);
            }

            return Json(new { data = CurrencyDetailList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCurrencyDetailParial(int? id)
        {




            CurrencyDetail currencyDetail = new CurrencyDetail();
            //ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", currencyDetail.CurrencyId);
            //ViewBag.MonthWorkOnId = new SelectList(db.MouthWorkOns, "Id", "Mouth", currencyDetail.MonthWorkOnId);
            //ViewBag.YearWorkOnId = new SelectList(db.YearWorkOns, "Id", "Year", currencyDetail.YearWorkOnId);
            //  ViewBag.CountryId = new SelectList(db.Countries, "Id", "NameAr", CurrencyDetail.CurrencyId);
            if (id > 0)
            {

                currencyDetail = db.CurrencyDetails.Find(id);
               

            }

            ViewBag.CurrencyId = new SelectList(db.Currencies.Where(x => x.Id != 2), "Id", "CurrencyName", currencyDetail.CurrencyId);
            ViewBag.MonthWorkOnId = new SelectList(db.MouthWorkOns, "Id", "Mouth", currencyDetail.MonthWorkOnId);
            ViewBag.YearWorkOnId = new SelectList(db.YearWorkOns, "Id", "Year", currencyDetail.YearWorkOnId);
            return PartialView("_AddEditcurrencyDetail", currencyDetail);

        }

        public ActionResult CreateOrUpdateCurrencyDetail(CurrencyDetail model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.CurrencyDetails.Any(x => x.MonthWorkOnId == model.MonthWorkOnId && x.YearWorkOnId == model.YearWorkOnId && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.CurrencyDetails.Any(x => x.MonthWorkOnId == model.MonthWorkOnId && x.YearWorkOnId == model.YearWorkOnId))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.CurrencyDetails.Add(model);
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
            CurrencyDetail CurrencyDetail = db.CurrencyDetails.Find(id);
            if (CurrencyDetail == null)
            {
                return HttpNotFound();
            }

            //if (db.CurrencyDetails.Any(x => x.AirPortId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var CurrencyDetailDelete = db.CurrencyDetails.Find(id);
            db.CurrencyDetails.Remove(CurrencyDetailDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }






        // GET: CurrencyDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrencyDetail currencyDetail = db.CurrencyDetails.Find(id);
            if (currencyDetail == null)
            {
                return HttpNotFound();
            }
            return View(currencyDetail);
        }

        // GET: CurrencyDetails/Create
        public ActionResult Create()
        {
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName");
            ViewBag.Month = new SelectList(db.MouthWorkOns, "Id", "Mouth");
            ViewBag.Year = new SelectList(db.YearWorkOns, "Id", "Id");
            return View();
        }

        // POST: CurrencyDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,CurrencyId,Month,Year,Date,ExchangeRateAgainstPrimaryCurrency,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] CurrencyDetail currencyDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CurrencyDetails.Add(currencyDetail);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", currencyDetail.CurrencyId);
        //    ViewBag.Month = new SelectList(db.MouthWorkOns, "Id", "Mouth", currencyDetail.Month);
        //    ViewBag.Year = new SelectList(db.YearWorkOns, "Id", "Id", currencyDetail.Year);
        //    return View(currencyDetail);
        //}

        // GET: CurrencyDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrencyDetail currencyDetail = db.CurrencyDetails.Find(id);
            if (currencyDetail == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", currencyDetail.CurrencyId);
            //ViewBag.Month = new SelectList(db.MouthWorkOns, "Id", "Mouth", currencyDetail.Month);
            //ViewBag.Year = new SelectList(db.YearWorkOns, "Id", "Id", currencyDetail.Year);
            return View(currencyDetail);
        }

        // POST: CurrencyDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CurrencyId,Month,Year,Date,ExchangeRateAgainstPrimaryCurrency,IsActive,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] CurrencyDetail currencyDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currencyDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", currencyDetail.CurrencyId);
            //ViewBag.Month = new SelectList(db.MouthWorkOns, "Id", "Mouth", currencyDetail.Month);
            //ViewBag.Year = new SelectList(db.YearWorkOns, "Id", "Id", currencyDetail.Year);
            return View(currencyDetail);
        }

        // GET: CurrencyDetails/Delete/5
     

        // POST: CurrencyDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CurrencyDetail currencyDetail = db.CurrencyDetails.Find(id);
            db.CurrencyDetails.Remove(currencyDetail);
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
