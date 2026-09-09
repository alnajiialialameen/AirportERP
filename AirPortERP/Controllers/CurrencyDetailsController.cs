using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
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
    public class CurrencyDetailsController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: CurrencyDetails
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult GetAllCurrencyDetails()
        {
            var id = User.Identity.GetUserId();
            WorkOn workon = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault();
            IQueryable<CurrencyDetail> CurrencyDetails = db.CurrencyDetails.Where(x => x.YearWorkOn == workon.YearWorkOn
            && x.MonthWorkOn == workon.MonthWorkOn);

            //IQueryable<CurrencyDetail> CurrencyDetails = db.CurrencyDetails;
            var CurrencyDetailList = new List<CurrencyDetailsViewModel>();
            foreach (var item in CurrencyDetails)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                CurrencyDetailsViewModel r = new CurrencyDetailsViewModel();
                r.Id = item.Id;
                if (item.Date.HasValue)
                {
                    r.Day = item.Date.Value.Day.ToString();
                }
                else
                {
                    r.Day = "غير محدد";
                }
                r.CurrencyName = item?.Currency.CurrencyName;
                r.Month = item?.MonthWorkOn?.ToString();
                r.Year = Convert.ToInt32(item?.YearWorkOn);
                r.ExchangeRate = item?.ExchangeRateAgainstPrimaryCurrency.ToString();


                //});
                CurrencyDetailList.Add(r);
            }

            return Json(new { data = CurrencyDetailList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetCurrencyDetailParial(int? id)
        //{
        //    CurrencyDetail currencyDetail = new CurrencyDetail();
        //    //ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", currencyDetail.CurrencyId);
        //    //ViewBag.MonthWorkOnId = new SelectList(db.MouthWorkOns, "Id", "Mouth", currencyDetail.MonthWorkOnId);
        //    //ViewBag.YearWorkOnId = new SelectList(db.YearWorkOns, "Id", "Year", currencyDetail.YearWorkOnId);
        //    //  ViewBag.CountryId = new SelectList(db.Countries, "Id", "NameAr", CurrencyDetail.CurrencyId);
        //    if (id > 0)
        //    {

        //        currencyDetail = db.CurrencyDetails.Find(id);         

        //    }

        //    ViewBag.CurrencyId = new SelectList(db.Currencies.Where(x => x.Id != 2), "Id", "CurrencyName", currencyDetail.CurrencyId);
        //    ViewBag.MonthWorkOnId = new SelectList(db.MouthWorkOns, "Id", "Mouth", currencyDetail.MonthWorkOnId);
        //    ViewBag.YearWorkOnId = new SelectList(db.YearWorkOns, "Id", "Year", currencyDetail.YearWorkOnId);
        //    return PartialView("_AddEditcurrencyDetailDaily", currencyDetail);
        //}

        public ActionResult CreateOrUpdateCurrencyDetail(CurrencyDetail model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.CurrencyDetails.Any(x => x.MonthWorkOn == model.MonthWorkOn && x.YearWorkOn == model.YearWorkOn && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.CurrencyDetails.Any(x => x.YearWorkOn == model.YearWorkOn && x.YearWorkOn == model.YearWorkOn))
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




        //public ActionResult CreateOrUpdateCurrencyDetailDaily(CurrencyDetailDto model)
        //{
        //    CurrencyDetail currencyDetail = new CurrencyDetail();

        //    currencyDetail.Id = model.Id;
        //    currencyDetail.CurrencyId = model.CurrencyId;
        //    currencyDetail.MonthWorkOnId = model.MonthWorkOnId;
        //    currencyDetail.YearWorkOnId = model.YearWorkOnId;

        //    DateTime date = Convert.ToDateTime(model.Date);
        //    currencyDetail.Date = date;
        //    int day = date.Day;
        //    int Mouth = date.Month;
        //    int Year = date.Year;
        //    model.MonthWorkOnId = Mouth;
        //    int YearWorkOnId;

        //    if (db.YearWorkOns.Any(x => x.Year == Year))
        //    {
        //        YearWorkOnId = db.YearWorkOns
        //                              .Where(x => x.Year == Year)
        //                              .Select(x => x.Id)
        //                              .FirstOrDefault();
        //    }
        //    else { return Json(new { success = true, message = "YearNotFound" }, JsonRequestBehavior.AllowGet); }
        //    model.YearWorkOnId = YearWorkOnId;

        //    if (ModelState.IsValid)
        //    {

        //        if (model.Id > 0)
        //        {
        //            if (db.CurrencyDetails.Any(x => x.MonthWorkOnId == model.MonthWorkOnId && x.YearWorkOnId == model.YearWorkOnId && x.Date.HasValue &&
        //                x.Date.Value.Day == day && x.Id != model.Id))
        //            {
        //                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
        //            }

        //            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {

        //            if (db.CurrencyDetails.Any(x =>
        //                x.MonthWorkOnId == model.MonthWorkOnId &&
        //                x.YearWorkOnId == model.YearWorkOnId &&
        //                x.Date.HasValue &&
        //                x.Date.Value.Day == day))
        //            {
        //                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
        //            }


        //            db.CurrencyDetails.Add(currencyDetail);
        //            db.SaveChanges();
        //            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetCurrencyDetailParial(int? id)
        {
            CurrencyDetailDto model = new CurrencyDetailDto();

            if (id > 0)
            {
                var currencyDetail = db.CurrencyDetails.Find(id);

                if (currencyDetail != null)
                {
                    // ===== Manual Mapping Entity -> DTO =====
                    model.Id = currencyDetail.Id;
                    model.CurrencyId = currencyDetail.CurrencyId;
                    model.MonthWorkOnId = currencyDetail.MonthWorkOn;
                    model.YearWorkOnId = currencyDetail.YearWorkOn;

                    if (currencyDetail.Date.HasValue)
                        model.Date = CommonUsed.ToDisplayDate(currencyDetail.Date.Value);

                    model.ExchangeRateAgainstPrimaryCurrency = currencyDetail.ExchangeRateAgainstPrimaryCurrency;
                    model.IsActive = currencyDetail.IsActive;
                    model.CreatedBy = currencyDetail.CreatedBy;
                    model.CreationDate = currencyDetail.CreationDate;
                    model.UpdatedBy = currencyDetail.UpdatedBy;
                    model.UpdatingDate = currencyDetail.UpdatingDate;
                }
            }

            ViewBag.CurrencyId = new SelectList(
                db.Currencies.Where(x => x.Id != 2),
                "Id",
                "CurrencyName",
                model.CurrencyId);

            ViewBag.MonthWorkOnId = new SelectList(db.WorkOns, "Id", "Mouth", model.MonthWorkOnId);

            ViewBag.YearWorkOnId = new SelectList(
                db.WorkOns,
                "Id",
                "Year",
                model.YearWorkOnId);

            return PartialView("_AddEditcurrencyDetailDaily", model);
        }



        public ActionResult CreateOrUpdateCurrencyDetailDaily(CurrencyDetailDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);

            // تحويل التاريخ
            DateTime date = CommonUsed.ConvertToDbDate(model.Date);




            int day = date.Day;
            int month = date.Month;
            int year = date.Year;

            // استخراج YearWorkOnId
            var yearWork = year;
            if (yearWork == null)
                return Json(new { success = true, message = "YearNotFound" }, JsonRequestBehavior.AllowGet);

            int yearWorkOnId = yearWork;

            CurrencyDetail currencyDetail;

            if (model.Id > 0)
            {
                // ===== UPDATE =====

                if (db.CurrencyDetails.Any(x =>
                    x.MonthWorkOn == month &&
                    x.YearWorkOn == yearWorkOnId &&
                    x.Date.HasValue &&
                    x.Date.Value.Day == day &&
                    x.Id != model.Id))
                {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                }

                currencyDetail = db.CurrencyDetails.Find(model.Id);

                if (currencyDetail == null)
                    return Json(new { success = false, message = "NotFound" });
            }
            else
            {
                // ===== ADD =====

                if (db.CurrencyDetails.Any(x =>
                    x.MonthWorkOn == month &&
                    x.YearWorkOn == yearWorkOnId &&
                    x.Date.HasValue &&
                    x.Date.Value.Day == day))
                {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                }

                currencyDetail = new CurrencyDetail();
                db.CurrencyDetails.Add(currencyDetail);
            }

            // ===== Manual Mapping DTO -> Entity =====

            currencyDetail.CurrencyId = model.CurrencyId;
            currencyDetail.MonthWorkOn = month;
            currencyDetail.YearWorkOn = yearWorkOnId;
            currencyDetail.Date = date;
            currencyDetail.ExchangeRateAgainstPrimaryCurrency = model.ExchangeRateAgainstPrimaryCurrency;
            currencyDetail.IsActive = model.IsActive;
            currencyDetail.CreatedBy = model.CreatedBy;
            currencyDetail.CreationDate = model.CreationDate;
            currencyDetail.UpdatedBy = model.UpdatedBy;
            currencyDetail.UpdatingDate = model.UpdatingDate;

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = model.Id > 0 ? "Update" : "Add"
            }, JsonRequestBehavior.AllowGet);
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


        // POST: CurrencyDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

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
